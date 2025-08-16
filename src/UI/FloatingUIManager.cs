using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;

namespace AIAgentPlugin
{
    public class FloatingUIManager : MonoBehaviour
    {
        private static FloatingUIManager instance;
        private static bool uiInitialized = false;
        private GameObject aiButton;
        
        public static void Initialize()
        {
            if (uiInitialized) return;
            
            try
            {
                Debug.Log("[AI Agent Plugin] Initializing UI integration...");
                
                // Create a persistent GameObject for our UI manager
                var managerObj = new GameObject("AIAgentUIManager");
                DontDestroyOnLoad(managerObj);
                
                instance = managerObj.AddComponent<FloatingUIManager>();
                instance.StartCoroutine(instance.TryIntegrateWithGameUI());
                
                uiInitialized = true;
                
                SimpleLogsDialog.AddLogEntry("UI integration initialized");
                Debug.Log("[AI Agent Plugin] UI manager created!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Failed to initialize UI: {ex.Message}");
            }
        }

        private System.Collections.IEnumerator TryIntegrateWithGameUI()
        {
            // Wait a few frames for game UI to be fully loaded
            yield return new WaitForSeconds(2f);
            
            var attempt = 0;
            const int maxAttempts = 5;
            
            while (attempt < maxAttempts)
            {
                try
                {
                    Debug.Log($"[AI Agent Plugin] UI integration attempt {attempt + 1}/{maxAttempts}");
                    
                    if (TryFindAndIntegrateWithUIContainer())
                    {
                        Debug.Log("[AI Agent Plugin] Successfully integrated with game UI!");
                        SimpleLogsDialog.AddLogEntry("AI button integrated with game UI");
                        yield break;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"[AI Agent Plugin] UI integration attempt {attempt + 1} failed: {ex.Message}");
                }
                
                attempt++;
                yield return new WaitForSeconds(3f); // Wait 3 seconds between attempts
            }
            
            // Always create overlay button as fallback
            Debug.Log("[AI Agent Plugin] UI integration failed, creating overlay button");
            CreateOverlayButton();
        }

        private bool TryFindAndIntegrateWithUIContainer()
        {
            // Try to find the game's UI canvas
            var gameCanvases = FindObjectsOfType<Canvas>();
            Canvas gameUICanvas = null;
            
            // Look for a canvas that might contain the top-level UI
            foreach (var canvas in gameCanvases)
            {
                if (canvas.name.Contains("UI") || canvas.name.Contains("Canvas") || canvas.name.Contains("Screen"))
                {
                    Debug.Log($"[AI Agent Plugin] Found potential UI canvas: {canvas.name}");
                    gameUICanvas = canvas;
                    break;
                }
            }
            
            if (gameUICanvas == null)
            {
                Debug.LogWarning("[AI Agent Plugin] Could not find suitable game UI canvas");
                return false;
            }
            
            // Look for potential button containers in the game UI
            var transforms = gameUICanvas.GetComponentsInChildren<Transform>(true);
            Transform buttonContainer = null;
            
            foreach (var transform in transforms)
            {
                var name = transform.name.ToLower();
                if (name.Contains("button") || name.Contains("control") || name.Contains("toolbar") || 
                    name.Contains("panel") || name.Contains("top") || name.Contains("left"))
                {
                    // Check if this container has buttons or similar UI elements
                    var buttons = transform.GetComponentsInChildren<Button>(true);
                    var images = transform.GetComponentsInChildren<Image>(true);
                    
                    if (buttons.Length > 0 || images.Length > 0)
                    {
                        Debug.Log($"[AI Agent Plugin] Found potential button container: {transform.name} with {buttons.Length} buttons and {images.Length} images");
                        buttonContainer = transform;
                        break;
                    }
                }
            }
            
            if (buttonContainer == null)
            {
                Debug.LogWarning("[AI Agent Plugin] Could not find suitable button container");
                return false;
            }
            
            // Create our button and add it to the found container
            return CreateIntegratedButton(buttonContainer);
        }

        private bool CreateIntegratedButton(Transform parentContainer)
        {
            try
            {
                Debug.Log($"[AI Agent Plugin] Creating integrated button in container: {parentContainer.name}");
                
                // Create the AI button
                aiButton = new GameObject("AIAgentButton");
                aiButton.transform.SetParent(parentContainer, false);
                
                // Add Image component for visual
                var image = aiButton.AddComponent<Image>();
                image.color = new Color(0.15f, 0.7f, 0.15f, 0.9f);
                
                // Add Button component
                var button = aiButton.AddComponent<Button>();
                button.targetGraphic = image;
                button.onClick.AddListener(OnButtonClick);
                
                // Add text
                var textObj = new GameObject("Text");
                textObj.transform.SetParent(aiButton.transform, false);
                var text = textObj.AddComponent<Text>();
                text.text = "AI";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 14;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                
                // Position text to fill button
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // Set up button size and layout to fit with existing UI
                var buttonRect = aiButton.GetComponent<RectTransform>();
                
                // Try to match the size of existing buttons in the container
                var existingButtons = parentContainer.GetComponentsInChildren<Button>(false);
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
                var layoutGroup = parentContainer.GetComponent<LayoutGroup>();
                if (layoutGroup != null)
                {
                    Debug.Log($"[AI Agent Plugin] Container has layout group: {layoutGroup.GetType().Name}");
                    // Layout group will handle positioning automatically
                }
                else
                {
                    // Manual positioning as last resort
                    buttonRect.anchorMin = new Vector2(0, 1);
                    buttonRect.anchorMax = new Vector2(0, 1);
                    buttonRect.anchoredPosition = new Vector2(50, -50);
                }
                
                Debug.Log("[AI Agent Plugin] Integrated button created successfully!");
                SimpleLogsDialog.AddLogEntry("AI button integrated with game UI successfully");
                return true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating integrated button: {ex.Message}");
                return false;
            }
        }

