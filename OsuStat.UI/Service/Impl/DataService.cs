using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using OsuStat.UI.MVVM.Model;

namespace OsuStat.UI.Service.Impl;

public class DataService : IDataService
{
    private readonly ISettingsService _settings;
    private readonly PlayerStat _playerStat;
    
    public DataService(ISettingsService settingsService, PlayerStat playerStat)
    {
        _playerStat = playerStat;
        _settings = settingsService;
    }
    
    public void SaveScore(List<BeatMap> beatmap)
    {
        try
        {
            var json = JsonSerializer.Serialize(beatmap);
            File.WriteAllTextAsync(
                Path.Combine(_settings.SaveScoreDirectoryPath, DateTime.Today.ToShortDateString()) + ".json", json);
            Console.WriteLine("Score saved");
        }
        catch (IOException e)
        {
            MessageBox.Show($"Failed to save score\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void SaveStatistics(PlayerStat playerStat)
    {
        try
        {
            var json = JsonSerializer.Serialize(playerStat);
            File.WriteAllTextAsync(
                Path.Combine(_settings.SavePlayerStatDirectoryPath, DateTime.Today.ToShortDateString()) + ".json", json);
            Console.WriteLine("Player statistic saved");
        }
        catch (IOException e)
        {
            MessageBox.Show($"Failed to save player statistic\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async void UploadData(ObservableCollection<BeatMap> beatmaps)
    {
        try
        {
            if (File.Exists(Path.Combine(_settings.SavePlayerStatDirectoryPath, DateTime.Today.ToShortDateString()) + ".json"))
            {
                var json = await File.ReadAllTextAsync(
                    Path.Combine(_settings.SavePlayerStatDirectoryPath ,DateTime.Today.ToShortDateString() + ".json")
                    );
                
                var result = JsonSerializer.Deserialize<PlayerStat>(json);
                
                _playerStat.MapPlayed = result.MapPlayed;
                _playerStat.AvgAccuracy = result.AvgAccuracy ;
                _playerStat.AvgBpm = result.AvgBpm;
                _playerStat.AvgStarRate = result.AvgStarRate;
                _playerStat.PlayTimeMin = result.PlayTimeMin;
                _playerStat.PpGained = result.PpGained;
                
                Console.WriteLine("Player statistic uploaded");
            }

            if (File.Exists(Path.Combine(_settings.SaveScoreDirectoryPath, DateTime.Today.ToShortDateString()) + ".json"))
            {
                var json = await File.ReadAllTextAsync(
                    Path.Combine(_settings.SaveScoreDirectoryPath, DateTime.Today.ToShortDateString()) + ".json"
                );
                
                var scores = JsonSerializer.Deserialize<List<BeatMap>>(json) ?? [];

                foreach (var beatmap in scores)
                {
                    beatmaps.Add(beatmap);
                }
                
                Console.WriteLine("Scores uploaded");
            }
        }
        catch (IOException e)
        {
            MessageBox.Show($"Failed to upload statistic\n{e.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}