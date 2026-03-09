namespace OsuStat.UI.Dto;

public record GetBeatMapStatResponseDto(
    string State,
    BeatMapDto beatmap,
    double Pp,
    double MaxPp
    );