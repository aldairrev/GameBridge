using System.Text.Json.Serialization;

namespace GameBridge.Models;

/// <summary>
/// Represents the structure of the config.json configuration file.
/// </summary>
public sealed class GameConfig
{
    /// <summary>
    /// List of available games configured for redirection.
    /// </summary>
    [JsonPropertyName("games")]
    public List<Game> Games { get; set; } = [];
}
