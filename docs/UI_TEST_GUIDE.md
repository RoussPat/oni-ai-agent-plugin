# UI Test Guide - AI Agent Plugin

## 🎯 **What's New**

The mod now has **two UI systems**:

1. **🎮 Game UI Integration** - Tries to add button to game's toolbar
2. **🔄 Fallback Floating UI** - Creates floating button if integration fails

## 🎮 **Testing Steps**

### Step 1: Basic Test
1. **Start Oxygen Not Included**
2. **Enable the AI Agent Plugin mod**
3. **Start a new game**
4. **Wait 10-30 seconds** for UI to appear

### Step 2: Look for AI Button
**You should see ONE of these:**

#### Option A: Game UI Integration (Preferred)
- **Green "AI" button** in the game's toolbar
- **Located alongside** time controls, sandbox button, etc.
- **Top-right area** of the screen

#### Option B: Fallback Floating UI
- **Green "AI" button** floating in top-right corner
- **Larger button** (60x60 pixels)
- **Always visible** on top of game UI

### Step 3: Test Button Functionality
1. **Click the AI button** (either type)
2. **Agent Control dialog should open**
3. **Test creating agents:**
   - Click "Create Hello World Agent"
   - Click "Create GPT Agent"
   - Check that agents appear in the list

## 🔍 **Debug Information**

### Check Logs
Run this command to see what's happening:
```bash
./manage.sh logs
```

### Expected Log Messages
Look for these messages:
```
[AI Agent Plugin] Starting UI integration...
[AI Agent Plugin] UI integration attempt 1/30
[AI Agent Plugin] Found X canvases in scene
[AI Agent Plugin] Canvas: [name] (renderMode: ScreenSpaceOverlay)
[AI Agent Plugin] Found X UI elements in canvas
[AI Agent Plugin] Checking UI element: [name]
[AI Agent Plugin] Found potential container: [name] with X buttons, Y images
[AI Agent Plugin] Container position: [position], anchors: [anchors]
[AI Agent Plugin] Found potential button container: [name]
[AI Agent Plugin] Creating AI button in container: [name]
[AI Agent Plugin] AI button created successfully in game UI!
```

### Fallback Messages
If integration fails, you'll see:
```
[AI Agent Plugin] Failed to integrate with game UI, using fallback floating UI
[AI Agent Plugin] Creating fallback floating UI...
[AI Agent Plugin] Fallback floating UI created successfully!
```

## 🚨 **Troubleshooting**

### If No Button Appears
1. **Wait longer** (up to 30 seconds)
2. **Check logs**: `./manage.sh logs`
3. **Restart the game**
4. **Verify mod is enabled**

### If Button Appears But Doesn't Work
1. **Click multiple times**
2. **Check logs for click messages**
3. **Try restarting the game**

### If You See Error Messages
1. **Check the specific error** in logs
2. **Common issues:**
   - Canvas not found
   - Button container not found
   - UI element positioning issues

## 🎯 **Success Criteria**

✅ **AI button appears** (either in toolbar or floating)  
✅ **Button is clickable** and opens dialog  
✅ **Agent creation works** in dialog  
✅ **No game control interference**  

## 📋 **Expected Behavior**

1. **No keyboard interference** - All game controls work normally
2. **AI button visible** - Either in toolbar or floating in top-right
3. **Clickable button** - Opens Agent Control dialog when clicked
4. **Working dialog** - Can create and manage AI agents
5. **Robust fallback** - Floating UI if game integration fails

## 🔧 **Technical Details**

### Game UI Integration
- **Searches for game canvases** with ScreenSpaceOverlay render mode
- **Looks for UI containers** positioned at top of screen
- **Matches button size** to existing game buttons
- **Uses layout groups** for automatic positioning

### Fallback Floating UI
- **Creates own canvas** with high sorting order
- **Positioned in top-right corner** (-80, -80 from corner)
- **60x60 pixel button** with "AI" text
- **Always visible** on top of game UI

## 🎮 **Testing Checklist**

- [ ] Mod enabled in game
- [ ] Game started and loaded
- [ ] Waited 10-30 seconds
- [ ] AI button visible (toolbar or floating)
- [ ] Button is clickable
- [ ] Dialog opens when clicked
- [ ] Can create agents in dialog
- [ ] Game controls work normally
- [ ] No error messages in logs

The improved system should be much more reliable! 🚀
