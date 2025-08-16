using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;

namespace AIAgentPlugin
{
    /// <summary>
    /// Integrates the AI button into the game's actual UI system
    /// </summary>
    public class GameUIIntegration : MonoBehaviour
    {
        private static GameUIIntegration instance;
        private GameObject aiButton;
        private bool uiIntegrated = false;
        private bool fallbackUIActive = false;
        
        public static void Initialize()
        {
            if (instance == null)
            {
                var go = new GameObject("GameUIIntegration");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<GameUIIntegration>();
                instance.StartCoroutine(instance.TryIntegrateWithGameUI());
            }
        }
        
        private System.Collections.IEnumerator TryIntegrateWithGameUI()
        {
            Debug.Log("[AI Agent Plugin] Starting UI integration...");
            
            // Wait for game to be fully loaded
            yield return new WaitForSeconds(5f);
            
            var attempts = 0;
            const int maxAttempts = 30; // Increased attempts
            
            while (attempts < maxAttempts && !uiIntegrated)
            {
                attempts++;
                Debug.Log($"[AI Agent Plugin] UI integration attempt {attempts}/{maxAttempts}");
                
                if (TryFindAndIntegrateWithGameUI())
                {
                    uiIntegrated = true;
                    Debug.Log("[AI Agent Plugin] Successfully integrated with game UI!");
                    SimpleLogsDialog.AddLogEntry("AI button integrated with game UI successfully");
                    break;
                }
                
                yield return new WaitForSeconds(1f); // Faster retry
            }
            
            if (!uiIntegrated)
            {
                Debug.LogWarning("[AI Agent Plugin] Failed to integrate with game UI, using fallback floating UI");
                SimpleLogsDialog.AddLogEntry("Using fallback floating UI system");
                CreateFallbackFloatingUI();
            }
        }
        
        private bool TryFindAndIntegrateWithGameUI()
        {
            try
            {
                // Look for the game's main UI canvas
                var canvases = FindObjectsOfType<Canvas>();
                Canvas gameCanvas = null;
                
                Debug.Log($"[AI Agent Plugin] Found {canvases.Length} canvases in scene");
                
                foreach (var canvas in canvases)
                {
                    Debug.Log($"[AI Agent Plugin] Canvas: {canvas.name} (renderMode: {canvas.renderMode})");
                    
                    // Look for the main game UI canvas - more specific targeting
                    if (canvas.renderMode == RenderMode.ScreenSpaceOverlay && 
                        (canvas.name.Contains("Game") || canvas.name.Contains("Main") || canvas.name.Contains("UI") || 
                         canvas.name.Contains("Overlay") || canvas.name.Contains("HUD")))
                    {
                        Debug.Log($"[AI Agent Plugin] Found potential game canvas: {canvas.name}");
                        gameCanvas = canvas;
                        break;
                    }
                }
                
                if (gameCanvas == null)
                {
                    Debug.LogWarning("[AI Agent Plugin] Could not find game canvas");
                    return false;
                }
                
                // Look for existing UI elements to understand the layout
                var allTransforms = gameCanvas.GetComponentsInChildren<Transform>(true);
                Debug.Log($"[AI Agent Plugin] Found {allTransforms.Length} UI elements in canvas");
                
                // Look for the top toolbar or button container - more specific targeting
                Transform buttonContainer = null;
                
                // First, try to find specific ONI UI elements
                foreach (var transform in allTransforms)
                {
                    var name = transform.name.ToLower();
                    Debug.Log($"[AI Agent Plugin] Checking UI element: {transform.name}");
                    
                    // Look for ONI-specific UI containers
                    if (name.Contains("toolbar") || name.Contains("topbar") || name.Contains("header") || 
                        name.Contains("controls") || name.Contains("buttons") || name.Contains("panel") ||
                        name.Contains("menu") || name.Contains("hud"))
                    {
                        // Check if this container has buttons or is positioned at the top
                        var buttons = transform.GetComponentsInChildren<Button>(true);
                        var images = transform.GetComponentsInChildren<Image>(true);
                        var rectTransform = transform.GetComponent<RectTransform>();
                        
                        Debug.Log($"[AI Agent Plugin] Found potential container: {transform.name} with {buttons.Length} buttons, {images.Length} images");
                        
                        // Check if this looks like a top toolbar (positioned at top of screen)
                        if (rectTransform != null)
                        {
                            var anchoredPosition = rectTransform.anchoredPosition;
                            var anchorMin = rectTransform.anchorMin;
                            var anchorMax = rectTransform.anchorMax;
                            
                            Debug.Log($"[AI Agent Plugin] Container position: {anchoredPosition}, anchors: {anchorMin} to {anchorMax}");
                            
                            // If it's positioned at the top or has buttons, it's a good candidate
                            if (anchorMax.y > 0.8f || buttons.Length > 0 || images.Length > 0)
                            {
                                Debug.Log($"[AI Agent Plugin] Found potential button container: {transform.name}");
                                buttonContainer = transform;
                                break;
                            }
                        }
                    }
                }
                
                if (buttonContainer == null)
                {
                    Debug.LogWarning("[AI Agent Plugin] Could not find button container");
                    return false;
                }
                
                // Create the AI button in the found container
                return CreateAIButtonInContainer(buttonContainer);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error in UI integration: {ex.Message}");
                return false;
            }
        }
        
