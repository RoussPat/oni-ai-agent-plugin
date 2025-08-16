# Build and Utility Scripts

This directory contains all build scripts and utilities for the AI Agent Plugin.

## Build Scripts

### Windows
- **[build.bat](build.bat)** - Simple build script for Windows
- **[build-windows.bat](build-windows.bat)** - Advanced build script with multiple tool detection

### Linux/Mac
- **[build.sh](build.sh)** - Build script for Linux and macOS

## Utility Scripts

### Development
- **[manage.sh](manage.sh)** - Main development management script
- **[generate-build-id.sh](generate-build-id.sh)** - Generate unique build identifiers

### Testing and Debugging
- **[test_input_fix.sh](test_input_fix.sh)** - Fix input testing issues
- **[debug_keyboard.sh](debug_keyboard.sh)** - Debug keyboard input problems
- **[check-install.sh](check-install.sh)** - Verify installation

### Setup
- **[dotnet-install.sh](dotnet-install.sh)** - Install .NET SDK

## Usage

### Building the Plugin

**Windows:**
```cmd
scripts\build.bat
```

**Linux/Mac:**
```bash
./scripts/build.sh
```

### Development Management

```bash
./scripts/manage.sh
```

This script provides various development tasks like:
- Building the project
- Running tests
- Cleaning build artifacts
- Installing dependencies

## Notes

- All scripts assume they're run from the project root directory
- Build artifacts are placed in the `build/` directory
- The main project file is `AIAgentPlugin.csproj` in the root directory
