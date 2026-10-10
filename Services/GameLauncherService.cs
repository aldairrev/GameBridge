using System.Diagnostics;
using GameBridge.Models;

namespace GameBridge.Services;

/// <summary>
/// Service responsible for validating, launching, and monitoring the lifecycle of game processes.
/// </summary>
public sealed class GameLauncherService
{
    /// <summary>
    /// Validates whether the game executable exists on the file system.
    /// Resolves relative paths against the GameBridge base directory.
    /// </summary>
    /// <param name="game">The game model to validate.</param>
    /// <param name="resolvedPath">The resulting absolute path.</param>
    /// <returns>True if the file exists; otherwise, False.</returns>
    public bool ValidateGamePath(Game game, out string resolvedPath)
    {
        resolvedPath = string.Empty;

        if (game == null || string.IsNullOrWhiteSpace(game.Path))
        {
            return false;
        }

        try
        {
            resolvedPath = Path.IsPathRooted(game.Path)
                ? game.Path
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, game.Path));

            return File.Exists(resolvedPath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Launches the specified game executable and asynchronously awaits its initial process termination.
    /// </summary>
    /// <param name="executablePath">Absolute path to the executable.</param>
    /// <param name="onStarted">Callback action invoked once the process successfully starts (to hide the UI).</param>
    /// <returns>A task that completes when the game process exits.</returns>
    /// <remarks>
    /// Documented limitation:
    /// If the game uses an intermediate bootstrap launcher that spawns an independent child process
    /// and immediately exits the parent process, GameBridge will detect the parent exit and close.
    /// To avoid intrusive code injection or kernel hooks, GameBridge strictly tracks the initial process.
    /// </remarks>
    public async Task LaunchAndTrackAsync(string executablePath, Action onStarted)
    {
        string? workingDirectory = Path.GetDirectoryName(executablePath);
        if (string.IsNullOrEmpty(workingDirectory))
        {
            workingDirectory = AppContext.BaseDirectory;
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = executablePath,
            WorkingDirectory = workingDirectory,
            UseShellExecute = false
        };

        using var process = new Process { StartInfo = startInfo };

        bool started = process.Start();
        if (!started)
        {
            throw new InvalidOperationException($"Failed to initialize process for: {executablePath}");
        }

        onStarted();

        await process.WaitForExitAsync();
    }
}
