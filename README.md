# GameBridge

**GameBridge** is a lightweight Windows game launcher written in C# and .NET 10. It acts as an intermediary bridge for your external games, emulators, or applications, and stays open while the launched process is running so your main launcher can accurately track playtime.

The interface uses **Windows Forms** and the built-in Windows Desktop runtime; Avalonia and its UI dependencies are not required. Windows Forms is Windows-only, so Linux builds are no longer supported.

## Features

- Lightweight Windows Forms interface with keyboard navigation.
- JSON configuration that can be edited without recompiling.
- Tracks the launched process and closes when it exits.
- Resolves relative game paths against the GameBridge executable directory.

## Download and Install

You don't need to compile the code yourself. You can download the ready-to-use application:

1. Go to the **[Releases](../../releases/latest)** page on the right side of this repository.
2. Download the latest `GameBridge.zip` file.
3. Extract the `.zip` file into any folder.
4. Edit the `config.json` file to add your games.
5. Run `GameBridge.exe`.

## Development and Deployment

- For instructions on how to set up your environment, develop, and test changes locally, see [SETUP.md](SETUP.md).
- For instructions on how to compile and publish the executable, see [DEPLOYMENT.md](DEPLOYMENT.md).

## Keyboard Navigation

- **Arrow keys**: Select a game or move focus between controls.
- **Enter**: Launch the selected game.
- **Escape**: Exit GameBridge.
