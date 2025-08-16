#!/bin/bash

# Check if the mod is properly installed

# Find the correct Windows username
WINDOWS_USER=""
if [ -d "/mnt/c/Users/patri" ]; then
    WINDOWS_USER="patri"
elif [ -d "/mnt/c/Users/pat" ]; then
    WINDOWS_USER="pat"
else
    WINDOWS_USER=$(ls /mnt/c/Users/ | grep -v "All Users\|Default\|Public\|desktop.ini" | head -n1)
fi

MOD_DIR="/mnt/c/Users/$WINDOWS_USER/Documents/Klei/OxygenNotIncluded/mods/Dev/AIAgentPlugin"

echo "🔍 Checking mod installation..."
echo "📁 Mod directory: $MOD_DIR"
echo ""

if [ ! -d "$MOD_DIR" ]; then
    echo "❌ Mod directory does not exist!"
    exit 1
fi

echo "📂 Files in mod directory:"
ls -la "$MOD_DIR"
echo ""

# Check for required files
REQUIRED_FILES=("AIAgentPlugin.dll" "mod_info.yaml" "mod.yaml")
ALL_PRESENT=true

for file in "${REQUIRED_FILES[@]}"; do
    if [ -f "$MOD_DIR/$file" ]; then
        echo "✅ $file - present"
    else
        echo "❌ $file - missing"
        ALL_PRESENT=false
    fi
done

echo ""
if [ "$ALL_PRESENT" = true ]; then
    echo "🎉 All required files are present!"
    echo "📊 File sizes:"
    du -h "$MOD_DIR"/*
    echo ""
    echo "🔍 Checking for build ID in DLL..."
    if command -v strings >/dev/null 2>&1; then
        BUILD_ID=$(strings "$MOD_DIR/AIAgentPlugin.dll" | grep -E "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$" | head -1)
        if [ -n "$BUILD_ID" ]; then
            echo "🆔 Build ID: $BUILD_ID"
            echo "🆔 Short ID: ${BUILD_ID:0:8}"
        else
            echo "⚠️  Build ID not found in DLL"
        fi
    else
        echo "⚠️  'strings' command not available - cannot check build ID"
    fi
    echo ""
    echo "📋 Deployment Information:"
    if [ -f "$MOD_DIR/deployment_info.txt" ]; then
        cat "$MOD_DIR/deployment_info.txt" | sed 's/^/   /'
        echo ""
        DEPLOYED_BUILD_ID=$(grep "Build ID:" "$MOD_DIR/deployment_info.txt" | cut -d' ' -f3)
        echo "🎮 Expected log message on game start:"
        echo "   '[AI Agent Plugin] Mod loaded: AIAgentPlugin v1.0.0 (Build: $DEPLOYED_BUILD_ID)'"
    else
        # Fallback to source code build ID
        SOURCE_BUILD_ID=$(grep "BuildId =>" Source/AIAgentPlugin.cs | cut -d'"' -f2 2>/dev/null)
        echo "🎮 Expected log message on game start:"
        echo "   '[AI Agent Plugin] Mod loaded: AIAgentPlugin v1.0.0 (Build: ${SOURCE_BUILD_ID:-UNKNOWN})'"
    fi
else
    echo "⚠️  Some files are missing. Run './manual-deploy.sh' to install."
fi