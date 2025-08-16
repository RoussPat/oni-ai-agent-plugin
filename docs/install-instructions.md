# Manual Installation Instructions

If the automatic deployment fails (usually because ONI is running), follow these steps:

## 1. Close Oxygen Not Included
Make sure the game is completely closed before copying files.

## 2. Copy Files Manually

**From:** `/home/pat/projects/oni-ai-agent-plugin/bin/Release/net471/`
**To:** `/mnt/c/Users/patri/Documents/Klei/OxygenNotIncluded/mods/Dev/AIAgentPlugin/`

Copy these files:
- `AIAgentPlugin.dll`
- `mod_info.yaml` (from project root)
- `mod.yaml` (from project root)

## 3. Launch ONI and Test

1. Start Oxygen Not Included
2. Look for the **"AI Agent"** button in the main menu (should appear with New Game, Load Game, etc.)
3. If you don't see the button, try pressing **Ctrl+F10** during gameplay as a backup
4. Check the debug console for messages like: `[AI Agent Plugin] Mod loaded`

## Troubleshooting

### No "AI Agent" button in main menu
- Check ONI debug console (F12) for error messages
- Verify all files were copied correctly
- Try the keyboard shortcut Ctrl+F10 instead

### Mod not loading
- Check file paths are correct
- Ensure you're using the Dev folder, not local
- Verify mod files have proper permissions

### Still having issues?
Run: `./check-install.sh` to verify file installation