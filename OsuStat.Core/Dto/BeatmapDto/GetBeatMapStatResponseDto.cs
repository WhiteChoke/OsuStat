namespace OsuStat.Core.Dto.BeatmapDto;

public record GetBeatMapStatResponseDto(
    string State,
    BeatMapDto Beatmap,
    double Pp,
    double MaxPp,
    double Acc
    );