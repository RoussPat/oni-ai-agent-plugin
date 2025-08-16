using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    /// <summary>
    /// Simple floating UI that is guaranteed to work
    /// </summary>
    public class SimpleFloatingUI : MonoBehaviour
    {
        private static SimpleFloatingUI instance;
        private GameObject aiButton;
        private Canvas uiCanvas;
        
        public static void Initialize()
        {
            if (instance == null)
            {
                var go = new GameObject("SimpleFloatingUI");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<SimpleFloatingUI>();
                instance.CreateFloatingUI();
            }
        }
        
        private void CreateFloatingUI()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating simple floating UI...");
                
                // Create canvas
                var canvasObj = new GameObject("SimpleAICanvas");
                canvasObj.transform.SetParent(transform, false);
                
                uiCanvas = canvasObj.AddComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                uiCanvas.sortingOrder = 9999; // Very high priority
                
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // Create the AI button
                aiButton = new GameObject("SimpleAIButton");
                aiButton.transform.SetParent(canvasObj.transform, false);
                
                // Add Image component
                var image = aiButton.AddComponent<Image>();
                image.color = new Color(0.2f, 0.8f, 0.2f, 0.9f); // Bright green
                
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
                text.fontSize = 20;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                
                // Position text
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                // Position button in top-right corner
                var buttonRect = aiButton.GetComponent<RectTransform>();
                buttonRect.anchorMin = new Vector2(1, 1);
                buttonRect.anchorMax = new Vector2(1, 1);
                buttonRect.anchoredPosition = new Vector2(-100, -100);
                buttonRect.sizeDelta = new Vector2(80, 80); // Large, visible button
                
                Debug.Log("[AI Agent Plugin] Simple floating UI created successfully!");
                SimpleLogsDialog.AddLogEntry("Simple floating UI created - green AI button in top-right corner");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating simple floating UI: {ex.Message}");
            }
        }
        
        private void OnAIButtonClick()
        {
            Debug.Log("[AI Agent Plugin] Simple AI button clicked!");
            SimpleLogsDialog.AddLogEntry("Simple AI button clicked - opening dialog");
            AgentControlDialog.ShowDialog();
        }
        
        // Make sure the UI stays visible
        void Update()
        {
            if (aiButton != null && !aiButton.activeSelf)
            {
                aiButton.SetActive(true);
            }
        }
    }
}
