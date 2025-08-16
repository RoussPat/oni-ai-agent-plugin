# AI Agent Plugin for Oxygen Not Included

A comprehensive AI agent plugin for Oxygen Not Included that provides a foundation for AI agents to interact with and play the game. The plugin includes a base agent framework, example agents, and a user interface for managing AI agents.

## Prerequisites

- .NET Framework 4.0 or later
- Visual Studio 2019+ or VS Code with C# extension
- Oxygen Not Included game installed

## Setup Instructions

### 1. Copy Game DLL References

You need to copy the following DLL files from your ONI installation to the `References/` folder:

**Game DLL Location:** `[Steam Installation]\steamapps\common\OxygenNotIncluded\OxygenNotIncluded_Data\Managed\`

**Required DLLs:**
- `Assembly-CSharp.dll`
- `Assembly-CSharp-firstpass.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`

Copy these files to: `references/`

### 2. Build the Plugin

**Windows:**
```cmd
scripts\build.bat
```

**Linux/Mac:**
```bash
./scripts/build.sh
```

Or manually:
```bash
dotnet build --configuration Release
```

### 3. Install the Plugin

1. Create the mod directory:
   - Windows: `%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Dev\AIAgentPlugin\`
   - Linux: `~/.config/unity3d/Klei/Oxygen Not Included/mods/Dev/AIAgentPlugin/`

2. Copy the following files to the mod directory:
   - `build/bin/Release/net471/AIAgentPlugin.dll`
   - `config/mod_info.yaml`
   - `config/mod.yaml`

### 4. Launch the Game

Start Oxygen Not Included and check the debug log for the message:
```
[AI Agent Plugin] Mod loaded: AIAgentPlugin v1.0.0
```

## Project Structure

```
oni-ai-agent-plugin/
├── src/                          # Source code
│   ├── Core/                     # Core plugin functionality
│   │   ├── Mod.cs               # Main mod entry point
│   │   └── AIAgentPlugin.cs     # Plugin loader
│   ├── Agents/                   # AI agent implementations
│   │   ├── AIAgent.cs           # Base agent class
│   │   ├── HelloWorldAgent.cs   # Example agent
│   │   ├── GPTAgent.cs          # GPT-powered agent
│   │   └── AgentManager.cs      # Agent management
│   ├── UI/                       # User interface components
│   │   ├── AgentControlDialog.cs
│   │   ├── SimpleConfigDialog.cs
│   │   └── ... (other UI files)
│   ├── Patches/                  # Harmony patches
│   │   ├── MinimalSafePatches.cs
│   │   └── NoKeyboardPatches.cs
│   └── Config/                   # Configuration management
│       └── AIAgentConfigManager.cs
├── references/                   # Game DLL references (you need to copy these)
├── docs/                         # Documentation and guides
├── scripts/                      # Build and utility scripts
├── build/                        # Build artifacts
├── config/                       # Configuration files
├── AIAgentPlugin.csproj          # Project file
└── README.md                     # This file
```

## Features

### AI Agent System
- **Base Agent Framework**: Abstract AIAgent class for creating custom agents
- **Agent Manager**: Centralized management of all AI agents
- **Hello World Agent**: Example agent that demonstrates basic functionality
- **Agent Control UI**: In-game interface for managing agents
- **Real-time Monitoring**: Live status updates and agent statistics

### User Interface
- **Agent Control Dialog**: Create, start, stop, and manage agents
- **Keyboard Shortcuts**: Ctrl+Alt+A to open agent controls
- **In-game Integration**: AI button integrated into game UI
- **Logging System**: Comprehensive logging and debugging tools

## Usage

1. **Load the mod** in Oxygen Not Included
2. **Press Ctrl+Alt+A** or click the AI button to open agent controls
3. **Create a Hello World Agent** to see the system in action
4. **Monitor agent activity** through the logs and status display

For detailed information about the AI agent system, see [docs/AGENT_README.md](docs/AGENT_README.md).

## Development

The plugin provides a solid foundation for AI agent development:

1. **Extend AIAgent class** to create custom agents
2. **Use AgentManager** for centralized agent management
3. **Leverage the UI system** for agent controls
4. **Reference the Hello World agent** as a starting template

### Creating Custom Agents

```csharp
public class MyCustomAgent : AIAgent
{
    protected override void PerformAgentActions()
    {
        // Your agent logic here
        LogMessage("My agent is working!");
    }
}
```

See [docs/AGENT_README.md](docs/AGENT_README.md) for complete development guide.

## Notes

- The plugin uses Harmony 2.x for runtime patching
- Target framework is .NET 4.0 to match ONI requirements
- Game DLLs are not included due to licensing restrictions