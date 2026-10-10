# Build and Publish

You can easily compile the application into a single executable using the provided scripts:

- **Windows**: Double-click `scripts\publish.bat`
- **Linux/WSL**: Run `./scripts/publish.sh`

This will execute the standard .NET publish command under the hood.

To build and copy the Windows output with Docker Compose, run:

```bash
docker compose up --build build
```

The published files are copied to `dist/windows/`. `docker compose build` only creates the image; the container must run to copy the files.
