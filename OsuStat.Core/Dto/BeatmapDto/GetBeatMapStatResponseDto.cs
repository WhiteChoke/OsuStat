namespace OsuStat.UI.Dto;

public record GetBeatMapStatResponseDto(
    string State,
    BeatMapDto Beatmap,
    double Pp,
    double MaxPp,
    double Acc
    );