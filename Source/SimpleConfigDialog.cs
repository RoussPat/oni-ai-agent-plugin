using System;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    public class SimpleConfigDialog : KModalScreen
    {
        public static SimpleConfigDialog Instance;
        
        // Configuration fields (simplified)
        private Toggle enabledToggle;
        private Toggle pauseToggle;
        private Toggle debugToggle;
        private Text statusText;
        private Button apiConfigButton;
        private Button logsButton;
        private Button rateLimitButton;
        private Button saveButton;
        private Button cancelButton;

        public static void ShowDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] ShowDialog called - simple version");
                
                if (Instance == null)
                {
                    Debug.Log("[AI Agent Plugin] Instance is null, creating dialog");
                    CreateDialog();
                }
                
                if (Instance != null)
                {
                    Debug.Log("[AI Agent Plugin] Activating dialog");
                    Instance.Activate();
                    Instance.LoadConfiguration();
                    Debug.Log("[AI Agent Plugin] Dialog activated successfully");
                }
                else
                {
                    Debug.LogError("[AI Agent Plugin] Instance is still null after CreateDialog");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error in ShowDialog: {ex.Message}");
                Debug.LogError($"[AI Agent Plugin] Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private static void CreateDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating simple config dialog...");
                
                // Create dialog GameObject
                GameObject dialogObj = new GameObject("AIAgentConfigDialog");
                dialogObj.SetActive(false);
                
                // Add the dialog component
                Instance = dialogObj.AddComponent<SimpleConfigDialog>();
                
                // Set up the UI using known-working approach
                Instance.SetupSimpleUI();
                
                // Find appropriate parent for dialog
                Transform parentTransform = null;
                
                // Try to find game UI root first (for in-game)
                var uiRoot = GameObject.Find("UI Root");
                if (uiRoot != null)
                {
                    Debug.Log("[AI Agent Plugin] Using UI Root as parent");
                    parentTransform = uiRoot.transform;
                }
                else if (FrontEndManager.Instance != null)
                {
                    Debug.Log("[AI Agent Plugin] Using FrontEndManager as parent");
                    parentTransform = FrontEndManager.Instance.transform;
                }
                else
                {
                    Debug.Log("[AI Agent Plugin] Using DontDestroyOnLoad as parent");
                    DontDestroyOnLoad(dialogObj);
                }
                
                if (parentTransform != null)
                {
                    dialogObj.transform.SetParent(parentTransform, false);
                }
                
                Debug.Log("[AI Agent Plugin] Simple config dialog created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating simple dialog: {ex.Message}");
                throw;
            }
        }

        private void SetupSimpleUI()
        {
            Debug.Log("[AI Agent Plugin] Setting up simple UI components...");
            
            // Create background panel
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // Reasonable sorting order
            canvas.worldCamera = null;

            // Add GraphicRaycaster for mouse interaction
            var graphicRaycaster = gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            graphicRaycaster.ignoreReversedGraphics = true;
            graphicRaycaster.blockingObjects = UnityEngine.UI.GraphicRaycaster.BlockingObjects.None;

            var canvasScaler = gameObject.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);

            // Background
            var background = new GameObject("Background");
            background.transform.SetParent(transform, false);
            var bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.8f);
            bgImage.raycastTarget = true;
            var bgRect = background.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            
            // Add button to background for click-to-close functionality
            var bgButton = background.AddComponent<Button>();
            bgButton.onClick.AddListener(() => {
                Debug.Log("[AI Agent Plugin] Background clicked - closing dialog");
                Deactivate();
            });

            // Main panel
            var mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(background.transform, false);
            var mainPanelImage = mainPanel.AddComponent<Image>();
            mainPanelImage.color = new Color(0.13f, 0.13f, 0.13f, 1f); // ONI dark gray
            mainPanelImage.raycastTarget = true;
            var mainRect = mainPanel.GetComponent<RectTransform>();
            mainRect.sizeDelta = new Vector2(500, 400); // Much smaller now
            mainRect.anchoredPosition = Vector2.zero;

            var layoutGroup = mainPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(25, 25, 25, 25);
            layoutGroup.spacing = 20;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;

            // Title
            CreateLabel(mainPanel, "AI Agent Control", 20, FontStyle.Bold, TextAnchor.MiddleCenter);

            // Build ID caption - VERY visible to confirm version
            var buildIdText = CreateLabel(mainPanel, $"Build: {Loader.BuildId.Substring(0, 8)} | Main Control Panel", 12, FontStyle.Italic, TextAnchor.MiddleCenter);
            buildIdText.color = new Color(0.7f, 0.9f, 0.7f, 1f); // Light green

            // Options section with better spacing
            CreateLabel(mainPanel, "Agent Options:", 16, FontStyle.Bold);
            enabledToggle = CreateToggle(mainPanel, "Enable AI Agent");
            pauseToggle = CreateToggle(mainPanel, "Pause game on AI actions");
            debugToggle = CreateToggle(mainPanel, "Enable debug logging");

            // Status
            statusText = CreateLabel(mainPanel, "Status: Disconnected", 14);
            statusText.color = Color.yellow;

            // Buttons
            var buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(mainPanel.transform, false);
            var buttonLayout = buttonPanel.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 8; // Reduced spacing for more buttons
            buttonLayout.childControlWidth = true;
            
            var buttonRect = buttonPanel.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 40);

            apiConfigButton = CreateButton(buttonPanel, "API Config", OnApiConfig);
            rateLimitButton = CreateButton(buttonPanel, "Rate Limits", OnRateLimit);
            logsButton = CreateButton(buttonPanel, "View Logs", OnViewLogs);
            saveButton = CreateButton(buttonPanel, "Save", OnSave);
            cancelButton = CreateButton(buttonPanel, "Cancel", OnCancel);
        }

        private Text CreateLabel(GameObject parent, string text, int fontSize, FontStyle fontStyle = FontStyle.Normal, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(parent.transform, false);
            var label = labelObj.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = fontSize;
            label.fontStyle = fontStyle;
            label.color = Color.white;
            label.alignment = alignment;
            
            var rect = labelObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, fontSize + 8);
            
            var layoutElement = labelObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = fontSize + 8;
            layoutElement.preferredHeight = fontSize + 8;
            
            return label;
        }

        private InputField CreateInputField(GameObject parent, string placeholder)
        {
            var inputObj = new GameObject("InputField");
            inputObj.transform.SetParent(parent.transform, false);
            
            var inputImage = inputObj.AddComponent<Image>();
            inputImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var inputField = inputObj.AddComponent<InputField>();
            inputField.transition = Selectable.Transition.ColorTint;
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            var text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            
            // Position text properly
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8, 0);
            textRect.offsetMax = new Vector2(-8, 0);
            
            var placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(inputObj.transform, false);
            var placeholderText = placeholderObj.AddComponent<Text>();
            placeholderText.text = placeholder;
            placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            placeholderText.fontSize = 14;
            placeholderText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            placeholderText.alignment = TextAnchor.MiddleLeft;
            
            // Position placeholder properly
            var placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(8, 0);
            placeholderRect.offsetMax = new Vector2(-8, 0);
            
            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
            inputField.targetGraphic = inputImage;
            
            var rect = inputObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 32);
            
            var layoutElement = inputObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 32;
            layoutElement.preferredHeight = 32;
            
            return inputField;
        }

        private Toggle CreateToggle(GameObject parent, string text)
        {
            var toggleObj = new GameObject("Toggle");
            toggleObj.transform.SetParent(parent.transform, false);
            
            var toggle = toggleObj.AddComponent<Toggle>();
            
            // Background
            var backgroundObj = new GameObject("Background");
            backgroundObj.transform.SetParent(toggleObj.transform, false);
            var bgImage = backgroundObj.AddComponent<Image>();
            bgImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            var backgroundRect = backgroundObj.GetComponent<RectTransform>();
            backgroundRect.sizeDelta = new Vector2(20, 20);
            backgroundRect.anchorMin = new Vector2(0, 0.5f);
            backgroundRect.anchorMax = new Vector2(0, 0.5f);
            backgroundRect.anchoredPosition = new Vector2(10, 0);
            
            // Checkmark
            var checkmarkObj = new GameObject("Checkmark");
            checkmarkObj.transform.SetParent(backgroundObj.transform, false);
            var checkmark = checkmarkObj.AddComponent<Image>();
            checkmark.color = Color.green;
            
            var checkmarkRect = checkmarkObj.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.offsetMin = new Vector2(2, 2);
            checkmarkRect.offsetMax = new Vector2(-2, -2);
            
            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(toggleObj.transform, false);
            var label = labelObj.AddComponent<Text>();
            label.text = text;
            label.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            label.fontSize = 15; // Slightly larger
            label.color = new Color(0.95f, 0.95f, 0.95f, 1f); // Very bright white
            label.alignment = TextAnchor.MiddleLeft;
            
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(35, 0); // 35px from left edge (after checkbox)
            labelRect.offsetMax = new Vector2(-10, 0); // 10px from right edge
            
            toggle.graphic = checkmark;
            toggle.targetGraphic = bgImage;
            
            var rect = toggleObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 32);
            
            var layoutElement = toggleObj.AddComponent<LayoutElement>();
            layoutElement.minHeight = 32;
            layoutElement.preferredHeight = 32;
            
            return toggle;
        }

        private Button CreateButton(GameObject parent, string text, UnityEngine.Events.UnityAction action)
        {
            var buttonObj = new GameObject("Button");
            buttonObj.transform.SetParent(parent.transform, false);
            
            var buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.5f, 0.8f, 1f); // ONI-style blue
            
            var button = buttonObj.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(action);
            
            var textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            var buttonText = textObj.AddComponent<Text>();
            buttonText.text = text;
            buttonText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buttonText.fontSize = 14;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;
            
            var textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            return button;
        }

        private void LoadConfiguration()
        {
            try
            {
                var config = AIAgentConfigManager.GetConfig();
                
                // Options only (API config moved to separate dialog)
                if (enabledToggle != null)
                    enabledToggle.isOn = config.Enabled;
                if (pauseToggle != null)
                    pauseToggle.isOn = config.PauseOnActions;
                if (debugToggle != null)
                    debugToggle.isOn = config.DebugLogging;
                
                UpdateStatus();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error loading configuration: {ex.Message}");
            }
        }

        private void OnSave()
        {
            try
            {
                var config = AIAgentConfigManager.GetConfig(); // Get existing config to preserve other settings
                
                // Save only the options (API config handled by separate dialog)
                config.Enabled = enabledToggle?.isOn ?? false;
                config.PauseOnActions = pauseToggle?.isOn ?? false;
                config.DebugLogging = debugToggle?.isOn ?? false;
                
                AIAgentConfigManager.SaveConfig(config);
                UpdateStatus();
                
                Debug.Log("[AI Agent Plugin] Configuration saved from main dialog");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error saving configuration: {ex.Message}");
            }
        }

        private void OnCancel()
        {
            Deactivate();
        }

        private void OnViewLogs()
        {
            SimpleLogsDialog.ShowDialog();
        }

        private void OnApiConfig()
        {
            ApiConfigDialog.ShowDialog();
        }

        private void OnRateLimit()
        {
            RateLimitDialog.ShowDialog();
        }

        private void UpdateStatus()
        {
            if (statusText == null) return;
            
            var config = AIAgentConfigManager.GetConfig();
            
            if (config.LocalModel.UseLocalModel)
            {
                if (config.Enabled)
                {
                    statusText.text = $"Status: Local model enabled ({config.LocalModel.ModelName})";
                    statusText.color = Color.cyan;
                }
                else
                {
                    statusText.text = "Status: Local model configured but agent disabled";
                    statusText.color = Color.yellow;
                }
            }
            else if (string.IsNullOrEmpty(config.ApiKey))
            {
                statusText.text = "Status: No API key configured";
                statusText.color = Color.red;
            }
            else if (config.Enabled)
            {
                statusText.text = $"Status: Agent enabled ({config.SelectedModel})";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "Status: Agent disabled";
                statusText.color = Color.yellow;
            }
        }
    }
}