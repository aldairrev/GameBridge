#!/bin/bash
dotnet publish ./GameBridge.csproj -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./dist/windows
