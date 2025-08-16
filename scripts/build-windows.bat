@echo off
echo Building AI Agent Plugin on Windows...

REM Change to the project directory
cd /d "\\wsl.localhost\Ubuntu\home\pat\projects\oni-ai-agent-plugin"

REM Try to find and use dotnet
if exist "C:\Program Files\dotnet\dotnet.exe" (
    echo Using dotnet from Program Files...
    "C:\Program Files\dotnet\dotnet.exe" build --configuration Release
    goto :check_result
)

if exist "C:\Program Files (x86)\dotnet\dotnet.exe" (
    echo Using dotnet from Program Files x86...
    "C:\Program Files (x86)\dotnet\dotnet.exe" build --configuration Release
    goto :check_result
)

REM Try to find dotnet in PATH
dotnet build --configuration Release 2>nul
if %errorlevel% equ 0 goto :success

echo Could not find dotnet. Trying MSBuild...

REM Try newer MSBuild versions first
if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe" AIAgentPlugin.csproj /p:Configuration=Release
    goto :check_result
)

if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" (
    "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" AIAgentPlugin.csproj /p:Configuration=Release
    goto :check_result
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" (
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe" AIAgentPlugin.csproj /p:Configuration=Release
    goto :check_result
)

if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" (
    "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" AIAgentPlugin.csproj /p:Configuration=Release
    goto :check_result
)

echo No suitable build tools found!
echo Please install .NET SDK or Visual Studio
pause
exit /b 1

:check_result
if %errorlevel% equ 0 goto :success
echo Build failed!
pause
exit /b 1

:success
echo Build successful!
echo DLL should be updated in build\bin\Release\net471\AIAgentPlugin.dll
pause