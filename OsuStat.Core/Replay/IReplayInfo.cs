using OsuStat.Core.Dto;

namespace OsuStat.Core.Replay;

public interface IReplayInfo
{
    Task<ReplayResultDto?> Get(string replayPath, string osuDirectoryPath);
}