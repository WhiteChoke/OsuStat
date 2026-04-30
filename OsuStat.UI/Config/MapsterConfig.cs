using System.IO;
using Mapster;
using OsuParsers.Enums;
using OsuStat.Core.Model;
using OsuStat.Data.Models;
using OsuStat.UI.MVVM.Model;
using OsuStat.UI.Service;

namespace OsuStat.UI.Config;

public static class MapsterConfig
{
    public static void Configure(ISettingsService settingsService)
    {
        TypeAdapterConfig<ReplayData, BeatmapEntity>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Artist, src => src.Artist)
            .Map(dest => dest.Mapper, src => src.Mapper)
            .Map(dest => dest.Bpm, src => src.Bpm)
            .Map(dest => dest.Length, src => src.Length)
            .Map(dest => dest.StarRate, src => src.StarRate)
            .Map(dest => dest.Hp, src => src.Hp)
            .Map(dest => dest.Cs, src => src.Cs)
            .Map(dest => dest.Ar, src => src.Ar)
            .Map(dest => dest.BgPath, src => src.BgPath)
            .Map(dest => dest.BeatmapHash, src => src.BeatmapHash);

        TypeAdapterConfig<ReplayData, PlayEntity>.NewConfig()
            .Map(dest => dest.PpGained, src => src.PpGained)
            .Map(dest => dest.Combo, src => src.Combo)
            .Map(dest => dest.Accuracy, src => src.Accuracy)
            .Map(dest => dest.Mods, src => src.Mods)
            .Map(dest => dest.Grade, src => src.Grade);

        TypeAdapterConfig<BeatmapEntity, BeatMap>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Artist, src => src.Artist)
            .Map(dest => dest.Mapper, src => src.Mapper)
            .Map(dest => dest.Bpm, src => src.Bpm)
            .Map(dest => dest.Length, src => src.Length)
            .Map(dest => dest.StarRate, src => src.StarRate)
            .Map(dest => dest.Hp, src => src.Hp)
            .Map(dest => dest.Cs, src => src.Cs)
            .Map(dest => dest.Ar, src => src.Ar)
            .Map(dest => dest.BgPath, src => src.BgPath);
        
        TypeAdapterConfig<ReplayData, BeatMap>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Artist, src => src.Artist)
            .Map(dest => dest.Mapper, src => src.Mapper)
            .Map(dest => dest.Bpm, src => src.Bpm)
            .Map(dest => dest.Length, src => src.Length)
            .Map(dest => dest.StarRate, src => src.StarRate)
            .Map(dest => dest.Hp, src => src.Hp)
            .Map(dest => dest.Cs, src => src.Cs)
            .Map(dest => dest.Ar, src => src.Ar)
            .Map(dest => dest.BgPath, src => src.BgPath);

        TypeAdapterConfig<ReplayData, Play>.NewConfig()
            .Map(dest => dest.PpGained, src => src.PpGained)
            .Map(dest => dest.MaxCombo, src => src.Combo)
            .Map(dest => dest.Accuracy, src => src.Accuracy)
            .Map(dest => dest.Mods, src => ConvertMods(settingsService, src.Mods))
            .Map(dest => dest.Grade, src => src.Grade)
            .Map(dest => dest.TimeStamp, src => src.TimeStamp); 
        
        TypeAdapterConfig<PlayEntity, Play>.NewConfig()
            .Map(dest => dest.PpGained, src => src.PpGained)
            .Map(dest => dest.MaxCombo, src => src.Combo)
            .Map(dest => dest.Accuracy, src => src.Accuracy)
            .Map(dest => dest.Mods, src => ConvertMods(settingsService, src.Mods))
            .Map(dest => dest.Grade, src => src.Grade);

        TypeAdapterConfig<PlayerStat, PlayerStatEntity>.NewConfig()
            .Map(dest => dest.PlayTimeMin, src => src.PlayTimeMin)
            .Map(dest => dest.MapPlayed, src => src.MapPlayed)
            .Map(dest => dest.PpGained, src => src.PpGained)
            .Map(dest => dest.AvgBpm, src => src.AvgBpm)
            .Map(dest => dest.AvgStarRate, src => src.AvgStarRate)
            .Map(dest => dest.AvgAccuracy, src => src.AvgAccuracy)
            .Map(dest => dest.Date, src => DateTime.Today);

        TypeAdapterConfig<BeatmapEntity, BeatMap>.NewConfig()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Artist, src => src.Artist)
            .Map(dest => dest.Mapper, src => src.Mapper)
            .Map(dest => dest.Bpm, src => src.Bpm)
            .Map(dest => dest.Length, src => src.Length)
            .Map(dest => dest.StarRate, src => src.StarRate)
            .Map(dest => dest.Hp, src => src.Hp)
            .Map(dest => dest.Cs, src => src.Cs)
            .Map(dest => dest.Ar, src => src.Ar)
            .Map(dest => dest.BgPath, src => src.BgPath);
    }
    
    private static List<string> ConvertMods(ISettingsService settings, List<Mods> mods)
    {
        return 
            mods.Select(mod => 
                    Path.Combine(settings.ModIconsFolder, $"{mod}.png"))
                .ToList();
    }
}