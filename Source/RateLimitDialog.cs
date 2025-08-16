using System;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    public class RateLimitDialog : KModalScreen
    {
        public static RateLimitDialog Instance;
        
        // UI Components
        private Toggle enableRateLimitToggle;
        private InputField requestsPerMinuteInput;
        private InputField inputTokensPerMinuteInput;
        private InputField outputTokensPerMinuteInput;
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
                Debug.LogError($"[AI Agent Plugin] Error showing rate limit dialog: {ex.Message}");
            }
        }

        private static void CreateDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating rate limit dialog...");
                
                GameObject dialogObj = new GameObject("AIAgentRateLimitDialog");
                dialogObj.SetActive(false);
                
                Instance = dialogObj.AddComponent<RateLimitDialog>();
                Instance.SetupRateLimitUI();
                
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
                
                Debug.Log("[AI Agent Plugin] Rate limit dialog created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating rate limit dialog: {ex.Message}");
                throw;
            }
        }

        private void SetupRateLimitUI()
        {
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 102; // Higher than other dialogs
            
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
            mainRect.sizeDelta = new Vector2(500, 400);
            mainRect.anchoredPosition = Vector2.zero;

            var layoutGroup = mainPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(25, 25, 25, 25);
            layoutGroup.spacing = 20;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;

            // Title
            CreateLabel(mainPanel, "Rate Limiting Configuration", 18, FontStyle.Bold, TextAnchor.MiddleCenter);
            
            // Build ID
            var buildIdText = CreateLabel(mainPanel, $"Build: {Loader.BuildId.Substring(0, 8)} | Rate Limit Settings", 11, FontStyle.Italic, TextAnchor.MiddleCenter);
            buildIdText.color = new Color(0.9f, 0.7f, 0.7f, 1f); // Light red

            // Enable rate limiting
            enableRateLimitToggle = CreateToggle(mainPanel, "Enable rate limiting");

            // Rate limiting fields
            CreateLabel(mainPanel, "Requests per minute:", 14);
            requestsPerMinuteInput = CreateInputField(mainPanel, "10");
            requestsPerMinuteInput.contentType = InputField.ContentType.IntegerNumber;

            CreateLabel(mainPanel, "Input tokens per minute:", 14);
            inputTokensPerMinuteInput = CreateInputField(mainPanel, "1000");
            inputTokensPerMinuteInput.contentType = InputField.ContentType.IntegerNumber;

            CreateLabel(mainPanel, "Output tokens per minute:", 14);
            outputTokensPerMinuteInput = CreateInputField(mainPanel, "500");
            outputTokensPerMinuteInput.contentType = InputField.ContentType.IntegerNumber;

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
                
                if (enableRateLimitToggle != null)
                    enableRateLimitToggle.isOn = config.RateLimit.EnableRateLimit;
                if (requestsPerMinuteInput != null)
                    requestsPerMinuteInput.text = config.RateLimit.RequestsPerMinute.ToString();
                if (inputTokensPerMinuteInput != null)
                    inputTokensPerMinuteInput.text = config.RateLimit.InputTokensPerMinute.ToString();
                if (outputTokensPerMinuteInput != null)
                    outputTokensPerMinuteInput.text = config.RateLimit.OutputTokensPerMinute.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error loading rate limit configuration: {ex.Message}");
            }
        }

        private void OnSave()
        {
            try
            {
                var config = AIAgentConfigManager.GetConfig();
                
                config.RateLimit.EnableRateLimit = enableRateLimitToggle?.isOn ?? true;
                config.RateLimit.RequestsPerMinute = int.TryParse(requestsPerMinuteInput?.text, out int requests) ? requests : 10;
                config.RateLimit.InputTokensPerMinute = int.TryParse(inputTokensPerMinuteInput?.text, out int inputTokens) ? inputTokens : 1000;
                config.RateLimit.OutputTokensPerMinute = int.TryParse(outputTokensPerMinuteInput?.text, out int outputTokens) ? outputTokens : 500;
                
                AIAgentConfigManager.SaveConfig(config);
                
                Debug.Log("[AI Agent Plugin] Rate limit configuration saved");
                Deactivate();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error saving rate limit configuration: {ex.Message}");
            }
        }

        private void OnCancel()
        {
            Deactivate();
        }
    }
}