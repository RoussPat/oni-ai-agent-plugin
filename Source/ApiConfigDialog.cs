using System;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    public class ApiConfigDialog : KModalScreen
    {
        public static ApiConfigDialog Instance;
        
        // API Configuration fields
        private InputField apiKeyInput;
        private InputField modelInput;
        private Toggle useLocalModelToggle;
        private InputField localEndpointInput;
        private InputField localModelNameInput;
        private Button saveButton;
        private Button cancelButton;

        public static void ShowDialog()
        {
            try
            {
                if (Instance == null)
                {
                    CreateDialog();
                }
                
                if (Instance != null)
                {
                    Instance.Activate();
                    Instance.LoadConfiguration();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing API config dialog: {ex.Message}");
            }
        }

        private static void CreateDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating API config dialog...");
                
                GameObject dialogObj = new GameObject("AIAgentApiConfigDialog");
                dialogObj.SetActive(false);
                
                Instance = dialogObj.AddComponent<ApiConfigDialog>();
                Instance.SetupApiConfigUI();
                
                // Find appropriate parent for dialog
                var uiRoot = GameObject.Find("UI Root");
                if (uiRoot != null)
                {
                    dialogObj.transform.SetParent(uiRoot.transform, false);
                }
                else
                {
                    DontDestroyOnLoad(dialogObj);
                }
                
                Debug.Log("[AI Agent Plugin] API config dialog created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating API config dialog: {ex.Message}");
                throw;
            }
        }

        private void SetupApiConfigUI()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 103; // Higher than other dialogs
            
            var graphicRaycaster = gameObject.AddComponent<UnityEngine.UI.GraphicRaycaster>();
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
            
            var bgButton = background.AddComponent<Button>();
            bgButton.onClick.AddListener(() => Deactivate());

            // Main panel
            var mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(background.transform, false);
            var mainPanelImage = mainPanel.AddComponent<Image>();
            mainPanelImage.color = new Color(0.13f, 0.13f, 0.13f, 1f);
            mainPanelImage.raycastTarget = true;
            var mainRect = mainPanel.GetComponent<RectTransform>();
            mainRect.sizeDelta = new Vector2(600, 500);
            mainRect.anchoredPosition = Vector2.zero;

            var layoutGroup = mainPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(25, 25, 25, 25);
            layoutGroup.spacing = 15;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;

            // Title
            CreateLabel(mainPanel, "API Configuration", 20, FontStyle.Bold, TextAnchor.MiddleCenter);
            
            // Build ID
            var buildIdText = CreateLabel(mainPanel, $"Build: {Loader.BuildId.Substring(0, 8)} | API Settings", 12, FontStyle.Italic, TextAnchor.MiddleCenter);
            buildIdText.color = new Color(0.9f, 0.9f, 0.7f, 1f); // Light yellow

            // Cloud API Configuration
            CreateLabel(mainPanel, "Cloud API Configuration:", 16, FontStyle.Bold);
            
            CreateLabel(mainPanel, "API Key:", 14);
            apiKeyInput = CreateInputField(mainPanel, "Enter API key...");

            CreateLabel(mainPanel, "Model (e.g. claude-3-haiku-20240307, gpt-4o):", 14);
            modelInput = CreateInputField(mainPanel, "claude-3-haiku-20240307");

            // Local Model Configuration
            CreateLabel(mainPanel, "Local Model Configuration:", 16, FontStyle.Bold);
            useLocalModelToggle = CreateToggle(mainPanel, "Use local model instead of cloud API");
            
            CreateLabel(mainPanel, "Local API Endpoint:", 14);
            localEndpointInput = CreateInputField(mainPanel, "http://localhost:1234/v1");
            
            CreateLabel(mainPanel, "Local Model Name:", 14);
            localModelNameInput = CreateInputField(mainPanel, "local-model");

            // Buttons
            var buttonPanel = new GameObject("ButtonPanel");
            buttonPanel.transform.SetParent(mainPanel.transform, false);
            var buttonLayout = buttonPanel.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 15;
            buttonLayout.childControlWidth = true;
            
            var buttonRect = buttonPanel.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 40);

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
            label.fontSize = 14;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleLeft;
            
            var labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(35, 0);
            labelRect.offsetMax = new Vector2(-10, 0);
            
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
            buttonImage.color = new Color(0.3f, 0.5f, 0.8f, 1f);
            
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
                
                // API Configuration
                if (apiKeyInput != null)
                    apiKeyInput.text = config.ApiKey;
                if (modelInput != null)
                    modelInput.text = config.SelectedModel;
                
                // Local Model Configuration
                if (useLocalModelToggle != null)
                    useLocalModelToggle.isOn = config.LocalModel.UseLocalModel;
                if (localEndpointInput != null)
                    localEndpointInput.text = config.LocalModel.ApiEndpoint;
                if (localModelNameInput != null)
                    localModelNameInput.text = config.LocalModel.ModelName;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error loading API configuration: {ex.Message}");
            }
        }

        private void OnSave()
        {
            try
            {
                var config = AIAgentConfigManager.GetConfig(); // Get existing config to preserve other settings
                
                // API Configuration
                config.ApiKey = apiKeyInput?.text ?? "";
                config.SelectedModel = modelInput?.text ?? "claude-3-haiku-20240307";
                
                // Local Model Configuration
                config.LocalModel.UseLocalModel = useLocalModelToggle?.isOn ?? false;
                config.LocalModel.ApiEndpoint = localEndpointInput?.text ?? "http://localhost:1234/v1";
                config.LocalModel.ModelName = localModelNameInput?.text ?? "local-model";
                
                AIAgentConfigManager.SaveConfig(config);
                
                Debug.Log("[AI Agent Plugin] API configuration saved");
                Deactivate();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error saving API configuration: {ex.Message}");
            }
        }

        private void OnCancel()
        {
            Deactivate();
        }
    }
}