# AI Agent System for Oxygen Not Included

This mod provides a foundation for AI agents to interact with and play Oxygen Not Included. The system includes a base agent framework, a Hello World example agent, and a user interface for managing agents.

## Features

### Base AI Agent Framework
- **AIAgent**: Abstract base class for all AI agents
- **AgentManager**: Central manager for creating and controlling agents
- **Agent States**: Idle, Running, Paused, Error, Thinking, Acting
- **Event System**: Logging, state changes, and lifecycle events

### Hello World Agent
- Demonstrates basic agent functionality
- Sends periodic greeting messages
- Shows state transitions and thinking behavior
- Configurable greeting interval and messages

### User Interface
- **Agent Control Dialog**: Create, start, stop, pause, and destroy agents
- **Real-time Status**: View agent states and statistics
- **Keyboard Shortcuts**: Ctrl+Alt+A to open agent control
- **In-game Button**: AI button integrated into game UI

## How to Use

### Starting the Agent System
1. Load the mod in Oxygen Not Included
2. The Agent Manager will automatically initialize
3. Look for the "AI" button in the game UI or press Ctrl+Alt+A

### Creating Your First Agent
1. Open the Agent Control Dialog (Ctrl+Alt+A or click AI button)
2. Click "Create Hello World Agent"
3. The agent will be created and started automatically
4. Watch the logs for greeting messages

### Managing Agents
- **Start All**: Start all stopped agents
- **Stop All**: Stop all running agents
- **Pause All**: Pause all running agents
- **Resume All**: Resume all paused agents
- **Destroy All**: Remove all agents
- **Refresh**: Update the agent list display

### Individual Agent Controls
Each agent in the list shows:
- Agent name and current state
- Start/Stop button
- Pause/Resume button
- Destroy button

## Creating Custom Agents

To create your own AI agent:

1. **Extend AIAgent class**:
```csharp
public class MyCustomAgent : AIAgent
{
    protected override void PerformAgentActions()
    {
        // Your agent logic here
        LogMessage("My agent is working!");
    }
    
    protected override float GetActionInterval()
    {
        return 5.0f; // Check every 5 seconds
    }
}
```

2. **Add to AgentManager**:
```csharp
public MyCustomAgent CreateMyCustomAgent()
{
    var agentObj = new GameObject("MyCustomAgent");
    agentObj.transform.SetParent(agentContainer.transform, false);
    
    var agent = agentObj.AddComponent<MyCustomAgent>();
    // Subscribe to events and add to active agents
    return agent;
}
```

3. **Add UI controls** in AgentControlDialog

## Agent States

- **Idle**: Agent is created but not running
- **Running**: Agent is active and performing actions
- **Paused**: Agent is temporarily stopped
- **Error**: Agent encountered an error
- **Thinking**: Agent is processing (custom state)
- **Acting**: Agent is performing an action (custom state)

## Logging and Debugging

- All agent messages are logged to the game console
- Use the SimpleLogsDialog to view agent activity
- Enable debug mode on agents for detailed logging
- Agent status is displayed in real-time in the control dialog

## Future Enhancements

This foundation supports future AI agent features:
- **Game State Analysis**: Reading colony status, resources, etc.
- **Decision Making**: AI logic for colony management
- **Action Execution**: Building, digging, assigning tasks
- **Learning Systems**: Adaptive behavior based on outcomes
- **Multi-Agent Coordination**: Multiple agents working together

## Technical Details

### Architecture
- **MonoBehaviour-based**: Agents run as Unity components
- **Coroutine-driven**: Non-blocking agent loops
- **Event-driven**: Loose coupling between components
- **Singleton Manager**: Centralized agent management

### Performance
- Agents run on configurable intervals (default: 1-2 seconds)
- Pause/resume functionality for performance control
- Automatic cleanup when agents are destroyed
- Minimal impact on game performance

### Extensibility
- Abstract base class for easy agent creation
- Event system for custom integrations
- Modular UI system for new controls
- Plugin architecture for additional features

## Troubleshooting

### Common Issues
1. **Agents not appearing**: Check that AgentManager is initialized
2. **UI not showing**: Verify keyboard shortcuts or look for AI button
3. **Build errors**: Ensure all using statements are correct
4. **Runtime errors**: Check agent exception handling

### Debug Mode
Enable debug mode on agents to see:
- State transitions
- Action execution details
- Error information
- Performance metrics

## Example Usage Scenarios

### Basic Monitoring Agent
```csharp
public class ColonyMonitorAgent : AIAgent
{
    protected override void PerformAgentActions()
    {
        // Check colony status
        var duplicants = FindObjectsOfType<MinionIdentity>();
        LogMessage($"Colony has {duplicants.Length} duplicants");
        
        // Check resources
        var food = FindObjectsOfType<Edible>();
        LogMessage($"Available food: {food.Length} items");
    }
}
```

### Automated Builder Agent
```csharp
public class BuilderAgent : AIAgent
{
    protected override void PerformAgentActions()
    {
        // Find construction tasks
        var constructionTasks = FindObjectsOfType<Constructable>();
        LogMessage($"Found {constructionTasks.Length} construction tasks");
        
        // Assign builders to tasks
        // Implementation would go here
    }
}
```

This AI agent system provides a solid foundation for creating intelligent automation in Oxygen Not Included!
