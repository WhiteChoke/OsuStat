using OsuStat.Core.Extractor;
using OsuStat.Core.Model;

namespace OsuStat.Core;

public static class PlayerInfo
{
    public static PlayerData Get(string osuDirectoryPath)
    {
        var data = PlayerExtractor.Extract(osuDirectoryPath);
        
        return data;
    }
}

