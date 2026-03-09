namespace OsuStat.UI.Dto;

public record GetBeatMapStatRequestDto(
    string filePath,
    int n300,
    int n100,
    int n50,
    int misses,
    int combo,
    int mods    
    );