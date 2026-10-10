using GameBridge.Models;
using GameBridge.Services;
using GameBridge.Views;

namespace GameBridge;

internal static class Program
{
    /// <summary>
    /// Application main entry point.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        var exePath = Environment.ProcessPath;
        var baseDir = exePath != null ? Path.GetDirectoryName(exePath) : AppContext.BaseDirectory;
        var logPath = Path.Combine(baseDir ?? AppContext.BaseDirectory, "app.log");

        void Log(string message)
        {
            try { File.AppendAllText(logPath, $"[{DateTime.Now}] {message}\n"); } catch { }
        }

        try
        {
            Log("=== Application starting ===");
            var configService = new ConfigService();
            
            Log("Loading config...");
            var config = configService.LoadConfig();

            if (config == null)
            {
                Log("Error: Returned configuration is null.");
                return;
            }
            if (config.Games.Count == 0)
            {
                Log("Error: No valid games found in config.json.");
                return;
            }

            Log($"Config loaded successfully. Games found: {config.Games.Count}");
            var launcherService = new GameLauncherService();
            
            Log("Initializing Application UI...");
            ApplicationConfiguration.Initialize();
            
            Log("Running MainWindow...");
            Application.Run(new MainWindow(config, launcherService));
            
            Log("Application closed normally.");
        }
        catch (Exception ex)
        {
            Log($"FATAL EXCEPTION: {ex}");
        }
    }
}
