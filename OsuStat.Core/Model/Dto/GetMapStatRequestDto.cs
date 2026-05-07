namespace OsuStat.Core.Model.Dto;

public record GetMapStatRequestDto(
    string filePath,
    ushort n300,
    ushort n100,
    ushort n50,
    ushort combo,
    ushort misses,
    int mods
    );