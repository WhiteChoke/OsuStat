using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Logging;
using OsuStat.Core;
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
    
    public DataService(
        PlayerStat playerStat,
        ISettingsService settingsService,
        ILogger<DataService> logger,
        BestScore bestScore)
    {
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
           var playerFilePath = GetTodayFilePath(_settingsService.SavePlayerStatDirectoryPath);
           var scoreFilePath =  GetTodayFilePath(_settingsService.SaveScoreDirectoryPath);
           var bestScorePath = GetTodayFilePath(Path.Combine(_settingsService.SaveScoreDirectoryPath, "Best"));
           
           _bestScore.BgPath = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Best score bg.jpg");
           
           try
           {
               var jsonPlayer = await File.ReadAllTextAsync(playerFilePath);
               var playerStat = JsonSerializer.Deserialize<PlayerStat>(jsonPlayer) 
                                ?? _playerStat;
               _playerStat.MapPlayed = playerStat.MapPlayed;
               _playerStat.AvgAccuracy = playerStat.AvgAccuracy ;
               _playerStat.AvgBpm = playerStat.AvgBpm;
               _playerStat.AvgStarRate = playerStat.AvgStarRate;
               _playerStat.PlayTimeMin = playerStat.PlayTimeMin;
               _playerStat.PpGained = playerStat.PpGained;
               
               _logger.LogInformation("Player statistic successful upload");
           }
           catch (Exception e)
           {
               _logger.LogWarning("Failed to load player statistic: {Message}", e.Message);
           }

           try
           {
               var jsonScores = await File.ReadAllTextAsync(scoreFilePath);
               var scores = JsonSerializer.Deserialize<List<BeatMap>>(jsonScores) 
                            ?? [];

               foreach (var beatmap in scores)
                   beatmaps.Add(beatmap);
           }
           catch (Exception e)
           {
               _logger.LogWarning("Failed to load scores: {Message}", e.Message);
           }

           try
           {
               var jsonBestScore = await File.ReadAllTextAsync(bestScorePath);
               var bestScore = JsonSerializer.Deserialize<BestScore>(jsonBestScore) 
                           ?? _bestScore;
               
               _bestScore.BgPath = bestScore.BgPath;
               _bestScore.MapName = bestScore.MapName;
               _bestScore.Pp = bestScore.Pp;
           }
           catch (Exception e)
           {
               _logger.LogWarning("Failed to load best score: {Message}", e.Message);
           }
    }

    public async Task LoadUserInformationAsync(Player player)
    {
        player.AvatarPath = Path.Combine(_settingsService.ApplicationFolder, "Assets", "Images", "Default avatar.jpeg");

        try
        {
            var playerInfo = PlayerInfo.GetPlayerInfo(_settingsService.GameFolder);
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
        catch (IOException e)
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