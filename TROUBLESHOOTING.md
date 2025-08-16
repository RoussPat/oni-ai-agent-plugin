# AI Agent Plugin Troubleshooting Guide

## Quick Fix Checklist

### 1. Enable Console Access
**First, enable the game console to see debug messages:**

1. **Right-click Oxygen Not Included in Steam**
2. **Properties → Launch Options**
3. **Add: `-console`**
4. **Start game and press `~` to open console**

### 2. Check Mod Installation
Run this command to verify installation:
```bash
./manage.sh test
```

### 3. Verify Mod is Enabled
- Start Oxygen Not Included
- Go to **Mods** menu
- Enable **"AI Agent Plugin"**
- Restart the game

## UI Button Issues

### Problem: No AI button visible
**Solutions:**

1. **Wait 10-15 seconds** after starting a game
   - The button should appear in top-left corner (green) or top-right corner (red)

2. **Check console for UI messages:**
   ```
   [AI Agent Plugin] UI integration attempt 1/5
   [AI Agent Plugin] Overlay button created successfully!
   [AI Agent Plugin] AI button created in top-left corner
   ```

3. **If no UI messages appear:**
   - The mod isn't loading properly
   - Check console for error messages
   - Verify mod files are in correct location

### Problem: Button appears but doesn't work
**Solutions:**
- Click the button multiple times
- Check console for error messages when clicking
- Try both the green (top-left) and red (top-right) buttons

## Keyboard Shortcut Issues

### Problem: Ctrl+Alt+A doesn't work
**Solutions:**

1. **Try different key combinations:**
   - Left Ctrl + Left Alt + A
   - Right Ctrl + Right Alt + A
   - Left Ctrl + Right Alt + A
   - Right Ctrl + Left Alt + A

2. **Check console for keyboard messages:**
   ```
   [AI Agent Plugin] Key check - Ctrl: True, Alt: True, A: True
   [AI Agent Plugin] Hotkey Ctrl+Alt+A pressed - opening agent control dialog
   ```

3. **If no keyboard messages appear:**
   - Input system isn't being patched
   - Try running game in windowed mode
   - Check if other mods conflict

4. **Alternative activation:**
   - Use the AI button instead
   - Wait for automatic test dialog (opens after 5 seconds)

## Console Debugging

### How to Access Console
1. **Add `-console` to Steam launch options**
2. **Start game**
3. **Press `~` (tilde key) to open console**
4. **Look for `[AI Agent Plugin]` messages**

### Important Console Messages

**✅ Good messages (mod is working):**
```
[AI Agent Plugin] Mod loaded: AIAgentPlugin v1.0.0 (Build: ...)
[AI Agent Plugin] GameClock initialized - AI Agent is ready
[AI Agent Plugin] Testing UI system...
[AI Agent Plugin] Overlay button created successfully!
[AI Agent Plugin] Hotkey Ctrl+Alt+A pressed - opening agent control dialog
```

**❌ Error messages (mod has issues):**
```
[AI Agent Plugin] Error in keyboard input: ...
[AI Agent Plugin] Error showing agent control dialog: ...
[AI Agent Plugin] Failed to initialize UI: ...
[AI Agent Plugin] Error in GameClock patch: ...
```

### No Console Messages at All
**The mod isn't loading. Check:**
1. Mod is enabled in mods menu
2. Mod files are in correct location
3. No compilation errors
4. Game is restarted after enabling mod

## File Location Issues

### Windows
```
%USERPROFILE%\Documents\Klei\OxygenNotIncluded\mods\Dev\AIAgentPlugin\
```

### Linux
```
~/.config/unity3d/Klei/Oxygen Not Included/mods/Dev/AIAgentPlugin/
```

### Required Files
- `AIAgentPlugin.dll`
- `mod_info.yaml`
- `mod.yaml`

## Common Error Solutions

### "Error in keyboard input"
- Input system conflict
- Try different key combinations
- Run in windowed mode

### "Error showing agent control dialog"
- UI system issue
- Check if game UI is fully loaded
- Wait longer before trying

### "Failed to initialize UI"
- Game UI not found
- Wait longer for game to load
- Try restarting the game

### "Error in GameClock patch"
- Mod loading issue
- Check mod installation
- Verify mod is enabled

## Testing Steps

### Step 1: Basic Mod Loading
1. Enable console (`-console` in launch options)
2. Start game and press `~`
3. Look for: `[AI Agent Plugin] Mod loaded: ...`
4. If not found: Mod not loading properly

### Step 2: UI System Test
1. Start a new game
2. Wait 5-10 seconds
3. Look for automatic test dialog
4. If dialog appears: UI system works
5. If not: Check console for UI errors

### Step 3: Keyboard Input Test
1. With console open, press Ctrl+Alt+A
2. Look for: `[AI Agent Plugin] Hotkey Ctrl+Alt+A pressed`
3. If not found: Input system issue
4. If found but no dialog: UI system issue

### Step 4: Button Test
1. Look for AI button in top-left (green) or top-right (red)
2. Click the button
3. Should open Agent Control dialog
4. If not: Check console for button click errors

## Advanced Debugging

### Enable Detailed Logging
Add to launch options:
```
-console -debug -logFile debug.log
```

### Check Log Files
Look in game directory for:
- `Player.log`
- `output_log.txt`
- `debug.log`

### Force UI Test
The mod automatically tests the UI system after 5 seconds. If this doesn't work, the entire UI system has issues.

## Still Not Working?

1. **Check console for specific error messages**
2. **Verify mod installation with `./manage.sh test`**
3. **Try clean install:**
   ```bash
   ./manage.sh clean
   ./manage.sh build
   ./manage.sh install
   ```
4. **Check if other mods conflict**
5. **Try running game in windowed mode**
6. **Verify game version compatibility**

## Getting Help

If you're still having issues:
1. **Enable console and note all `[AI Agent Plugin]` messages**
2. **Check which step fails in the testing process**
3. **Note any error messages in console**
4. **Try the troubleshooting steps above**
