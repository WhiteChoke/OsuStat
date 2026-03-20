using OsuParsers.Enums;

namespace OsuStat.UI.Dto;

public record ReplayResultDto(
    string Name,
    string Artist,
    string Mapper,
    double Bpm,
    int Length,
    double StarRate,
    double Hp,
    double Cs,
    double Ar,
    string? BgPath,
    double PpGained,
    ushort MaxCombo,
    double Accuracy,
    List<Mods> Mods
    );