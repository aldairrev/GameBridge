# Local Setup and Development

## Development

To make changes and test them instantly during development, you **must** install the .NET 10 SDK locally on your Windows host. Because this is a Windows Forms application (native Windows UI), Linux environments (like Docker or WSL without interoperability setup) cannot run the graphical interface directly.

Run the application using the provided scripts:

- **Windows**: Double-click `scripts\run.bat` or run it from CMD/PowerShell.
- **Linux/WSL**: Run `./scripts/run.sh` from the terminal.
