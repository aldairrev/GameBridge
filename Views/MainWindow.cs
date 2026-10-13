using GameBridge.Models;
using GameBridge.Services;

namespace GameBridge.Views;

/// <summary>
/// Windows Forms window for game selection and redirection.
/// </summary>
public sealed class MainWindow : Form
{
    private readonly GameConfig _config;
    private readonly GameLauncherService _launcherService;
    private readonly ComboBox _gamesComboBox;
    private readonly Button _launchButton;
    private readonly Button _exitButton;
    private readonly Panel _statusPanel;
    private readonly Label _statusLabel;

    public MainWindow() : this(new GameConfig(), new GameLauncherService())
    {
    }

    public MainWindow(GameConfig config, GameLauncherService launcherService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _launcherService = launcherService ?? throw new ArgumentNullException(nameof(launcherService));

        Text = "GameBridge";
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(390, 200);
        Font = new Font("Segoe UI", 9F);
        KeyPreview = true;

        var instructionLabel = new Label
        {
            AutoSize = true,
            Text = "Select a game to launch:",
            Location = new Point(20, 20)
        };

        _gamesComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Location = new Point(20, 45),
            Size = new Size(350, 30)
        };
        _gamesComboBox.Items.AddRange(_config.Games.ToArray());

        _statusLabel = new Label
        {
            AutoSize = true,
            ForeColor = Color.Green,
            Location = new Point(10, 10),
            MaximumSize = new Size(328, 0)
        };

        _statusPanel = new Panel
        {
            BorderStyle = BorderStyle.FixedSingle,
            Location = new Point(20, 85),
            Size = new Size(350, 45),
            Visible = true
        };
        _statusPanel.Controls.Add(_statusLabel);

        _launchButton = new Button
        {
            Text = "Launch Game",
            Font = new Font(Font, FontStyle.Bold),
            Location = new Point(250, 145),
            Size = new Size(120, 32),
            UseVisualStyleBackColor = true
        };
        _launchButton.Click += OnLaunchClicked;

        _exitButton = new Button
        {
            Text = "Exit",
            Location = new Point(160, 145),
            Size = new Size(80, 32),
            UseVisualStyleBackColor = true
        };
        _exitButton.Click += (_, _) => Close();

        Controls.AddRange(
        [
            instructionLabel,
            _gamesComboBox,
            _statusPanel,
            _launchButton,
            _exitButton
        ]);

        _gamesComboBox.SelectedIndexChanged += OnGameSelectionChanged;

        if (_config.Games.Count > 0)
        {
            _gamesComboBox.SelectedIndex = 0;
        }

        KeyDown += OnWindowKeyDown;
    }

    private void OnGameSelectionChanged(object? sender, EventArgs e)
    {
        if (_gamesComboBox.SelectedItem is not Game selectedGame)
        {
            return;
        }

        var statusBuilder = new System.Text.StringBuilder();

        if (!_launcherService.ValidateGamePath(selectedGame, out string resolvedPath))
        {
            statusBuilder.AppendLine("[-] Validating game path... [ERROR]");
            statusBuilder.AppendLine($"    Path not found: {selectedGame.Path}");
            ShowStatus(statusBuilder.ToString(), Color.Red, false);
            return;
        }
        statusBuilder.AppendLine("[+] Validating game path... [OK]");

        string[] allowedExtensions = [".exe", ".bat", ".cmd", ".lnk"];
        string extension = Path.GetExtension(resolvedPath).ToLowerInvariant();
        
        if (!allowedExtensions.Contains(extension))
        {
            statusBuilder.AppendLine("[-] Checking executable format... [ERROR]");
            statusBuilder.AppendLine($"    Unsupported file type: '{extension}'. Must be one of: {string.Join(", ", allowedExtensions)}");
            ShowStatus(statusBuilder.ToString(), Color.Red, false);
            return;
        }
        statusBuilder.AppendLine("[+] Checking executable format... [OK]");

        statusBuilder.AppendLine("[+] Ready to launch.");
        ShowStatus(statusBuilder.ToString(), Color.Green, true);
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Close();
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Enter && _launchButton.Enabled)
        {
            LaunchSelectedGame();
            e.Handled = true;
        }
    }

    private void OnLaunchClicked(object? sender, EventArgs e)
    {
        LaunchSelectedGame();
    }

    private async void LaunchSelectedGame()
    {
        if (_gamesComboBox.SelectedItem is not Game selectedGame)
        {
            return;
        }

        if (!_launcherService.ValidateGamePath(selectedGame, out string resolvedPath))
        {
            OnGameSelectionChanged(this, EventArgs.Empty);
            return;
        }

        try
        {
            SetControlsEnabled(false);
            ShowStatus("[+] Launching game... [WAITING]", Color.Blue, false);

            await _launcherService.LaunchAndTrackAsync(resolvedPath, Hide, selectedGame.Arguments);
            Close();
        }
        catch (Exception ex)
        {
            Show();
            SetControlsEnabled(true);
            
            var statusBuilder = new System.Text.StringBuilder();
            statusBuilder.AppendLine("[+] Validating game path... [OK]");
            statusBuilder.AppendLine("[+] Checking executable format... [OK]");
            statusBuilder.AppendLine("[-] Launching game... [ERROR]");
            statusBuilder.AppendLine($"    {ex.Message}");
            
            ShowStatus(statusBuilder.ToString(), Color.Red, false);
        }
    }

    private void ShowStatus(string message, Color color, bool canLaunch)
    {
        _statusLabel.Text = message;
        _statusLabel.ForeColor = color;
        _statusPanel.Height = Math.Max(45, _statusLabel.Height + 20);
        _launchButton.Top = _statusPanel.Bottom + 15;
        _exitButton.Top = _launchButton.Top;
        ClientSize = new Size(390, _launchButton.Bottom + 20);
        _statusPanel.Visible = true;
        _launchButton.Enabled = canLaunch;
    }

    private void SetControlsEnabled(bool enabled)
    {
        _gamesComboBox.Enabled = enabled;
        _launchButton.Enabled = enabled;
        _exitButton.Enabled = enabled;
    }
}
