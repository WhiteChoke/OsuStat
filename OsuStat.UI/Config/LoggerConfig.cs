using System.IO;
using Serilog;
using Serilog.Core;

namespace OsuStat.UI.Config;

public static class LoggerConfig
{
    public static Logger GetLogger()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Osu stat", "Logs", "log.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
    }
}