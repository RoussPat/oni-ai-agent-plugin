using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace AIAgentPlugin
{
    public static class Loader
    {
        public static AssemblyName AssemblyName => Assembly.GetExecutingAssembly().GetName();
        public static Version Version => AssemblyName.Version;
        public static string Name => AssemblyName.Name;
        public static string BuildId => "d950d10a-0f43-46eb-aa9f-fd23c79a5878";
        
        public static void OnLoad()
        {
            try
            {
                Debug.Log($"[AI Agent Plugin] Mod loaded: {Name} v{Version} (Build: {BuildId})");
                
                // Initialize logs dialog first
                SimpleLogsDialog.AddLogEntry($"AI Agent Plugin initialized - Build: {BuildId.Substring(0, 8)}");
                SimpleLogsDialog.AddLogEntry("AI Agent system initialized");
                SimpleLogsDialog.AddLogEntry("Agent Manager ready for AI agents");
                SimpleLogsDialog.AddLogEntry("Keyboard shortcuts: Ctrl+Alt+A");
                
                // Initialize Agent Manager
                var agentManager = AgentManager.Instance;
                SimpleLogsDialog.AddLogEntry("Agent Manager created successfully");
                
                // Apply Harmony patches carefully
                var harmony = new Harmony("com.aiagent.oni.plugin");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                
                Debug.Log("[AI Agent Plugin] Initialization complete");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to initialize: {ex.Message}");
                Debug.LogError($"[AI Agent Plugin] Stack trace: {ex.StackTrace}");
            }
        }
    }
}