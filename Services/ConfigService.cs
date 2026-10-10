using System.Text.Json;
using GameBridge.Models;

namespace GameBridge.Services;

/// <summary>
/// Service responsible for locating, loading, and validating the config.json file.
/// </summary>
public sealed class ConfigService
{
    private const string ConfigFileName = "config.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    /// <summary>
    /// Gets the absolute path of config.json in the executable's base directory.
    /// Resolves against AppContext.BaseDirectory to avoid depending on Steam's working directory.
    /// </summary>
    public string GetConfigPath()
    {
        var exePath = Environment.ProcessPath;
        var baseDir = exePath != null ? Path.GetDirectoryName(exePath) : AppContext.BaseDirectory;
        return Path.Combine(baseDir ?? AppContext.BaseDirectory, ConfigFileName);
    }

    /// <summary>
    /// Safely reads and deserializes the configuration.
    /// Returns null if the file does not exist, is empty, contains invalid JSON syntax, or has no games.
    /// </summary>
    public GameConfig? LoadConfig()
    {
        string configPath = GetConfigPath();

        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"The configuration file does not exist at path: {configPath}");
        }

        string jsonContent = File.ReadAllText(configPath);
        if (string.IsNullOrWhiteSpace(jsonContent))
        {
            throw new InvalidOperationException("The config.json file is empty.");
        }

        var config = JsonSerializer.Deserialize<GameConfig>(jsonContent, JsonOptions);
        if (config?.Games == null || config.Games.Count == 0)
        {
            throw new InvalidOperationException("The file contains no games or has an invalid format.");
        }

        config.Games.RemoveAll(g => string.IsNullOrWhiteSpace(g.Name) || string.IsNullOrWhiteSpace(g.Path));

        if (config.Games.Count == 0)
        {
            throw new InvalidOperationException("All games in config.json have blank properties.");
        }

        return config;
    }
}