        private bool CreateAIButtonInContainer(Transform container)
        {
            try
            {
                Debug.Log($"[AI Agent Plugin] Creating AI button in container: {container.name}");
                
                // Create the AI button
                aiButton = new GameObject("AIAgentButton");
                aiButton.transform.SetParent(container, false);
                
                // Add Image component for visual
                var image = aiButton.AddComponent<Image>();
                image.color = new Color(0.2f, 0.8f, 0.2f, 0.9f); // Green color
                
                // Add Button component
                var button = aiButton.AddComponent<Button>();
                button.targetGraphic = image;
                button.onClick.AddListener(OnAIButtonClick);
                
                // Add text
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(aiButton.transform, false);
                var text = textObj.AddComponent<Text>();
                text.text = "AI";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 12;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                
                // Position text to fill button
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // Set up button size and position
                var buttonRect = aiButton.GetComponent<RectTransform>();
                
                // Try to match the size of existing buttons in the container
                var existingButtons = container.GetComponentsInChildren<Button>(false);
                if (existingButtons.Length > 0)
                {
                    var exampleButton = existingButtons[0];
                    var exampleRect = exampleButton.GetComponent<RectTransform>();
                    buttonRect.sizeDelta = exampleRect.sizeDelta;
                    Debug.Log($"[AI Agent Plugin] Matched button size to existing: {exampleRect.sizeDelta}");
                }
                else
                {
                    buttonRect.sizeDelta = new Vector2(40, 40); // Default size
                }
                
                // Position the button (let layout groups handle positioning if they exist)
                var layoutGroup = container.GetComponent<LayoutGroup>();
                if (layoutGroup != null)
                {
                    Debug.Log($"[AI Agent Plugin] Container has layout group: {layoutGroup.GetType().Name}");
                    // Layout group will handle positioning automatically
                }
                else
                {
                    // Manual positioning as fallback - try to position at top-right
                    buttonRect.anchorMin = new Vector2(1, 1);
                    buttonRect.anchorMax = new Vector2(1, 1);
                    buttonRect.anchoredPosition = new Vector2(-50, -50);
                }
                
                Debug.Log("[AI Agent Plugin] AI button created successfully in game UI!");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating AI button: {ex.Message}");
                return false;
            }
        }
        
        private void CreateFallbackFloatingUI()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating fallback floating UI...");
                
                // Create a floating UI that appears in the top-right corner
                var canvas = new GameObject("AIAgentFloatingCanvas");
                var canvasComponent = canvas.AddComponent<Canvas>();
                canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasComponent.sortingOrder = 1000; // High priority
                
                var scaler = canvas.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                canvas.AddComponent<GraphicRaycaster>();
                
                // Create the AI button
                aiButton = new GameObject("AIAgentFloatingButton");
                aiButton.transform.SetParent(canvas.transform, false);
                
                // Add Image component for visual
                var image = aiButton.AddComponent<Image>();
                image.color = new Color(0.2f, 0.8f, 0.2f, 0.9f); // Green color
                
                // Add Button component
                var button = aiButton.AddComponent<Button>();
                button.targetGraphic = image;
                button.onClick.AddListener(OnAIButtonClick);
                
                // Add text
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(aiButton.transform, false);
                var text = textObj.AddComponent<Text>();
                text.text = "AI";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 16;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                
                // Position text to fill button
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // Set up button size and position
                var buttonRect = aiButton.GetComponent<RectTransform>();
                buttonRect.sizeDelta = new Vector2(60, 60);
                buttonRect.anchorMin = new Vector2(1, 1);
                buttonRect.anchorMax = new Vector2(1, 1);
                buttonRect.anchoredPosition = new Vector2(-80, -80);
                
                fallbackUIActive = true;
                Debug.Log("[AI Agent Plugin] Fallback floating UI created successfully!");
                SimpleLogsDialog.AddLogEntry("Fallback floating UI created");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating fallback UI: {ex.Message}");
            }
        }
        
        private void OnAIButtonClick()
        {
            Debug.Log("[AI Agent Plugin] AI button clicked!");
            SimpleLogsDialog.AddLogEntry("AI button clicked - opening agent control dialog");
            FloatingUIManager.ShowConfigDialog();
        }
        
        // Update method to check if button still exists
        void Update()
        {
            if (aiButton == null && (uiIntegrated || fallbackUIActive))
            {
                Debug.LogWarning("[AI Agent Plugin] AI button was destroyed, attempting to recreate...");
                uiIntegrated = false;
                fallbackUIActive = false;
                StartCoroutine(TryIntegrateWithGameUI());
            }
        }
    }
}
