using HarmonyLib;
using UnityEngine;

namespace AIAgentPlugin
{
    /// <summary>
    /// Safe patches that don't interfere with keyboard input
    /// </summary>
    public class NoKeyboardPatches
    {
        // Only patch Game.OnPrefabInit which is safe and confirmed to exist
        [HarmonyPatch(typeof(Game), "OnPrefabInit")]
        public class Game_OnPrefabInit_Patch
        {
            public static void Postfix()
            {
                try
                {
                    Debug.Log("[AI Agent Plugin] Game.OnPrefabInit called - initializing floating UI");
                    FloatingUIManager.Initialize();
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
                    SimpleLogsDialog.AddLogEntry("Plugin active - Use AI buttons to open agent controls");
                    SimpleLogsDialog.AddLogEntry("No keyboard shortcuts - UI buttons only for safety");
                    
                    // Test the UI system after a short delay
                    var testObj = new GameObject("UITest");
                    testObj.AddComponent<UITestComponent>();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[AI Agent Plugin] Error in GameClock patch: {ex.Message}");
                }
            }
        }
        
        // Test component to verify UI system is working
        public class UITestComponent : MonoBehaviour
        {
            private float testDelay = 5f; // Wait 5 seconds before testing
            
            void Start()
            {
                StartCoroutine(TestUISystem());
            }
            
            private System.Collections.IEnumerator TestUISystem()
            {
                yield return new WaitForSeconds(testDelay);
                
                Debug.Log("[AI Agent Plugin] Testing UI system...");
                SimpleLogsDialog.AddLogEntry("Testing UI system - if you see this, the plugin is working");
                
                // Test the UI activation
                FloatingUIManager.TestActivation();
                
                // Destroy this test component
                Destroy(gameObject);
            }
        }
    }
}
