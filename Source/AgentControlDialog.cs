using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    /// <summary>
    /// Dialog for controlling AI agents
    /// </summary>
    public class AgentControlDialog : MonoBehaviour
    {
        public static AgentControlDialog Instance;
        
        // UI Components
        private Text titleText;
        private Text statusText;
        private Button createHelloWorldButton;
        private Button createGPTButton;
        private Button startAllButton;
        private Button stopAllButton;
        private Button pauseAllButton;
        private Button resumeAllButton;
        private Button destroyAllButton;
        private Button refreshButton;
        private Button closeButton;
        
        // Agent list
        private Transform agentListContainer;
        private List<GameObject> agentListItems = new List<GameObject>();
        
        // Update timer
        private float lastUpdateTime = 0f;
        private float updateInterval = 2f; // Update every 2 seconds
        
        // Canvas and UI management
        private Canvas dialogCanvas;
        private bool isActive = false;
        
        public static void ShowDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Showing Agent Control Dialog");
                
                if (Instance == null)
                {
                    CreateDialog();
                }
                
                if (Instance != null)
                {
                    Debug.Log("[AI Agent Plugin] About to activate dialog...");
                    Instance.Activate();
                    Instance.RefreshAgentList();
                    Debug.Log("[AI Agent Plugin] Agent Control Dialog activation complete");
                }
                else
                {
                    Debug.LogError("[AI Agent Plugin] Instance is null after creation!");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing agent control dialog: {ex.Message}");
            }
        }
        
        private static void CreateDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating Agent Control Dialog...");
                
                // Create dialog GameObject
                GameObject dialogObj = new GameObject("AgentControlDialog");
                
                // Add the dialog component
                Instance = dialogObj.AddComponent<AgentControlDialog>();
                
                // Set up the canvas
                Instance.SetupCanvas();
                
                // Set up the UI
                Instance.SetupUI();
                
                // Make sure it's not destroyed on scene changes
                DontDestroyOnLoad(dialogObj);
                
                Debug.Log("[AI Agent Plugin] Agent Control Dialog created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating agent control dialog: {ex.Message}");
            }
        }
        
        private void SetupCanvas()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Setting up dialog canvas...");
                
                // Create canvas
                var canvasObj = new GameObject("DialogCanvas");
                canvasObj.transform.SetParent(transform, false);
                
                dialogCanvas = canvasObj.AddComponent<Canvas>();
                dialogCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                dialogCanvas.sortingOrder = 1000; // High priority to be on top
                
                // Add canvas scaler
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                // Add graphic raycaster for input
                canvasObj.AddComponent<GraphicRaycaster>();
                
                Debug.Log($"[AI Agent Plugin] Dialog canvas setup complete - canvas: {dialogCanvas.name}, sortingOrder: {dialogCanvas.sortingOrder}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error setting up canvas: {ex.Message}");
            }
        }
        
        private void SetupUI()
        {
            try
            {
                // Create main panel
                var panel = new GameObject("Panel");
                panel.transform.SetParent(dialogCanvas.transform, false);
                
                var panelImage = panel.AddComponent<Image>();
                panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
                
                var panelRect = panel.GetComponent<RectTransform>();
                panelRect.anchorMin = new Vector2(0.1f, 0.1f);
                panelRect.anchorMax = new Vector2(0.9f, 0.9f);
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;
                
                // Add layout
                var layout = panel.AddComponent<VerticalLayoutGroup>();
                layout.spacing = 10;
                layout.padding = new RectOffset(20, 20, 20, 20);
                layout.childControlHeight = false;
                layout.childControlWidth = true;
                
                // Title
                var titleObj = new GameObject("Title");
                titleObj.transform.SetParent(panel.transform, false);
                titleText = titleObj.AddComponent<Text>();
                titleText.text = "AI Agent Control Panel";
                titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                titleText.fontSize = 18;
                titleText.color = Color.white;
                titleText.alignment = TextAnchor.MiddleCenter;
                titleText.fontStyle = FontStyle.Bold;
                
                var titleRect = titleObj.GetComponent<RectTransform>();
                titleRect.sizeDelta = new Vector2(0, 30);
                
                // Status text
                var statusObj = new GameObject("Status");
                statusObj.transform.SetParent(panel.transform, false);
                statusText = statusObj.AddComponent<Text>();
                statusText.text = "Ready";
                statusText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                statusText.fontSize = 14;
                statusText.color = Color.green;
                statusText.alignment = TextAnchor.MiddleLeft;
                
                var statusRect = statusObj.GetComponent<RectTransform>();
                statusRect.sizeDelta = new Vector2(0, 20);
                
                // Control buttons container
                var controlsContainer = new GameObject("Controls");
                controlsContainer.transform.SetParent(panel.transform, false);
                
                var controlsLayout = controlsContainer.AddComponent<HorizontalLayoutGroup>();
                controlsLayout.spacing = 10;
                controlsLayout.childControlHeight = false;
                controlsLayout.childControlWidth = true;
                
                var controlsRect = controlsContainer.GetComponent<RectTransform>();
                controlsRect.sizeDelta = new Vector2(0, 40);
                
                // Create buttons
                createHelloWorldButton = CreateButton(controlsContainer, "Create Hello World Agent", OnCreateHelloWorldAgent);
                createGPTButton = CreateButton(controlsContainer, "Create GPT Agent", OnCreateGPTAgent);
                startAllButton = CreateButton(controlsContainer, "Start All", OnStartAllAgents);
                stopAllButton = CreateButton(controlsContainer, "Stop All", OnStopAllAgents);
                pauseAllButton = CreateButton(controlsContainer, "Pause All", OnPauseAllAgents);
                resumeAllButton = CreateButton(controlsContainer, "Resume All", OnResumeAllAgents);
                destroyAllButton = CreateButton(controlsContainer, "Destroy All", OnDestroyAllAgents);
                refreshButton = CreateButton(controlsContainer, "Refresh", OnRefresh);
                
                // Agent list container
                var listContainer = new GameObject("AgentList");
                listContainer.transform.SetParent(panel.transform, false);
                
                var listLayout = listContainer.AddComponent<VerticalLayoutGroup>();
                listLayout.spacing = 5;
                listLayout.childControlHeight = false;
                listLayout.childControlWidth = true;
                
                agentListContainer = listContainer.transform;
                
                var listRect = listContainer.GetComponent<RectTransform>();
                listRect.sizeDelta = new Vector2(0, 200);
                
                // Close button
                var closeContainer = new GameObject("CloseContainer");
                closeContainer.transform.SetParent(panel.transform, false);
                
                var closeLayout = closeContainer.AddComponent<HorizontalLayoutGroup>();
                closeLayout.childAlignment = TextAnchor.MiddleRight;
                closeLayout.childControlHeight = false;
                closeLayout.childControlWidth = false;
                
                var closeRect = closeContainer.GetComponent<RectTransform>();
                closeRect.sizeDelta = new Vector2(0, 40);
                
                closeButton = CreateButton(closeContainer, "Close", OnClose);
                
                Debug.Log("[AI Agent Plugin] Agent Control Dialog UI setup complete");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error setting up agent control dialog UI: {ex.Message}");
            }
        }
        
        private Button CreateButton(GameObject parent, string text, UnityEngine.Events.UnityAction onClick)
        {
            var buttonObj = new GameObject($"Button_{text.Replace(" ", "")}");
            buttonObj.transform.SetParent(parent.transform, false);
            
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(onClick);
            
            var buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            var buttonText = buttonTextObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 12;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            var buttonTextRect = buttonTextObj.GetComponent<RectTransform>();
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;
            
            var buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(120, 30);
            
            return button;
        }
        
        public void Activate()
        {
            try
            {
                Debug.Log($"[AI Agent Plugin] Activate called - dialogCanvas: {(dialogCanvas != null ? "exists" : "null")}");
                
                if (dialogCanvas != null)
                {
                    dialogCanvas.enabled = true;
                    gameObject.SetActive(true);
                    isActive = true;
                    Debug.Log("[AI Agent Plugin] Dialog activated and made visible");
                    
                    // Force the canvas to update
                    Canvas.ForceUpdateCanvases();
                    Debug.Log("[AI Agent Plugin] Canvas update forced");
                }
                else
                {
                    Debug.LogError("[AI Agent Plugin] dialogCanvas is null - cannot activate dialog");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error activating dialog: {ex.Message}");
            }
        }
        
        public void Deactivate()
        {
            try
            {
                if (dialogCanvas != null)
                {
                    dialogCanvas.enabled = false;
                    gameObject.SetActive(false);
                    isActive = false;
                    Debug.Log("[AI Agent Plugin] Dialog deactivated and hidden");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error deactivating dialog: {ex.Message}");
            }
        }
        
        private void Update()
        {
            // Handle ESC key to close dialog
            if (isActive && Input.GetKeyDown(KeyCode.Escape))
            {
                OnClose();
                return;
            }
            
            // Update status periodically
            if (isActive && Time.time - lastUpdateTime >= updateInterval)
            {
                UpdateStatus();
                lastUpdateTime = Time.time;
            }
        }
        
        private void UpdateStatus()
        {
            try
            {
                var manager = AgentManager.Instance;
                var agentCount = manager.GetAgentCount();
                var hasRunning = manager.HasRunningAgents();
                
                statusText.text = $"Agents: {agentCount} | Running: {(hasRunning ? "Yes" : "No")}";
                statusText.color = hasRunning ? Color.green : Color.yellow;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error updating status: {ex.Message}");
            }
        }
        
        private void RefreshAgentList()
        {
            try
            {
                // Clear existing list items
                foreach (var item in agentListItems)
                {
                    if (item != null)
                    {
                        Destroy(item);
                    }
                }
                agentListItems.Clear();
                
                // Get all agents
                var manager = AgentManager.Instance;
                var agents = manager.GetAllAgents();
                
                if (agents.Count == 0)
                {
                    var noAgentsObj = new GameObject("NoAgents");
                    noAgentsObj.transform.SetParent(agentListContainer, false);
                    
                    var noAgentsText = noAgentsObj.AddComponent<Text>();
                    noAgentsText.text = "No agents created yet";
                    noAgentsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    noAgentsText.fontSize = 14;
                    noAgentsText.color = Color.gray;
                    noAgentsText.alignment = TextAnchor.MiddleCenter;
                    
                    var noAgentsRect = noAgentsObj.GetComponent<RectTransform>();
                    noAgentsRect.sizeDelta = new Vector2(0, 30);
                    
                    agentListItems.Add(noAgentsObj);
                }
                else
                {
                    foreach (var agent in agents)
                    {
                        CreateAgentListItem(agent);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error refreshing agent list: {ex.Message}");
            }
        }
        
        private void CreateAgentListItem(AIAgent agent)
        {
            try
            {
                var itemObj = new GameObject($"AgentItem_{agent.agentName}");
                itemObj.transform.SetParent(agentListContainer, false);
                
                var itemImage = itemObj.AddComponent<Image>();
                itemImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
                
                var itemLayout = itemObj.AddComponent<HorizontalLayoutGroup>();
                itemLayout.spacing = 10;
                itemLayout.childControlHeight = false;
                itemLayout.childControlWidth = true;
                
                var itemRect = itemObj.GetComponent<RectTransform>();
                itemRect.sizeDelta = new Vector2(0, 40);
                
                // Agent info
                var infoObj = new GameObject("Info");
                infoObj.transform.SetParent(itemObj.transform, false);
                
                var infoText = infoObj.AddComponent<Text>();
                infoText.text = $"{agent.agentName} - {agent.currentState}";
                infoText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                infoText.fontSize = 12;
                infoText.color = Color.white;
                infoText.alignment = TextAnchor.MiddleLeft;
                
                var infoRect = infoObj.GetComponent<RectTransform>();
                infoRect.sizeDelta = new Vector2(0, 30);
                
                // Control buttons
                var startStopButton = CreateButton(itemObj, agent.isEnabled ? "Stop" : "Start", () => OnToggleAgent(agent));
                var pauseButton = CreateButton(itemObj, agent.isPaused ? "Resume" : "Pause", () => OnTogglePause(agent));
                var destroyButton = CreateButton(itemObj, "Destroy", () => OnDestroyAgent(agent));
                
                agentListItems.Add(itemObj);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating agent list item: {ex.Message}");
            }
        }
        
        // Button event handlers
        private void OnCreateHelloWorldAgent()
        {
            try
            {
                var manager = AgentManager.Instance;
                var agent = manager.CreateHelloWorldAgent();
                
                if (agent != null)
                {
                    agent.StartAgent();
                    RefreshAgentList();
                    SimpleLogsDialog.AddLogEntry("Created and started Hello World Agent");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating Hello World agent: {ex.Message}");
            }
        }
        
        private void OnCreateGPTAgent()
        {
            try
            {
                var manager = AgentManager.Instance;
                var agent = manager.CreateGPTAgent();
                
                if (agent != null)
                {
                    agent.StartAgent();
                    RefreshAgentList();
                    SimpleLogsDialog.AddLogEntry("Created and started GPT Agent");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating GPT agent: {ex.Message}");
            }
        }
        
        private void OnStartAllAgents()
        {
            try
            {
                AgentManager.Instance.StartAllAgents();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error starting all agents: {ex.Message}");
            }
        }
        
        private void OnStopAllAgents()
        {
            try
            {
                AgentManager.Instance.StopAllAgents();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error stopping all agents: {ex.Message}");
            }
        }
        
        private void OnPauseAllAgents()
        {
            try
            {
                AgentManager.Instance.PauseAllAgents();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error pausing all agents: {ex.Message}");
            }
        }
        
        private void OnResumeAllAgents()
        {
            try
            {
                AgentManager.Instance.ResumeAllAgents();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error resuming all agents: {ex.Message}");
            }
        }
        
        private void OnDestroyAllAgents()
        {
            try
            {
                AgentManager.Instance.DestroyAllAgents();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error destroying all agents: {ex.Message}");
            }
        }
        
        private void OnRefresh()
        {
            RefreshAgentList();
        }
        
        private void OnToggleAgent(AIAgent agent)
        {
            try
            {
                if (agent.isEnabled)
                {
                    agent.StopAgent();
                }
                else
                {
                    agent.StartAgent();
                }
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error toggling agent: {ex.Message}");
            }
        }
        
        private void OnTogglePause(AIAgent agent)
        {
            try
            {
                agent.TogglePause();
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error toggling pause: {ex.Message}");
            }
        }
        
        private void OnDestroyAgent(AIAgent agent)
        {
            try
            {
                AgentManager.Instance.DestroyAgent(agent.agentName);
                RefreshAgentList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error destroying agent: {ex.Message}");
            }
        }
        
        private void OnClose()
        {
            Deactivate();
        }
    }
}
