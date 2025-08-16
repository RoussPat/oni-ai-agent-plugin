# Oxygen Not Included Console Access Guide

## How to Access the Game Console

### Method 1: Debug Mode (Recommended)
1. **Navigate to your Oxygen Not Included installation folder**
   - Usually: `C:\Program Files (x86)\Steam\steamapps\common\OxygenNotIncluded`
   - Or find it in Steam: Right-click game → Properties → Local Files → Browse Local Files
2. **Create a file named `debug_enable.txt`** in the game's root folder
3. **Start the game**
4. **Press `BACKSPACE` to open debug tools**
5. **Note: This is for debugging only, not for mod interaction**

### Method 2: Log Files (For Mod Debugging)
1. **Navigate to the game's log folder:**
   - Windows: `%USERPROFILE%\AppData\LocalLow\Klei\Oxygen Not Included\`
   - Linux: `~/.config/unity3d/Klei/Oxygen Not Included/`
2. **Look for log files** with timestamps
3. **Open the latest log file** in a text editor
4. **Search for "[AI Agent Plugin]"** to see mod messages

### Method 3: Alternative Debug Methods
1. **Some older versions may support:**
   - `Ctrl + Shift + C` (if available)
   - `F1` or `F12` (if available)
2. **Check game version and documentation**
3. **Note: Most recent versions only support debug_enable.txt method**

## What to Look For

### Plugin Loading Messages
Look for these messages in the console:
```
[AI Agent Plugin] Mod loaded: AIAgentPlugin v1.0.0 (Build: d950d10a-0f43-46eb-aa9f-fd23c79a5878)
[AI Agent Plugin] GameClock initialized - AI Agent is ready
[AI Agent Plugin] Testing UI system...
```

### Error Messages
Look for these error patterns:
```
[AI Agent Plugin] Error in keyboard input: ...
[AI Agent Plugin] Error showing agent control dialog: ...
[AI Agent Plugin] Failed to initialize UI: ...
```

### Debug Commands
Once debug tools are open (BACKSPACE), you can:
- Access various debug menus
- View game state information
- Note: This is for game debugging, not mod interaction

## Common Issues

### Debug Tools Not Opening
- Make sure `debug_enable.txt` file exists in game root folder
- Try pressing `BACKSPACE` key
- Check if file is in correct location

### No Log Messages
- Verify the mod is enabled in mods menu
- Check if mod files are in correct location
- Restart the game after enabling mod

### Permission Issues
- Run Steam as administrator
- Check file permissions in mod directory
- Verify mod files are not read-only

## Quick Test
1. Create `debug_enable.txt` in game root folder
2. Start game
3. Press `BACKSPACE` to open debug tools
4. Check log files for `[AI Agent Plugin]` messages
5. If no messages appear, the mod isn't loading properly
