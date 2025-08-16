using System;
using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace AIAgentPlugin
{
    /// <summary>
    /// Base class for AI agents that can interact with Oxygen Not Included
    /// </summary>
    public abstract class AIAgent : MonoBehaviour
    {
        [Header("Agent Configuration")]
        public string agentName = "Base Agent";
        public bool isEnabled = false;
        public bool isPaused = false;
        public bool debugMode = false;
        
        [Header("Agent State")]
        public AgentState currentState = AgentState.Idle;
        public float lastActionTime = 0f;
        public int totalActions = 0;
        
        // Events
        public event System.Action<string> OnAgentLog;
        public event System.Action<AgentState> OnStateChanged;
        public event System.Action OnAgentEnabled;
        public event System.Action OnAgentDisabled;
        
        // Coroutine for agent loop
        private Coroutine agentLoopCoroutine;
        
        protected virtual void Awake()
        {
            LogMessage($"Agent {agentName} initialized");
        }
        
        protected virtual void Start()
        {
            if (isEnabled)
            {
                StartAgent();
            }
        }
        
        protected virtual void OnDestroy()
        {
            StopAgent();
        }
        
        /// <summary>
        /// Start the agent's main loop
        /// </summary>
        public virtual void StartAgent()
        {
            if (agentLoopCoroutine != null)
            {
                StopCoroutine(agentLoopCoroutine);
            }
            
            isEnabled = true;
            SetState(AgentState.Running);
            agentLoopCoroutine = StartCoroutine(AgentLoop());
            
            LogMessage($"Agent {agentName} started");
            OnAgentEnabled?.Invoke();
        }
        
        /// <summary>
        /// Stop the agent's main loop
        /// </summary>
        public virtual void StopAgent()
        {
            if (agentLoopCoroutine != null)
            {
                StopCoroutine(agentLoopCoroutine);
                agentLoopCoroutine = null;
            }
            
            isEnabled = false;
            SetState(AgentState.Idle);
            
            LogMessage($"Agent {agentName} stopped");
            OnAgentDisabled?.Invoke();
        }
        
        /// <summary>
        /// Pause or unpause the agent
        /// </summary>
        public virtual void TogglePause()
        {
            isPaused = !isPaused;
            SetState(isPaused ? AgentState.Paused : AgentState.Running);
            
            LogMessage($"Agent {agentName} {(isPaused ? "paused" : "resumed")}");
        }
        
        /// <summary>
        /// Main agent loop - runs continuously while enabled
        /// </summary>
        protected virtual IEnumerator AgentLoop()
        {
            LogMessage($"Agent {agentName} loop started");
            
            while (isEnabled)
            {
                // Perform agent actions if not paused
                if (!isPaused)
                {
                    try
                    {
                        PerformAgentActions();
                        totalActions++;
                        lastActionTime = Time.time;
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"Error in agent loop: {ex.Message}");
                    }
                }
                
                // Wait before next iteration
                yield return new WaitForSeconds(GetActionInterval());
            }
            
            LogMessage($"Agent {agentName} loop ended");
        }
        
        /// <summary>
        /// Override this method to implement specific agent behavior
        /// </summary>
        protected abstract void PerformAgentActions();
        
        /// <summary>
        /// Get the interval between agent actions (in seconds)
        /// </summary>
        protected virtual float GetActionInterval()
        {
            return 1.0f; // Default: 1 second
        }
        
        /// <summary>
        /// Set the agent's current state
        /// </summary>
        protected virtual void SetState(AgentState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                OnStateChanged?.Invoke(newState);
                
                if (debugMode)
                {
                    LogMessage($"State changed to: {newState}");
                }
            }
        }
        
        /// <summary>
        /// Log a message from the agent
        /// </summary>
        protected virtual void LogMessage(string message)
        {
            string logMessage = $"[{agentName}] {message}";
            Debug.Log(logMessage);
            OnAgentLog?.Invoke(logMessage);
        }
        
        /// <summary>
        /// Get a summary of the agent's current status
        /// </summary>
        public virtual string GetStatusSummary()
        {
            return $"Agent: {agentName}\n" +
                   $"State: {currentState}\n" +
                   $"Enabled: {isEnabled}\n" +
                   $"Paused: {isPaused}\n" +
                   $"Total Actions: {totalActions}\n" +
                   $"Last Action: {Time.time - lastActionTime:F1}s ago";
        }
    }
    
    /// <summary>
    /// Possible states for an AI agent
    /// </summary>
    public enum AgentState
    {
        Idle,
        Running,
        Paused,
        Error,
        Thinking,
        Acting
    }
}
