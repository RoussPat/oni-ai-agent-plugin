using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace AIAgentPlugin
{
    /// <summary>
    /// AI Agent that uses local GPT-OSS-20B model via Ollama API
    /// </summary>
    public class GPTAgent : AIAgent
    {
        [Header("GPT Agent Settings")]
        public string modelName = "gpt-oss:20b";
        public string ollamaUrl = "http://localhost:11434";
        public int maxTokens = 150;
        public float temperature = 0.7f;
        public bool enableGameContext = true;
        
        [Header("Game Context")]
        public string gameContext = "You are an AI assistant helping to manage an Oxygen Not Included colony. You can analyze the game state and provide suggestions.";
        
        private Queue<string> messageQueue = new Queue<string>();
        private bool isProcessingRequest = false;
        private float lastContextUpdate = 0f;
        private float contextUpdateInterval = 30f; // Update context every 30 seconds
        
        protected override void Awake()
        {
            agentName = "GPT-OSS-20B Agent";
            base.Awake();
        }
        
        protected override void Start()
        {
            base.Start();
            LogMessage("GPT Agent initialized with local GPT-OSS-20B model");
            LogMessage($"Ollama URL: {ollamaUrl}");
            LogMessage($"Model: {modelName}");
        }
        
        /// <summary>
        /// Perform the agent's main actions - analyze game state and get AI responses
        /// </summary>
        protected override void PerformAgentActions()
        {
            // Update game context periodically
            if (Time.time - lastContextUpdate >= contextUpdateInterval)
            {
                UpdateGameContext();
                lastContextUpdate = Time.time;
            }
            
            // Process any queued messages
            if (messageQueue.Count > 0 && !isProcessingRequest)
            {
                ProcessNextMessage();
            }
            
            // Occasionally ask the AI for insights
            if (UnityEngine.Random.Range(0f, 1f) < 0.1f && !isProcessingRequest) // 10% chance
            {
                AskAIForInsights();
            }
        }
        
        /// <summary>
        /// Update the game context with current state information
        /// </summary>
        private void UpdateGameContext()
        {
            try
            {
                var contextBuilder = new StringBuilder();
                contextBuilder.AppendLine(gameContext);
                contextBuilder.AppendLine();
                contextBuilder.AppendLine("Current Game State:");
                
                // Get basic game information
                contextBuilder.AppendLine($"- Game Running: {(Game.Instance != null ? "Yes" : "No")}");
                
                // Get duplicant information
                var duplicants = FindObjectsOfType<MinionIdentity>();
                contextBuilder.AppendLine($"- Duplicants: {duplicants.Length}");
                
                // Get basic resource information
                var food = FindObjectsOfType<Edible>();
                contextBuilder.AppendLine($"- Food Items: {food.Length}");
                
                // Get construction information
                var construction = FindObjectsOfType<Constructable>();
                contextBuilder.AppendLine($"- Construction Tasks: {construction.Length}");
                
                gameContext = contextBuilder.ToString();
                
                if (debugMode)
                {
                    LogMessage("Updated game context");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating game context: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Ask the AI for insights about the current game state
        /// </summary>
        private void AskAIForInsights()
        {
            var prompt = "Based on the current game state, provide a brief insight or suggestion for managing the colony. Keep it concise and practical.";
            SendMessageToAI(prompt);
        }
        
        /// <summary>
        /// Send a message to the AI and get a response
        /// </summary>
        public void SendMessageToAI(string message)
        {
            if (isProcessingRequest)
            {
                messageQueue.Enqueue(message);
                LogMessage("Message queued (AI is busy)");
                return;
            }
            
            StartCoroutine(SendOllamaRequest(message));
        }
        
        /// <summary>
        /// Process the next message in the queue
        /// </summary>
        private void ProcessNextMessage()
        {
            if (messageQueue.Count > 0)
            {
                var message = messageQueue.Dequeue();
                StartCoroutine(SendOllamaRequest(message));
            }
        }
        
        /// <summary>
        /// Send a request to the Ollama API
        /// </summary>
        private IEnumerator SendOllamaRequest(string message)
        {
            isProcessingRequest = true;
            SetState(AgentState.Thinking);
            
            LogMessage($"Sending request to AI: {message}");
            
            // For now, simulate AI response since UnityWebRequest is not available
            // In a real implementation, you would use UnityWebRequest or System.Net.Http
            yield return new WaitForSeconds(1f); // Simulate network delay
            
            try
            {
                // Simulate AI response for testing
                var simulatedResponse = $"This is a simulated response from {modelName} to: {message}";
                LogMessage($"AI Response: {simulatedResponse}");
                SimpleLogsDialog.AddLogEntry($"AI: {simulatedResponse}");
                
                // TODO: Implement actual HTTP request when UnityWebRequest is available
                LogMessage("Note: Using simulated response. Real HTTP requests not yet implemented.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error in AI request: {ex.Message}");
            }
            
            isProcessingRequest = false;
            SetState(AgentState.Running);
            
            // Process next message if available
            if (messageQueue.Count > 0)
            {
                ProcessNextMessage();
            }
        }
        
        /// <summary>
        /// Get the interval between agent actions
        /// </summary>
        protected override float GetActionInterval()
        {
            return 5.0f; // Check every 5 seconds
        }
        
        /// <summary>
        /// Override status summary to include AI-specific information
        /// </summary>
        public override string GetStatusSummary()
        {
            return base.GetStatusSummary() + 
                   $"\nModel: {modelName}" +
                   $"\nQueue: {messageQueue.Count} messages" +
                   $"\nProcessing: {isProcessingRequest}" +
                   $"\nContext Updates: {Time.time - lastContextUpdate:F1}s ago";
        }
        
        /// <summary>
        /// Send a custom message to the AI
        /// </summary>
        public void SendCustomMessage(string message)
        {
            SendMessageToAI(message);
        }
        
        /// <summary>
        /// Ask the AI about a specific aspect of the game
        /// </summary>
        public void AskAboutGame(string topic)
        {
            var prompt = $"Tell me about {topic} in the context of Oxygen Not Included colony management.";
            SendMessageToAI(prompt);
        }
    }
    
    /// <summary>
    /// Ollama API request structure
    /// </summary>
    [System.Serializable]
    public class OllamaRequest
    {
        public string model;
        public string prompt;
        public bool stream;
        public OllamaOptions options;
    }
    
    /// <summary>
    /// Ollama API options
    /// </summary>
    [System.Serializable]
    public class OllamaOptions
    {
        public float temperature;
        public int num_predict;
    }
    
    /// <summary>
    /// Ollama API response structure
    /// </summary>
    [System.Serializable]
    public class OllamaResponse
    {
        public string model;
        public string response;
        public long created_at;
        public bool done;
    }
}
