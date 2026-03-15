using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class DataService : IDataService
{
    private readonly ISettingsService _settingsService;
    private readonly PlayerStat _playerStat;
    private readonly ILogger<DataService> _logger;

    public DataService(PlayerStat playerStat, ISettingsService settingsService, ILogger<DataService> logger)
    {
        _logger = logger;
        _playerStat = playerStat;
        _settingsService = settingsService;
    }
    
    public async Task SaveDataAsync<T>(T data, string directory)
    {
        try
        {
            var path = GetTodayFilePath(directory);
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(path, json);
            _logger.LogInformation("Saved data to {path}", path);
        }
        catch (IOException e)
        {
            _logger.LogError("Failed to save data: {}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("unexpected error: {}", e.Message);
            throw;
        }
    }
    
    public async Task UploadData(ObservableCollection<BeatMap> beatmaps)
    {
        try
        {
            var playerFilePath = GetTodayFilePath(_settingsService.SavePlayerStatDirectoryPath);
            var scoreFilePath =  GetTodayFilePath(_settingsService.SaveScoreDirectoryPath);
            
            var jsonPlayer = await File.ReadAllTextAsync(playerFilePath);
            var jsonScores = await File.ReadAllTextAsync(scoreFilePath);
            
            var playerStat = JsonSerializer.Deserialize<PlayerStat>(jsonPlayer) 
                ?? _playerStat;
            var scores = JsonSerializer.Deserialize<List<BeatMap>>(jsonScores)
                         ?? [];
            
            _playerStat.MapPlayed = playerStat.MapPlayed;
            _playerStat.AvgAccuracy = playerStat.AvgAccuracy ;
            _playerStat.AvgBpm = playerStat.AvgBpm;
            _playerStat.AvgStarRate = playerStat.AvgStarRate;
            _playerStat.PlayTimeMin = playerStat.PlayTimeMin;
            _playerStat.PpGained = playerStat.PpGained;
            
            foreach (var beatmap in scores)
                beatmaps.Add(beatmap);
            
            _logger.LogInformation("Data successful upload");
        }
        catch (IOException e)
        {
            _logger.LogError("Failed to read data file: {}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError("unexpected error: {}", e.Message);
            throw;
        }
    }

    private string GetTodayFilePath(string directory)
    {
        return Path.Combine(directory, $"{DateTime.Today:yy-MM-dd}.json");
    }
}