using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    public class SimpleLogsDialog : KModalScreen
    {
        public static SimpleLogsDialog Instance;
        private static List<string> logEntries = new List<string>();
        
        private Text logsText;
        private ScrollRect scrollRect;
        private Button clearButton;
        private Button refreshButton;
        private Button closeButton;

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
                    Instance.RefreshLogs(); // Refresh after activation to avoid coroutine issues
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing simple logs dialog: {ex.Message}");
            }
        }

        public static void AddLogEntry(string entry)
        {
            var timestamp = System.DateTime.Now.ToString("HH:mm:ss");
            logEntries.Add($"[{timestamp}] {entry}");
            
            // Keep only last 100 entries
            if (logEntries.Count > 100)
            {
                logEntries.RemoveAt(0);
            }

            // Refresh UI if dialog is open
            if (Instance != null && Instance.gameObject.activeInHierarchy)
            {
                Instance.RefreshLogs();
            }
        }

        private static void CreateDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating simple logs dialog...");
                
                GameObject dialogObj = new GameObject("AIAgentLogsDialog");
                dialogObj.SetActive(false);
                
                Instance = dialogObj.AddComponent<SimpleLogsDialog>();
                Instance.SetupSimpleLogsUI();
                
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
                
                Debug.Log("[AI Agent Plugin] Simple logs dialog created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating simple logs dialog: {ex.Message}");
                throw;
            }
        }

        private void SetupSimpleLogsUI()
        {
            Debug.Log("[AI Agent Plugin] Setting up simple logs UI...");
            
            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 101; // Higher than config dialog
            canvas.worldCamera = null;
            
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
            
            var bgButton = background.AddComponent<Button>();
            bgButton.onClick.AddListener(() => Deactivate());

            // Main panel
            var mainPanel = new GameObject("MainPanel");
            mainPanel.transform.SetParent(background.transform, false);
            var mainPanelImage = mainPanel.AddComponent<Image>();
            mainPanelImage.color = new Color(0.13f, 0.13f, 0.13f, 1f);
            mainPanelImage.raycastTarget = true;
            var mainRect = mainPanel.GetComponent<RectTransform>();
            mainRect.sizeDelta = new Vector2(800, 600);
            mainRect.anchoredPosition = Vector2.zero;

            var layoutGroup = mainPanel.AddComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.spacing = 15;
            layoutGroup.childControlHeight = false;
            layoutGroup.childControlWidth = true;

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(mainPanel.transform, false);
            var titleText = titleObj.AddComponent<Text>();
            titleText.text = "AI Agent Logs";
            titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            titleText.fontSize = 20;
            titleText.fontStyle = FontStyle.Bold;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(0, 30);

            // Build ID caption
            var buildIdObj = new GameObject("BuildId");
            buildIdObj.transform.SetParent(mainPanel.transform, false);
            var buildIdText = buildIdObj.AddComponent<Text>();
            buildIdText.text = $"Build: {Loader.BuildId.Substring(0, 8)} | Simple Logs Dialog";
            buildIdText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            buildIdText.fontSize = 12;
            buildIdText.fontStyle = FontStyle.Italic;
            buildIdText.color = new Color(0.7f, 0.7f, 0.9f, 1f); // Light blue
            buildIdText.alignment = TextAnchor.MiddleCenter;
            
            var buildIdRect = buildIdObj.GetComponent<RectTransform>();
            buildIdRect.sizeDelta = new Vector2(0, 18);

            // Scroll view for logs
            var scrollView = new GameObject("ScrollView");
            scrollView.transform.SetParent(mainPanel.transform, false);
            scrollRect = scrollView.AddComponent<ScrollRect>();
            
            var scrollImage = scrollView.AddComponent<Image>();
            scrollImage.color = new Color(0.05f, 0.05f, 0.05f, 1f);
            
            var scrollLayout = scrollView.AddComponent<LayoutElement>();
            scrollLayout.flexibleHeight = 1f;
            scrollLayout.minHeight = 400;
            scrollLayout.preferredHeight = 450;

            // Content area
            var content = new GameObject("Content");
            content.transform.SetParent(scrollView.transform, false);
            var contentRect = content.GetComponent<RectTransform>();
            
            var contentFitter = content.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            var contentLayout = content.AddComponent<VerticalLayoutGroup>();
            contentLayout.childControlHeight = false;
            contentLayout.childControlWidth = true;
            contentLayout.childForceExpandWidth = true;

            // Logs text
            var logsObj = new GameObject("LogsText");
            logsObj.transform.SetParent(content.transform, false);
            logsText = logsObj.AddComponent<Text>();
            logsText.text = "No logs available";
            logsText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            logsText.fontSize = 11;
            logsText.color = Color.white;
            logsText.alignment = TextAnchor.UpperLeft;
            
            // Make text fill the full width
            var logsRect = logsObj.GetComponent<RectTransform>();
            logsRect.anchorMin = new Vector2(0, 0);
            logsRect.anchorMax = new Vector2(1, 1);
            logsRect.offsetMin = new Vector2(5, 0);
            logsRect.offsetMax = new Vector2(-5, 0);
            
            var logsLayout = logsObj.AddComponent<LayoutElement>();
            logsLayout.flexibleHeight = 1f;
            logsLayout.flexibleWidth = 1f;
            logsLayout.minHeight = 20;

            // Configure ScrollRect
            scrollRect.content = contentRect;
            scrollRect.viewport = scrollView.GetComponent<RectTransform>();
            scrollRect.verticalScrollbar = null;
            scrollRect.horizontalScrollbar = null;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;

            // Buttons
            var buttonContainer = new GameObject("ButtonContainer");
            buttonContainer.transform.SetParent(mainPanel.transform, false);
            var buttonLayout = buttonContainer.AddComponent<HorizontalLayoutGroup>();
            buttonLayout.spacing = 15;
            buttonLayout.childControlWidth = true;
            
            var buttonRect = buttonContainer.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, 40);

            clearButton = CreateButton(buttonContainer, "Clear", OnClear);
            refreshButton = CreateButton(buttonContainer, "Refresh", OnRefresh);
            closeButton = CreateButton(buttonContainer, "Close", OnClose);
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

        private void RefreshLogs()
        {
            try
            {
                if (logsText != null)
                {
                    if (logEntries.Count == 0)
                    {
                        logsText.text = "No logs available";
                    }
                    else
                    {
                        logsText.text = string.Join("\n", logEntries.ToArray());
                    }
                    
                    // Scroll to bottom with safety checks - only if dialog is active
                    if (scrollRect != null && gameObject.activeInHierarchy)
                    {
                        StartCoroutine(ScrollToBottomNextFrame());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error refreshing simple logs: {ex.Message}");
            }
        }
        
        private System.Collections.IEnumerator ScrollToBottomNextFrame()
        {
            yield return null; // Wait one frame
            if (scrollRect != null && scrollRect.content != null)
            {
                try
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[AI Agent Plugin] Could not scroll to bottom: {ex.Message}");
                }
            }
        }

        private void OnClear()
        {
            logEntries.Clear();
            RefreshLogs();
            Debug.Log("[AI Agent Plugin] Simple logs cleared");
        }

        private void OnRefresh()
        {
            RefreshLogs();
        }

        private void OnClose()
        {
            Deactivate();
        }
    }
}