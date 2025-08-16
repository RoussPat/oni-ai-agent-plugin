using System;
using UnityEngine;

namespace AIAgentPlugin
{
    /// <summary>
    /// A simple "Hello World" agent that demonstrates basic AI agent functionality
    /// This agent just logs periodic messages to show the system is working
    /// </summary>
    public class HelloWorldAgent : AIAgent
    {
        [Header("Hello World Agent Settings")]
        public string[] greetingMessages = {
            "Hello, Oxygen Not Included!",
            "I am an AI agent exploring your world",
            "The colony looks interesting today",
            "I'm learning about your base",
            "This is a test of AI agent integration",
            "Greetings from the AI system!",
            "I'm here to help manage your colony",
            "The AI agent is active and monitoring"
        };
        
        private int messageIndex = 0;
        private float lastGreetingTime = 0f;
        private float greetingInterval = 10f; // Send a greeting every 10 seconds
        
        protected override void Awake()
        {
            agentName = "Hello World Agent";
            base.Awake();
        }
        
        protected override void Start()
        {
            base.Start();
            LogMessage("Hello World Agent ready to greet the world!");
        }
        
        /// <summary>
        /// Perform the agent's main actions - in this case, just send periodic greetings
        /// </summary>
        protected override void PerformAgentActions()
        {
            // Send a greeting message periodically
            if (Time.time - lastGreetingTime >= greetingInterval)
            {
                SendGreeting();
                lastGreetingTime = Time.time;
            }
            
            // Occasionally change state to show different behaviors
            if (UnityEngine.Random.Range(0f, 1f) < 0.1f) // 10% chance
            {
                SetState(AgentState.Thinking);
                LogMessage("Thinking about the colony...");
                
                // Return to running state after a short delay
                StartCoroutine(ReturnToRunningState());
            }
        }
        
        /// <summary>
        /// Send a greeting message
        /// </summary>
        private void SendGreeting()
        {
            string message = greetingMessages[messageIndex % greetingMessages.Length];
            LogMessage($"Greeting #{messageIndex + 1}: {message}");
            messageIndex++;
            
            // Also log to the game's log system
            SimpleLogsDialog.AddLogEntry($"AI Agent: {message}");
        }
        
        /// <summary>
        /// Return to running state after thinking
        /// </summary>
        private System.Collections.IEnumerator ReturnToRunningState()
        {
            yield return new WaitForSeconds(2f);
            SetState(AgentState.Running);
        }
        
        /// <summary>
        /// Get the interval between agent actions
        /// </summary>
        protected override float GetActionInterval()
        {
            return 2.0f; // Check every 2 seconds
        }
        
        /// <summary>
        /// Override status summary to include agent-specific information
        /// </summary>
        public override string GetStatusSummary()
        {
            return base.GetStatusSummary() + 
                   $"\nGreetings Sent: {messageIndex}" +
                   $"\nNext Greeting: {greetingInterval - (Time.time - lastGreetingTime):F1}s";
        }
        
        /// <summary>
        /// Custom method to change greeting interval
        /// </summary>
        public void SetGreetingInterval(float interval)
        {
            greetingInterval = Mathf.Max(1f, interval); // Minimum 1 second
            LogMessage($"Greeting interval changed to {greetingInterval} seconds");
        }
        
        /// <summary>
        /// Add a custom greeting message
        /// </summary>
        public void AddGreeting(string message)
        {
            Array.Resize(ref greetingMessages, greetingMessages.Length + 1);
            greetingMessages[greetingMessages.Length - 1] = message;
            LogMessage($"Added new greeting: {message}");
        }
    }
}
