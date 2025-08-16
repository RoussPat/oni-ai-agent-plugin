@echo off
echo Building AI Agent Plugin...

dotnet build --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo Build successful!
    echo.
    echo To install the mod:
    echo 1. Copy the compiled DLL from build\bin\Release\net471\ to your ONI mods folder
    echo 2. Copy config\mod_info.yaml and config\mod.yaml to the same folder
    echo.
    echo Mod folder location: %%USERPROFILE%%\Documents\Klei\OxygenNotIncluded\mods\Dev\AIAgentPlugin\
) else (
    echo Build failed with error code %ERRORLEVEL%
)

pause