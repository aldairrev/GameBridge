FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /src

COPY GameBridge.csproj ./
RUN dotnet restore GameBridge.csproj

COPY . .

RUN dotnet publish GameBridge.csproj \
    -c Release \
    -r win-x64 \
    --self-contained false \
    -p:PublishSingleFile=true \
    -p:DebugType=None \
    -p:DebugSymbols=false \
    -o /app/publish/windows

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS export

COPY --from=builder /app/publish/windows/ /app/publish/windows/

ENTRYPOINT ["sh", "-c", "cp -rv /app/publish/windows/. /output/"]
