using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace AIAgentPlugin
{
    /// <summary>
    /// Manages AI agents in the game
    /// </summary>
    public class AgentManager : MonoBehaviour
    {
        private static AgentManager instance;
        private Dictionary<string, AIAgent> activeAgents = new Dictionary<string, AIAgent>();
        private GameObject agentContainer;
        
        // Events
        public event System.Action<string> OnAgentCreated;
        public event System.Action<string> OnAgentDestroyed;
        public event System.Action<string, AgentState> OnAgentStateChanged;
        
        public static AgentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateInstance();
                }
                return instance;
            }
        }
        
        private static void CreateInstance()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating AgentManager instance...");
                
                var managerObj = new GameObject("AIAgentManager");
                DontDestroyOnLoad(managerObj);
                
                instance = managerObj.AddComponent<AgentManager>();
                instance.Initialize();
                
                Debug.Log("[AI Agent Plugin] AgentManager created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to create AgentManager: {ex.Message}");
            }
        }
        
        private void Initialize()
        {
            // Create a container for all agents
            agentContainer = new GameObject("AgentContainer");
            agentContainer.transform.SetParent(transform, false);
            
            Debug.Log("[AI Agent Plugin] AgentManager initialized");
            SimpleLogsDialog.AddLogEntry("AI Agent Manager initialized");
        }
        
        /// <summary>
        /// Create a new Hello World agent
        /// </summary>
        public HelloWorldAgent CreateHelloWorldAgent()
        {
            try
            {
                var agentObj = new GameObject("HelloWorldAgent");
                agentObj.transform.SetParent(agentContainer.transform, false);
                
                var agent = agentObj.AddComponent<HelloWorldAgent>();
                
                // Subscribe to agent events
                agent.OnAgentLog += (message) => Debug.Log($"[AgentManager] {message}");
                agent.OnStateChanged += (state) => OnAgentStateChanged?.Invoke(agent.agentName, state);
                agent.OnAgentEnabled += () => Debug.Log($"[AgentManager] Agent {agent.agentName} enabled");
                agent.OnAgentDisabled += () => Debug.Log($"[AgentManager] Agent {agent.agentName} disabled");
                
                // Add to active agents
                activeAgents[agent.agentName] = agent;
                
                OnAgentCreated?.Invoke(agent.agentName);
                SimpleLogsDialog.AddLogEntry($"Created Hello World Agent: {agent.agentName}");
                
                Debug.Log($"[AI Agent Plugin] Hello World Agent created: {agent.agentName}");
                return agent;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to create Hello World Agent: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Create a new GPT agent
        /// </summary>
        public GPTAgent CreateGPTAgent()
        {
            try
            {
                var agentObj = new GameObject("GPTAgent");
                agentObj.transform.SetParent(agentContainer.transform, false);
                
                var agent = agentObj.AddComponent<GPTAgent>();
                
                // Subscribe to agent events
                agent.OnAgentLog += (message) => Debug.Log($"[AgentManager] {message}");
                agent.OnStateChanged += (state) => OnAgentStateChanged?.Invoke(agent.agentName, state);
                agent.OnAgentEnabled += () => Debug.Log($"[AgentManager] Agent {agent.agentName} enabled");
                agent.OnAgentDisabled += () => Debug.Log($"[AgentManager] Agent {agent.agentName} disabled");
                
                // Add to active agents
                activeAgents[agent.agentName] = agent;
                
                OnAgentCreated?.Invoke(agent.agentName);
                SimpleLogsDialog.AddLogEntry($"Created GPT Agent: {agent.agentName}");
                
                Debug.Log($"[AI Agent Plugin] GPT Agent created: {agent.agentName}");
                return agent;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to create GPT Agent: {ex.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Get an agent by name
        /// </summary>
        public AIAgent GetAgent(string agentName)
        {
            activeAgents.TryGetValue(agentName, out AIAgent agent);
            return agent;
        }
        
        /// <summary>
        /// Get all active agents
        /// </summary>
        public List<AIAgent> GetAllAgents()
        {
            return new List<AIAgent>(activeAgents.Values);
        }
        
        /// <summary>
        /// Start all agents
        /// </summary>
        public void StartAllAgents()
        {
            foreach (var agent in activeAgents.Values)
            {
                if (!agent.isEnabled)
                {
                    agent.StartAgent();
                }
            }
            
            SimpleLogsDialog.AddLogEntry($"Started {activeAgents.Count} agents");
        }
        
        /// <summary>
        /// Stop all agents
        /// </summary>
        public void StopAllAgents()
        {
            foreach (var agent in activeAgents.Values)
            {
                if (agent.isEnabled)
                {
                    agent.StopAgent();
                }
            }
            
            SimpleLogsDialog.AddLogEntry($"Stopped {activeAgents.Count} agents");
        }
        
        /// <summary>
        /// Pause all agents
        /// </summary>
        public void PauseAllAgents()
        {
            foreach (var agent in activeAgents.Values)
            {
                if (agent.isEnabled && !agent.isPaused)
                {
                    agent.TogglePause();
                }
            }
            
            SimpleLogsDialog.AddLogEntry($"Paused {activeAgents.Count} agents");
        }
        
        /// <summary>
        /// Resume all agents
        /// </summary>
        public void ResumeAllAgents()
        {
            foreach (var agent in activeAgents.Values)
            {
                if (agent.isEnabled && agent.isPaused)
                {
                    agent.TogglePause();
                }
            }
            
            SimpleLogsDialog.AddLogEntry($"Resumed {activeAgents.Count} agents");
        }
        
        /// <summary>
        /// Destroy an agent
        /// </summary>
        public void DestroyAgent(string agentName)
        {
            if (activeAgents.TryGetValue(agentName, out AIAgent agent))
            {
                activeAgents.Remove(agentName);
                OnAgentDestroyed?.Invoke(agentName);
                
                if (agent != null)
                {
                    Destroy(agent.gameObject);
                }
                
                SimpleLogsDialog.AddLogEntry($"Destroyed agent: {agentName}");
            }
        }
        
        /// <summary>
        /// Destroy all agents
        /// </summary>
        public void DestroyAllAgents()
        {
            var agentNames = new List<string>(activeAgents.Keys);
            
            foreach (var agentName in agentNames)
            {
                DestroyAgent(agentName);
            }
            
            SimpleLogsDialog.AddLogEntry("All agents destroyed");
        }
        
        /// <summary>
        /// Get a summary of all agents
        /// </summary>
        public string GetAgentsSummary()
        {
            if (activeAgents.Count == 0)
            {
                return "No active agents";
            }
            
            var summary = $"Active Agents ({activeAgents.Count}):\n";
            
            foreach (var agent in activeAgents.Values)
            {
                summary += $"\n{agent.GetStatusSummary()}\n";
            }
            
            return summary;
        }
        
        /// <summary>
        /// Get the number of active agents
        /// </summary>
        public int GetAgentCount()
        {
            return activeAgents.Count;
        }
        
        /// <summary>
        /// Check if any agents are running
        /// </summary>
        public bool HasRunningAgents()
        {
            foreach (var agent in activeAgents.Values)
            {
                if (agent.isEnabled && !agent.isPaused)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
