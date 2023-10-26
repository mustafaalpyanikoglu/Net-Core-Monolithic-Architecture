using Microsoft.Extensions.Configuration;
using Serilog;
using WebAPI.Controllers.Logging.Serilog;
using WebAPI.Controllers.Logging.Serilog.Messages;
using WebAPI.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;

namespace WebAPI.CrossCuttingConcerns.Logging.Serilog.Logger;

/// <summary>
/// Represents a logger that writes log entries to a file using Serilog.
/// </summary>
public class FileLogger : LoggerServiceBase
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the FileLogger class with the specified configuration.
    /// </summary>
    /// <param name="configuration">The IConfiguration object used to read Serilog configurations.</param>
    public FileLogger(IConfiguration configuration)
    {
        _configuration = configuration;

        // Get the FileLogConfiguration from the appsettings.json configuration section "SeriLogConfigurations:FileLogConfiguration".
        FileLogConfiguration logConfig =
            configuration.GetSection("SeriLogConfigurations:FileLogConfiguration").Get<FileLogConfiguration>()
            ?? throw new Exception(SerilogMessages.NullOptionsMessage);

        // Create the log file path by combining the current directory and the folder path from the configuration.
        string logFilePath = string.Format(format: "{0}{1}", arg0: Directory.GetCurrentDirectory() + logConfig.FolderPath, arg1: ".txt");

        // Configure Serilog to write log entries to the specified log file.
        Logger = new LoggerConfiguration().WriteTo
            .File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 5000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
            )
            .CreateLogger();
    }
}
