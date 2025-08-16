using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AIAgentPlugin
{
    public static class UIComponents
    {
        public static Dropdown CreateDropdown(GameObject parent, List<string> options, string label = "")
        {
            var container = new GameObject("DropdownContainer");
            container.transform.SetParent(parent.transform, false);
            
            var containerLayout = container.AddComponent<VerticalLayoutGroup>();
            containerLayout.spacing = 5;
            containerLayout.childControlHeight = false;
            containerLayout.childControlWidth = true;
            
            // Add label if provided
            if (!string.IsNullOrEmpty(label))
            {
                var labelObj = new GameObject("Label");
                labelObj.transform.SetParent(container.transform, false);
                var labelText = labelObj.AddComponent<Text>();
                labelText.text = label;
                labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                labelText.fontSize = 14;
                labelText.color = Color.white;
                
                var labelRect = labelObj.GetComponent<RectTransform>();
                labelRect.sizeDelta = new Vector2(0, 20);
                
                var labelLayout = labelObj.AddComponent<LayoutElement>();
                labelLayout.minHeight = 20;
                labelLayout.preferredHeight = 20;
            }
            
            // Create dropdown
            var dropdownObj = new GameObject("Dropdown");
            dropdownObj.transform.SetParent(container.transform, false);
            
            var dropdownImage = dropdownObj.AddComponent<Image>();
            dropdownImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            var dropdown = dropdownObj.AddComponent<Dropdown>();
            dropdown.targetGraphic = dropdownImage;
            
            // Create label for dropdown
            var dropdownLabelObj = new GameObject("Label");
            dropdownLabelObj.transform.SetParent(dropdownObj.transform, false);
            var dropdownLabel = dropdownLabelObj.AddComponent<Text>();
            dropdownLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            dropdownLabel.fontSize = 14;
            dropdownLabel.color = Color.white;
            dropdownLabel.alignment = TextAnchor.MiddleLeft;
            
            var dropdownLabelRect = dropdownLabelObj.GetComponent<RectTransform>();
            dropdownLabelRect.anchorMin = Vector2.zero;
            dropdownLabelRect.anchorMax = Vector2.one;
            dropdownLabelRect.offsetMin = new Vector2(10, 0);
            dropdownLabelRect.offsetMax = new Vector2(-25, 0);
            
            // Create arrow
            var arrowObj = new GameObject("Arrow");
            arrowObj.transform.SetParent(dropdownObj.transform, false);
            var arrow = arrowObj.AddComponent<Image>();
            arrow.color = Color.white;
            
            var arrowRect = arrowObj.GetComponent<RectTransform>();
            arrowRect.anchorMin = new Vector2(1, 0.5f);
            arrowRect.anchorMax = new Vector2(1, 0.5f);
            arrowRect.sizeDelta = new Vector2(20, 20);
            arrowRect.anchoredPosition = new Vector2(-15, 0);
            
            // Create template
            var templateObj = new GameObject("Template");
            templateObj.transform.SetParent(dropdownObj.transform, false);
            templateObj.SetActive(false);
            
            var templateImage = templateObj.AddComponent<Image>();
            templateImage.color = new Color(0.15f, 0.15f, 0.15f, 1f);
            
            var templateRect = templateObj.GetComponent<RectTransform>();
            templateRect.anchorMin = new Vector2(0, 0);
            templateRect.anchorMax = new Vector2(1, 0);
            templateRect.offsetMin = new Vector2(0, -100);
            templateRect.offsetMax = new Vector2(0, 0);
            
            // Create content
            var contentObj = new GameObject("Content");
            contentObj.transform.SetParent(templateObj.transform, false);
            
            var contentRect = contentObj.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;
            
            // Create item
            var itemObj = new GameObject("Item");
            itemObj.transform.SetParent(contentObj.transform, false);
            
            var itemToggle = itemObj.AddComponent<Toggle>();
            
            var itemBg = itemObj.AddComponent<Image>();
            itemBg.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            
            var itemLabelObj = new GameObject("Item Label");
            itemLabelObj.transform.SetParent(itemObj.transform, false);
            var itemLabel = itemLabelObj.AddComponent<Text>();
            itemLabel.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            itemLabel.fontSize = 14;
            itemLabel.color = Color.white;
            itemLabel.alignment = TextAnchor.MiddleLeft;
            
            var itemLabelRect = itemLabelObj.GetComponent<RectTransform>();
            itemLabelRect.anchorMin = Vector2.zero;
            itemLabelRect.anchorMax = Vector2.one;
            itemLabelRect.offsetMin = new Vector2(10, 0);
            itemLabelRect.offsetMax = new Vector2(-10, 0);
            
            itemToggle.targetGraphic = itemBg;
            itemToggle.graphic = null;
            
            var itemRect = itemObj.GetComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(0, 30);
            
            // Configure dropdown
            dropdown.captionText = dropdownLabel;
            dropdown.itemText = itemLabel;
            dropdown.template = templateRect;
            dropdown.options.Clear();
            
            foreach (var option in options)
            {
                dropdown.options.Add(new Dropdown.OptionData(option));
            }
            
            dropdown.value = 0;
            dropdown.RefreshShownValue();
            
            var dropdownRect = dropdownObj.GetComponent<RectTransform>();
            dropdownRect.sizeDelta = new Vector2(0, 32);
            
            var dropdownLayout = dropdownObj.AddComponent<LayoutElement>();
            dropdownLayout.minHeight = 32;
            dropdownLayout.preferredHeight = 32;
            
            var containerRect = container.GetComponent<RectTransform>();
            containerRect.sizeDelta = new Vector2(0, string.IsNullOrEmpty(label) ? 32 : 57);
            
            var containerLayoutElement = container.AddComponent<LayoutElement>();
            containerLayoutElement.minHeight = string.IsNullOrEmpty(label) ? 32 : 57;
            containerLayoutElement.preferredHeight = string.IsNullOrEmpty(label) ? 32 : 57;
            
            return dropdown;
        }
    }
}