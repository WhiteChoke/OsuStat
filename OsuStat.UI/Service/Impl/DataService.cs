using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Logging;
using OsuStat.Core.Extractor;
using OsuStat.Data.Repository;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class DataService : IDataService
{
    private readonly ISettingsService _settingsService;
    private readonly PlayerStat _playerStat;
    private readonly BestScore _bestScore;
    private readonly ILogger<DataService> _logger;
    private readonly HttpClient _httpClient = new();
    private const string Url = "https://a.ppy.sh/";
    private readonly PlayerStatRepository _playerStatRepository;
    private readonly PlayRepository _playRepository;
    

    
    public DataService(
        PlayerStat playerStat,
        ISettingsService settingsService,
        ILogger<DataService> logger,
        BestScore bestScore,
        PlayerStatRepository playerStatRepository,
        PlayRepository playRepository)
    {
        _playRepository = playRepository;
        _playerStatRepository = playerStatRepository;
        _logger = logger;
        _playerStat = playerStat;
        _settingsService = settingsService;
        _bestScore = bestScore;
    }
    
    public async Task SaveDataAsync<T>(T data, string directory)
    {
        try
        {
            var path = GetTodayFilePath(directory);
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json);
            _logger.LogInformation("Data saved to {path}", path);
        }
        catch (IOException e)
        {
            _logger.LogError("Failed to save data: {Message}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error: {Message}", e.Message);
            throw;
        }
    }
    
    public async Task LoadStatisticAsync(ObservableCollection<BeatMap> beatmaps)
    {
        
        var statEntity = await _playerStatRepository.GetStatByDateAsync(DateTime.Today) ??
                         await _playerStatRepository.CreateStat();
        
        _playerStat.MapPlayed = statEntity.MapPlayed;
        _playerStat.PlayTimeMin = statEntity.PlayTimeMin;
        _playerStat.AvgStarRate = statEntity.AvgStarRate;
        _playerStat.AvgAccuracy = statEntity.AvgAccuracy;
        _playerStat.AvgAccuracy = statEntity.AvgAccuracy;
        _playerStat.AvgBpm = statEntity.AvgBpm;
        
        var playsEntityList = await _playRepository.GetPlaysByDate(DateTime.Today);

        if (playsEntityList.Count == 0)
            return;
        

        var max = playsEntityList.MaxBy(p => p.PpGained);

        _bestScore.BgPath = max.Beatmap.BgPath;
        _bestScore.MapName = max.Beatmap.Name;
        _bestScore.Pp = max.PpGained;


        foreach (var play in playsEntityList)
        {
            var mapPlayCount = playsEntityList.Count(p => p.Id == play.Id);
            var beatmap = new BeatMap
            {
                PlayCount = mapPlayCount,
                Mods = play.Mods,
                Grade = play.Grade,
                PpGained = play.PpGained,
                MaxCombo = play.Combo,
                Accuracy = play.Accuracy,
                Name = play.Beatmap.Name,
                Artist = play.Beatmap.Artist,
                Mapper = play.Beatmap.Mapper,
                Bpm = play.Beatmap.Bpm,
                Length = play.Beatmap.Length,
                StarRate = play.Beatmap.StarRate,
                Hp = play.Beatmap.Hp,
                Cs = play.Beatmap.Cs,
                Ar = play.Beatmap.Ar,
                BgPath = play.Beatmap.BgPath
            };
            
            beatmaps.Add(beatmap);
        }
    }

    public async Task LoadUserInformationAsync(Player player)
    {
        player.AvatarPath = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Default avatar.jpeg");

        try
        {
            var playerInfo = PlayerExtractor.Extract(_settingsService.GameFolder);
            player.Nickname = playerInfo.Nickname;
            player.GlobalRanking = "#" + playerInfo.Ranking;

            var avatarPath = await GetAvatar(playerInfo.Id, 5);
            if (avatarPath == null)
            {
                MessageBox.Show($"Failed to load avatar :(", "Failed", MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            player.AvatarPath = avatarPath;
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
        try
        {
            var pfpBytes = await _httpClient.GetByteArrayAsync(Url + id);
            var path = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Avatar.jpeg");

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
                        _logger.LogError("Failed to write avatar: {}", e.Message);
                    await Task.Delay(500);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("unexpected error: {}", e.Message);
        }
        return null;
    }

    private string GetTodayFilePath(string directory)
    {
        return Path.Combine(directory, $"{DateTime.Today:yy-MM-dd}.json");
    }
}