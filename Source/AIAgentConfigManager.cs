using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace AIAgentPlugin
{
    [Serializable]
    public class RateLimitConfig
    {
        public bool EnableRateLimit = true;
        public int RequestsPerMinute = 10;
        public int InputTokensPerMinute = 1000;
        public int OutputTokensPerMinute = 500;
    }

    [Serializable]
    public class LocalModelConfig
    {
        public bool UseLocalModel = false;
        public string ApiEndpoint = "http://localhost:1234/v1";
        public string ModelName = "local-model";
        public bool RequiresApiKey = false;
    }

    [Serializable]
    public class AIAgentConfigData
    {
        // API Configuration
        public string ApiKey = "";
        public string SelectedModel = "claude-3-haiku-20240307";
        public LocalModelConfig LocalModel = new LocalModelConfig();
        
        // Rate Limiting
        public RateLimitConfig RateLimit = new RateLimitConfig();
        
        // General Settings
        public bool Enabled = false;
        public bool PauseOnActions = true;
        public bool DebugLogging = false;
    }

    public static class AIAgentConfigManager
    {
        private static AIAgentConfigData cachedConfig;
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Klei", "OxygenNotIncluded", "mods", "AIAgentConfig.json"
        );

        public static AIAgentConfigData GetConfig()
        {
            if (cachedConfig == null)
            {
                LoadConfig();
            }
            return cachedConfig;
        }

        public static void SaveConfig(AIAgentConfigData config)
        {
            try
            {
                cachedConfig = config;
                
                // Ensure directory exists
                var directory = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Save to file
                var json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(ConfigPath, json);
                
                Debug.Log($"[AI Agent Plugin] Config saved to: {ConfigPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to save config: {ex.Message}");
            }
        }

        private static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var json = File.ReadAllText(ConfigPath);
                    cachedConfig = JsonConvert.DeserializeObject<AIAgentConfigData>(json);
                    Debug.Log($"[AI Agent Plugin] Config loaded from: {ConfigPath}");
                }
                else
                {
                    cachedConfig = new AIAgentConfigData();
                    Debug.Log("[AI Agent Plugin] Using default config");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to load config: {ex.Message}");
                cachedConfig = new AIAgentConfigData();
            }
        }

        public static void ReloadConfig()
        {
            cachedConfig = null;
            LoadConfig();
        }
    }
}