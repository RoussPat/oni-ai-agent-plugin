using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    /// <summary>
    /// Very simple test dialog to verify UI visibility
    /// </summary>
    public class SimpleTestDialog : MonoBehaviour
    {
        private static SimpleTestDialog instance;
        private Canvas dialogCanvas;
        
        public static void ShowTestDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Showing Simple Test Dialog");
                
                if (instance == null)
                {
                    CreateTestDialog();
                }
                
                if (instance != null)
                {
                    instance.Show();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing test dialog: {ex.Message}");
            }
        }
        
        private static void CreateTestDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Creating Simple Test Dialog...");
                
                // Create dialog GameObject
                GameObject dialogObj = new GameObject("SimpleTestDialog");
                DontDestroyOnLoad(dialogObj);
                
                instance = dialogObj.AddComponent<SimpleTestDialog>();
                instance.SetupTestDialog();
                
                Debug.Log("[AI Agent Plugin] Simple Test Dialog created successfully");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error creating test dialog: {ex.Message}");
            }
        }
        
        private void SetupTestDialog()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Setting up test dialog...");
                
                // Create canvas
                var canvasObj = new GameObject("TestCanvas");
                canvasObj.transform.SetParent(transform, false);
                
                dialogCanvas = canvasObj.AddComponent<Canvas>();
                dialogCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                dialogCanvas.sortingOrder = 9999; // Very high priority
                
                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // Create background panel
                var panel = new GameObject("Background");
                panel.transform.SetParent(canvasObj.transform, false);
                
                var panelImage = panel.AddComponent<Image>();
                panelImage.color = new Color(0, 0, 0, 0.8f); // Black background
                
                var panelRect = panel.GetComponent<RectTransform>();
                panelRect.anchorMin = Vector2.zero;
                panelRect.anchorMax = Vector2.one;
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;
                
                // Create text
                var textObj = new GameObject("TestText");
                textObj.transform.SetParent(canvasObj.transform, false);
                
                var text = textObj.AddComponent<Text>();
                text.text = "AI AGENT PLUGIN TEST DIALOG\n\nIf you can see this, the UI system is working!\n\nPress ESC to close";
                text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                text.fontSize = 24;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;
                text.fontStyle = FontStyle.Bold;
                
                var textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.2f, 0.2f);
                textRect.anchorMax = new Vector2(0.8f, 0.8f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                
                Debug.Log("[AI Agent Plugin] Test dialog setup complete");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error setting up test dialog: {ex.Message}");
            }
        }
        
        private void Show()
        {
            try
            {
                Debug.Log("[AI Agent Plugin] Showing test dialog...");
                dialogCanvas.enabled = true;
                gameObject.SetActive(true);
                Canvas.ForceUpdateCanvases();
                Debug.Log("[AI Agent Plugin] Test dialog should now be visible!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[AI Agent Plugin] Error showing test dialog: {ex.Message}");
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("[AI Agent Plugin] ESC pressed - closing test dialog");
                dialogCanvas.enabled = false;
                gameObject.SetActive(false);
            }
        }
    }
}
