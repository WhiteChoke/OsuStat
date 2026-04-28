using System.IO;
using MapsterMapper;
using OsuParsers.Enums;
using OsuStat.Data.Models;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;

namespace OsuStat.UI.Mapper;

public class BeatmapMapper
{
    private readonly IMapper _mapper;
    private readonly ISettingsService _settings;
    
    public BeatmapMapper(IMapper mapper, ISettingsService settings)
    {
        _mapper = mapper;
        _settings = settings;
    }
    
    public BeatMap ToBeatMap(PlayEntity play)
    {
        var beatmap = _mapper.Map<BeatMap>(play.Beatmap);
        beatmap.Accuracy = play.Accuracy;
        beatmap.MaxCombo = play.Combo;
        beatmap.PpGained = play.PpGained;
        beatmap.Mods = ConvertMods(play.Mods);
        
        return beatmap;
    }
    
    private List<string> ConvertMods(List<Mods> mods)
    {
        return 
            mods.Select(mod => 
                    Path.Combine(_settings.ModIconsFolder, $"{mod}.png"))
                .ToList();
    }

}