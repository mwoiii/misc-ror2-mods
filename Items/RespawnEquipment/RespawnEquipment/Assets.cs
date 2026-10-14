using R2API;
using RespawnEquipment.Components;
using RoR2;
using RoR2.UI;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace RespawnEquipment {
    public class Assets {

        public static AssetBundle assetBundle;

        public static GameObject characterSelectUI;

        public static BuffDef immunityBd = Addressables.LoadAssetAsync<BuffDef>(RoR2_Base_Common.bdImmune_asset).WaitForCompletion();

        public static GameObject polymorphEffect = Addressables.LoadAssetAsync<GameObject>(RoR2_DLC4_ShrineCompulsion.SurvivorPolymorphCharge_prefab).WaitForCompletion();

        public static GameObject pickupTransceiverPrefab;

        public static void Init() {
            GetAssetBundle();
            CreateCharacterSelectUI();
            CreatePickupModel();
        }

        private static void GetAssetBundle() {
            using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"RespawnEquipment.mwmwrespawnequipmentbundle")) {
                if (assetStream != null) {
                    assetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }
        }

        private static void CreateCharacterSelectUI() {
            characterSelectUI = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Command.CommandPickerPanel_prefab).WaitForCompletion(), "CharacterPickerPanel", false);
            characterSelectUI.GetComponent<MPButton>().enabled = false;
            var mainRectTransform = characterSelectUI.transform.Find("MainPanel/Juice/Label").GetComponent<LanguageTextMeshController>().token = Language.interactionHeader;

            CharacterPickerPanel characterPanel = characterSelectUI.AddComponent<CharacterPickerPanel>();
            PickupPickerPanel pickerPanel = characterSelectUI.GetComponent<PickupPickerPanel>();
            characterPanel.gridlayoutGroup = pickerPanel.gridlayoutGroup;
            characterPanel.buttonContainer = pickerPanel.buttonContainer;
            characterPanel.buttonPrefab = pickerPanel.buttonPrefab;
            characterPanel.coloredImages = pickerPanel.coloredImages;
            characterPanel.darkColoredImages = pickerPanel.darkColoredImages;
            characterPanel.useLockSpriteForUnavailableOptions = pickerPanel.useLockSpriteForUnavailableOptions;
            var button = characterPanel.buttonPrefab.GetComponent<HGButton>();
            ColorBlock colorBlock = button.colors;
            colorBlock.normalColor = Color.white;
            button.colors = colorBlock;
            var childLocator = button.GetComponent<ChildLocator>();
            Transform icon = childLocator.FindChild("Icon");
            Object.DestroyImmediate(icon.GetComponent<AsyncImage>());
            Object.DestroyImmediate(icon.GetComponent<Image>());
            var rawImage = icon.gameObject.AddComponent<RawImage>();
            button.targetGraphic = rawImage;
            Object.Destroy(button.GetComponent<Image>());
            Object.Destroy(pickerPanel);

            CharacterPickerPanelSizeAdjuster characterSizer = characterSelectUI.AddComponent<CharacterPickerPanelSizeAdjuster>();
            PickerPanelSizeAdjuster pickupSizer = characterSelectUI.GetComponent<PickerPanelSizeAdjuster>();
            characterSizer.pickerPanel = characterPanel;
            characterSizer.rectToAdjust = pickupSizer.rectToAdjust;
            characterSizer.minRows = pickupSizer.minRows;
            characterSizer.maxRows = pickupSizer.maxRows;
            characterSizer.adjustmentPerRow = pickupSizer.adjustmentPerRow;
            Object.Destroy(pickupSizer);
        }

        private static void CreatePickupModel() {
            pickupTransceiverPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>(RoR2_Base_DroneBackup.PickupRadio_prefab).WaitForCompletion(), "PickupNkuhanaTransceiver", false);
            var meshRenderer = pickupTransceiverPrefab.GetComponentInChildren<MeshRenderer>();
            var matCorruptRadio = new Material(meshRenderer.material);
            matCorruptRadio.SetTexture("_MainTex", assetBundle.LoadAsset<Texture>("texRadioDiffuse"));
            matCorruptRadio.SetTexture("_EmTex", assetBundle.LoadAsset<Texture>("texRadioEmission"));
            meshRenderer.sharedMaterials = [matCorruptRadio, Addressables.LoadAssetAsync<Material>(RoR2_DLC4_survival_common.matMalachiteOverlay_mat).WaitForCompletion()];
        }
    }
}
