namespace OsuStat.Core.Model.Dto;

public record GetMapStatRequestDto(
    string filePath,
    int n300,
    int n100,
    int n50,
    int combo,
    int misses,
    int mods
    );