        private void CreateOverlayButton()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating overlay button...");
                
                // Create a canvas for our button
                var canvasObj = new GameObject("AIAgentCanvas");
                canvasObj.transform.SetParent(transform, false);
                
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1000; // High priority to ensure visibility
                
                var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                
                // Create the button
                aiButton = new GameObject("AIAgentOverlayButton");
                aiButton.transform.SetParent(canvasObj.transform, false);
                
                // Add Image component for visual
                var image = aiButton.AddComponent<Image>();
                image.color = new Color(0.2f, 0.8f, 0.2f, 0.9f); // More visible green
                
                // Add Button component
                var button = aiButton.AddComponent<Button>();
                button.targetGraphic = image;
                button.onClick.AddListener(OnButtonClick);
                
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
                
                // Position the button in top-left corner for maximum visibility
                var buttonRect = aiButton.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(0, 1);
                buttonRect.anchorMax = new Vector2(0, 1);
                buttonRect.anchoredPosition = new Vector2(50, -50); // Top-left corner
                buttonRect.sizeDelta = new Vector2(50, 50); // Larger and more visible
                
                Debug.Log("[AI Agent Plugin] Overlay button created successfully!");
                SimpleLogsDialog.AddLogEntry("AI button created in top-left corner");
                
                // Also create a second button in top-right as backup
                CreateBackupButton(canvasObj);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating overlay button: {ex.Message}");
            }
        }
        
        private void CreateBackupButton(GameObject canvasObj)
        {
            try
            {
                // Create a backup button in top-right corner
                var backupButton = new GameObject("AIAgentBackupButton");
                backupButton.transform.SetParent(canvasObj.transform, false);
                
                var backupImage = backupButton.AddComponent<Image>();
                backupImage.color = new Color(0.8f, 0.2f, 0.2f, 0.9f); // Red for backup
                
                var backupButtonComponent = backupButton.AddComponent<Button>();
                backupButtonComponent.targetGraphic = backupImage;
                backupButtonComponent.onClick.AddListener(OnButtonClick);
                
                // Add text
                var backupTextObj = new GameObject("Text");
                backupTextObj.transform.SetParent(backupButton.transform, false);
                var backupText = backupTextObj.AddComponent<Text>();
                backupText.text = "AI";
                backupText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                backupText.fontSize = 14;
                backupText.color = Color.white;
                backupText.alignment = TextAnchor.MiddleCenter;
                backupText.fontStyle = FontStyle.Bold;
                
                var backupTextRect = backupTextObj.GetComponent<RectTransform>();
                backupTextRect.anchorMin = Vector2.zero;
                backupTextRect.anchorMax = Vector2.one;
                backupTextRect.offsetMin = Vector2.zero;
                backupTextRect.offsetMax = Vector2.zero;
                
                // Position in top-right
                var backupRect = backupButton.GetComponent<RectTransform>();
                backupRect.anchorMin = new Vector2(1, 1);
                backupRect.anchorMax = new Vector2(1, 1);
                backupRect.anchoredPosition = new Vector2(-50, -50);
                backupRect.sizeDelta = new Vector2(40, 40);
                
                Debug.Log("[AI Agent Plugin] Backup button created in top-right corner");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating backup button: {ex.Message}");
            }
        }
        
        private void OnButtonClick()
        {
            Debug.Log("[AI Agent Plugin] AI button clicked!");
            
            // Show agent control dialog instead of config dialog
            AgentControlDialog.ShowDialog();
            SimpleLogsDialog.AddLogEntry("Agent Control dialog opened");
        }
        
        // Update method - simplified without direct Input references
        void Update()
        {
            try
            {
                // Simplified update loop - keyboard handling moved to patches
                // Keep the floating button active and responsive
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error in UI update: {ex.Message}");
            }
        }

        // Toggle button visibility
        public static void ToggleButtonVisibility()
        {
            if (instance != null && instance.aiButton != null)
            {
                bool isActive = instance.aiButton.activeSelf;
                instance.aiButton.SetActive(!isActive);
                Debug.Log($"[AI Agent Plugin] AI button visibility: {!isActive}");
            }
        }

        // Manual activation method that can be called from anywhere
        public static void ShowConfigDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Manual activation requested");
                AgentControlDialog.ShowDialog();
                SimpleLogsDialog.AddLogEntry("Agent Control opened manually");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing agent control dialog: {ex.Message}");
            }
        }
        
        // Test method for debugging - can be called manually
        public static void TestActivation()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Test activation requested");
                SimpleLogsDialog.AddLogEntry("Test activation - checking if UI system is working");
                
                // Try to show the dialog
                ShowConfigDialog();
                
                Debug.Log("[AI Agent Plugin] Test activation completed");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error in test activation: {ex.Message}");
                SimpleLogsDialog.AddLogEntry($"Test activation failed: {ex.Message}");
            }
        }
    }

    // Remove duplicate patch - handled in MinimalSafePatches now
}