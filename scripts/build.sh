#!/bin/bash

echo "Building AI Agent Plugin..."

# Check if dotnet is available
if command -v dotnet &> /dev/null; then
    echo "Using dotnet..."
    dotnet build --configuration Release
    if [ $? -eq 0 ]; then
        echo "Build successful!"
        echo ""
        echo "To install the mod:"
        echo "1. Copy the compiled DLL from build/bin/Release/net471/ to your ONI mods folder"
        echo "2. Copy config/mod_info.yaml and config/mod.yaml to the same folder"
        echo ""
        echo "Mod folder location: ~/.config/unity3d/Klei/Oxygen Not Included/mods/Dev/AIAgentPlugin/"
    else
        echo "Build failed!"
        exit 1
    fi
else
    echo "Error: dotnet not found. Please install .NET SDK."
    exit 1
fi
