using UnityEngine;

namespace RespawnEquipment.Components {
    // copied and modified PickerPanelSizeAdjuster
    public class CharacterPickerPanelSizeAdjuster : MonoBehaviour {
        public CharacterPickerPanel pickerPanel;

        public RectTransform rectToAdjust;

        [Tooltip("How many rows can we have before we begin to adjust")]
        public int minRows = 5;

        [Tooltip("After this many rows, we will not adjust the size any higher")]
        public int maxRows = 7;

        [Tooltip("How much should we adjust the window for each new row")]
        public float adjustmentPerRow;

        [Tooltip("This will be set automatically")]
        public float objectHeight;

        private bool capturedDefaults;

        private void Awake() {
            pickerPanel.OnPanelSetupComplete += AdjustPickerPanelSize;
        }

        private void CaptureDefaults() {
            if (capturedDefaults) {
                return;
            }
            RectTransform buttonContainer = pickerPanel.buttonContainer;
            if (buttonContainer != null && buttonContainer.childCount > 0) {
                RectTransform component = buttonContainer.GetChild(0).GetComponent<RectTransform>();
                if (component != null) {
                    objectHeight = component.rect.height;
                }
            }
            capturedDefaults = true;
        }

        private void AdjustPickerPanelSize() {
            int maxColumnCount = Mathf.Max(1, pickerPanel.maxColumnCount);
            int rows = pickerPanel.buttonContainer.childCount / maxColumnCount;
            rows = Mathf.Clamp(rows, minRows, maxRows) - minRows;
            rectToAdjust.ForceUpdateRectTransforms();
            CaptureDefaults();
            if (rows > 0) {
                float num2 = adjustmentPerRow / 2f * rows;
                Vector2 offsetMin = rectToAdjust.offsetMin;
                rectToAdjust.offsetMin = new Vector2(offsetMin.x, offsetMin.y - num2);
                offsetMin = rectToAdjust.offsetMax;
                rectToAdjust.offsetMax = new Vector2(offsetMin.x, offsetMin.y + num2);
            }

            rectToAdjust.sizeDelta = new Vector2(250f + 120 * (maxColumnCount - 1), rectToAdjust.sizeDelta.y);
        }
    }
}
