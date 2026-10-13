using System.Text.Json.Serialization;

namespace GameBridge.Models;

/// <summary>
/// Represents the configuration for an individual game.
/// </summary>
public sealed class Game
{
    /// <summary>
    /// Display name of the game for the graphical selector.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Path to the game executable (absolute or relative to the GameBridge directory).
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Optional command-line arguments to pass to the executable.
    /// </summary>
    [JsonPropertyName("arguments")]
    public string? Arguments { get; set; }

    public override string ToString() => Name;
}
