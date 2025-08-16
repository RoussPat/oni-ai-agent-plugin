using HarmonyLib;
using UnityEngine;

namespace AIAgentPlugin
{
    public class MinimalSafePatches
    {
        // Only patch Game.OnPrefabInit which is safe and confirmed to exist
        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class Game_OnPrefabInit_Patch
        {
            public static void Postfix()
            {
                try
                {
                    Debug.Log("[AI Agent Plugin] Game.OnPrefabInit called - initializing simple floating UI");
                    SimpleFloatingUI.Initialize();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[AI Agent Plugin] Error in Game.OnPrefabInit: {ex.Message}");
                }
            }
        }

        // Minimal logging patch to confirm plugin is working
        [HarmonyPatch(typeof(GameClock), "OnPrefabInit")]
        public class GameClock_OnPrefabInit_Patch
        {
            public static void Postfix()
            {
                try
                {
                    Debug.Log("[AI Agent Plugin] GameClock initialized - AI Agent is ready");
                    SimpleLogsDialog.AddLogEntry("Plugin active - Press Ctrl+Alt+A to open AI configuration");
                    SimpleLogsDialog.AddLogEntry("Hotkey: Ctrl+Alt+A opens the AI Agent configuration dialog");
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[AI Agent Plugin] Error in GameClock patch: {ex.Message}");
                }
            }
        }
        
        // Working keyboard input patch
        [HarmonyPatch(typeof(Game), "Update")]
        public class Game_Update_Patch
        {
            public static void Postfix()
            {
                try
                {
                    // Check for Ctrl+Alt+A hotkey
                    if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.A))
                    {
                        Debug.Log("[AI Agent Plugin] Hotkey Ctrl+Alt+A pressed - opening configuration");
                        SimpleLogsDialog.AddLogEntry("Hotkey pressed - opening AI Agent configuration");
                        AgentControlDialog.ShowDialog();
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[AI Agent Plugin] Error in keyboard input: {ex.Message}");
                }
            }
        }
    }
}