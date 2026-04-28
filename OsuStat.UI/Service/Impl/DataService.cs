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
using OsuStat.UI.Mapper;
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
    private readonly BeatmapMapper _beatmapMapper;
    
    public DataService(
        ISettingsService settingsService,
        ILogger<DataService> logger,
        PlayerStatRepository playerStatRepository,
        PlayRepository playRepository,
        BeatmapRepository beatmapRepository,
        IMapper mapper,
        IDataStorage dataStorage,
        OsuStatDbContext dbContext,
        BeatmapMapper beatmapMapper)
    {
        _dataStorage = dataStorage;
        _mapper = mapper;
        _beatmapRepository = beatmapRepository;
        _playRepository = playRepository;
        _playerStatRepository = playerStatRepository;
        _logger = logger;
        _settingsService = settingsService;
        _dbContext = dbContext;
        _beatmapMapper = beatmapMapper;
    }
    
    public async void SaveAndUpdateAsyncEvent(object? sender, ReplayData replayData)
    {
        await using  var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var beatmapUi = await SavePlayData(replayData);
            var stat = await SaveStat(replayData);
            await transaction.CommitAsync();
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _dataStorage.PlayerStat.UpdateStatistic(stat);

                if (beatmapUi.PpGained > _dataStorage.BestScore.Pp)
                {
                    _dataStorage.BestScore.Update(
                        beatmapUi.Name,
                        beatmapUi.PpGained,
                        beatmapUi.BgPath
                    );
                }

                if (_dataStorage.Beatmaps.Contains(beatmapUi))
                {
                    var oldBeatmap = _dataStorage.Beatmaps.First(b => b.Equals(beatmapUi));
                    _dataStorage.Beatmaps.Remove(oldBeatmap);

                    beatmapUi.PpGained = oldBeatmap.PpGained >= beatmapUi.PpGained
                        ? oldBeatmap.PpGained
                        : beatmapUi.PpGained;

                    beatmapUi.PlayCount = oldBeatmap.PlayCount + 1;
                }
                else
                {
                    beatmapUi.PlayCount = 1;
                }

                _dataStorage.Beatmaps.Insert(0, beatmapUi);
            });

            _logger.Log(LogLevel.Information, "View successfully updated");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            _logger.LogCritical("Failed to save statistic due {message}", e.Message);
        }
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

    private async Task<BeatMap> SavePlayData(ReplayData replayData)
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
        playToCreate.PlayedAt = DateTime.Now;
        
        var savedPlay = await _playRepository.CreatePlay(playToCreate);
        savedPlay.Beatmap = beatmapEntity;
        
        _logger.LogInformation("Play saved successfully");
        
        return _beatmapMapper.ToBeatMap(savedPlay);
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


        foreach (var play in playsEntityList)
        {
            var mapPlayCount = playsEntityList.Count(p => p.Beatmap.BeatmapHash == play.Beatmap.BeatmapHash);
            var beatmap = _beatmapMapper.ToBeatMap(play);
            beatmap.PlayCount = mapPlayCount;
            
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