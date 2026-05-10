using System.Text.RegularExpressions;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.OsuMemoryModels.Direct;
using OsuStat.Core.Model;
using OsuStat.Core.Model.Structures;
using ProcessMemoryDataFinder;

namespace OsuStat.Core.Service.Impl;

public class GameWatcher
{
    private readonly BanchoUser _user =  new();
    private readonly GeneralData _general = new();
    private readonly CurrentBeatmap _currentBeatmap = new();
    private StructuredOsuMemoryReader _reader;
    private CurrentPlayMeta _currentPlayMeta = new();
    private readonly PlayedBeatmap _playedBeatmap; 
    // fuck events bro
    public Func<int, Task> PlayCompleted;
    public Func<int,Task> OnGameStarted;

    public GameWatcher(PlayedBeatmap playedBeatmap)
    {
        _playedBeatmap = playedBeatmap;
    }
    public async Task Start()
    {
        _reader = new StructuredOsuMemoryReader(new ProcessTargetOptions("osu!"));
        _ = GeneralObserver();
        await OnGameStarted.Invoke(await CurrentPp());
    }

    public async Task<int> CurrentPp()
    {
        var isRead = _reader.TryRead(_user);
        while (!isRead || string.IsNullOrEmpty(_user.UserPpAccLevel))
        {
            await Task.Delay(500);
            isRead = _reader.TryRead(_user);
        }
        var rawPp = Regex.Match(_user.UserPpAccLevel, "[\\d,.]+").Value;
        var pp = rawPp.Replace(",", "");
        
        return int.Parse(pp);
    }
    
    private async Task GeneralObserver()
    {
        while (true)
        {
            await Task.Delay(500);
            _reader.TryRead(_general);

            switch (_general.OsuStatus)
            {
                case OsuMemoryStatus.ResultsScreen:
                    if (_currentPlayMeta.IsCalculated)
                        break;
                    await PlayCompleted.Invoke(await CurrentPp());
                    _currentPlayMeta.IsCalculated = true;
                    break;
                case OsuMemoryStatus.Playing:
                    if (_reader.TryRead(_currentBeatmap))
                    {
                        if (_currentBeatmap.Md5 == _currentPlayMeta.Md5Hash && !_currentPlayMeta.IsCalculated) 
                            break;
                        _playedBeatmap.MapDirectory = _currentBeatmap.FolderName;
                        _playedBeatmap.OsuFileName = _currentBeatmap.OsuFileName;
                        _playedBeatmap.OsuBeatmapId = _currentBeatmap.Id;
                        
                        _currentPlayMeta.Md5Hash = _currentBeatmap.Md5;
                        _currentPlayMeta.IsCalculated = false;
                    }
                    break;
                default:
                    _currentPlayMeta.Md5Hash = string.Empty;
                    _currentPlayMeta.IsCalculated = false;
                    break;
            }
        }
    }
}