using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using OsuStat.Core;
using OsuStat.Core.Extractor;
using OsuStat.Core.Model;
using OsuStat.Data.Context;
using OsuStat.Data.Models;
using OsuStat.Data.Repository;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class DataService : IDataService
{
    private readonly ISettingsService _settingsService;
    private readonly ILogger<DataService> _logger;
    private readonly PlayerStatRepository _playerStatRepository;
    private readonly PlayRepository _playRepository;
    private readonly BeatmapRepository _beatmapRepository;
    private readonly IMapper _mapper;
    private readonly IDataStorage _dataStorage;
    private readonly OsuStatDbContext  _dbContext;
    
    public DataService(
        ISettingsService settingsService,
        ILogger<DataService> logger,
        PlayerStatRepository playerStatRepository,
        PlayRepository playRepository,
        BeatmapRepository beatmapRepository,
        IMapper mapper,
        IDataStorage dataStorage,
        OsuStatDbContext dbContext)
    {
        _dataStorage = dataStorage;
        _mapper = mapper;
        _beatmapRepository = beatmapRepository;
        _playRepository = playRepository;
        _playerStatRepository = playerStatRepository;
        _logger = logger;
        _settingsService = settingsService;
        _dbContext = dbContext;
    }
    
    public async void SaveAndUpdateAsyncEvent(object? sender, ReplayData replayData)
    {
        await using  var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        { 
            var play = _mapper.Map<Play>(replayData);
            var beatmap = _mapper.Map<BeatMap>(replayData);
            
            if (IsPlayExist(beatmap, play))
                return; 
            
            var stat = await SaveStat(replayData);
            await SavePlayData(replayData);
            await transaction.CommitAsync();
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _dataStorage.PlayerStat.UpdateStatistic(stat);

                if (play.PpGained > _dataStorage.BestScore.Pp)
                {
                    _dataStorage.BestScore.Update(
                        beatmap.Name,
                        play.PpGained,
                        beatmap.BgPath
                    );
                }

                if (_dataStorage.Beatmaps.Contains(beatmap))
                {
                    var observableBeatmap = _dataStorage.Beatmaps
                        .Select((item, index) => new { item, index })
                        .First(b => b.item.Equals(beatmap));

                    observableBeatmap.item.PlayCount++;
                    observableBeatmap.item.Plays.Add(play);
                    
                    _dataStorage.Beatmaps.Move(observableBeatmap.index, 0);
                }
                else
                {
                    beatmap.PlayCount = 1;
                    beatmap.Plays.Add(play);
                    _dataStorage.Beatmaps.Insert(0, beatmap);
                }

            });

            _logger.Log(LogLevel.Information, "View successfully updated");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            _logger.LogCritical("Failed to save statistic due {message}", e.Message);
        }
    }

    private bool IsPlayExist(BeatMap beatmap, Play play)
    {
        var foundBeatmap = _dataStorage.Beatmaps
            .FirstOrDefault(b => b.Equals(beatmap));
        
        if (foundBeatmap == null)
            return false;
        
        var foundPlay = foundBeatmap.Plays.FirstOrDefault(p => p.Equals(play));
        
        if (foundPlay == null)
            return false;

        return true;
    }

    private async Task<PlayerStatEntity> SaveStat(ReplayData replayData)
    {
        var statEntity = await _playerStatRepository.GetStatByDateAsync(DateTime.Today);
        
        statEntity.MapPlayed++;
        statEntity.SessionStarRateSum += replayData.StarRate;
        statEntity.SessionAccuracySum += replayData.Accuracy;
        statEntity.SessionBpmSum += replayData.Bpm;
        statEntity.AvgStarRate = statEntity.SessionStarRateSum / statEntity.MapPlayed;
        statEntity.AvgAccuracy = statEntity.SessionAccuracySum / statEntity.MapPlayed;
        statEntity.AvgBpm = statEntity.SessionBpmSum / statEntity.MapPlayed;

        await _playerStatRepository.UpdateTodayStatAsync(statEntity);
        
        _logger.LogInformation("Statistic updated successfully");
        
        return statEntity;
    }

    private async Task SavePlayData(ReplayData replayData)
    {
        var beatmapEntity = await _beatmapRepository.GetBeatmapsByHashCode(replayData.BeatmapHash);

        if (beatmapEntity == null)
        {
            var beatmapToCreate = _mapper.Map<BeatmapEntity>(replayData);
            beatmapEntity = await _beatmapRepository.CreateBeatmap(beatmapToCreate);
            _logger.LogInformation("Beatmap created successfully");
        }

        var playToCreate = _mapper.Map<PlayEntity>(replayData);
        
        playToCreate.BeatmapId = beatmapEntity.Id;
        playToCreate.PlayedAt = replayData.TimeStamp;
        
        await _playRepository.CreatePlay(playToCreate);
        
        _logger.LogInformation("Play saved successfully");
    }
    
    public async Task LoadStatisticAsync()
    {
        var statEntity = await _playerStatRepository.GetStatByDateAsync(DateTime.Today) ??
                         await _playerStatRepository.CreateStat();
        
        _dataStorage.PlayerStat.LoadStatistics(statEntity);
        
        var playsEntityList = await _playRepository.GetPlaysByDate(DateTime.Today);

        if (playsEntityList.Count == 0)
            return;
        

        var max = playsEntityList.MaxBy(p => p.PpGained);

        _dataStorage.BestScore.BgPath = max.Beatmap.BgPath;
        _dataStorage.BestScore.MapName = max.Beatmap.Name;
        _dataStorage.BestScore.Pp = max.PpGained;

        var groupedPlays = playsEntityList.GroupBy(p => p.BeatmapId);
        
        foreach (var group in groupedPlays)
        {
            var beatmap = _mapper.Map<BeatMap>(group.First().Beatmap);
            
            beatmap.PlayCount = group.Count();
            
            beatmap.Plays = new ObservableCollection<Play>(group.Select(p => _mapper.Map<Play>(p)));
            
            _dataStorage.Beatmaps.Add(beatmap);
        }
        
        _logger.LogInformation("Statistics loaded successfully");
    }

    public async Task LoadUserInformationAsync()
    {
        _dataStorage.Player.AvatarPath = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Default avatar.jpeg");

        try
        {
            var playerInfo = PlayerExtractor.Extract(_settingsService.GameFolder);
            _dataStorage.Player.Nickname = playerInfo.Nickname;
            _dataStorage.Player.GlobalRanking = "#" + playerInfo.Ranking;

            var avatarPath = await GetAvatar(playerInfo.Id, 5);
            if (avatarPath == null)
            {
                MessageBox.Show($"Failed to load avatar :(", "Failed", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            _dataStorage.Player.AvatarPath = avatarPath;
        }
        catch (IOException)
        {
            MessageBox.Show("Unable to find the osu folder\nPlease select the game folder in settings", "Invalid folder", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to load player data: {Message}", e.Message);
            throw;
        }
    }

    private async Task<string?> GetAvatar(int id,int retryCount)
    {
        var path = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Avatar.jpg");
        var pfpBytes = await Requests.GetAvatar(id);
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                await File.WriteAllBytesAsync(path, pfpBytes);
                return path;
            }
            catch (IOException e)
            {
                if (i == retryCount - 1)
                    _logger.LogError("Failed to write avatar\n {message}",  e.Message);
                await Task.Delay(500);
            }
        }
        return null;
    }
}