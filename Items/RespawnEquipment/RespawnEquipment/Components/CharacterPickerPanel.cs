using R2API.Networking;
using R2API.Networking.Interfaces;
using RespawnEquipment.Networking;
using RoR2;
using RoR2.EntitlementManagement;
using RoR2.ExpansionManagement;
using RoR2.UI;
using RoR2BepInExPack.GameAssetPathsBetter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RespawnEquipment.Components {
    // copied and modified PickupPickerPanel
    [RequireComponent(typeof(RectTransform))]
    public class CharacterPickerPanel : MonoBehaviour {

        private PlayerCharacterMasterController player;

        public struct Option {
            public SurvivorIndex survivorIndex;

            public bool available;

            public Texture2D texture;

            public Color? overrideUnavailableBGColor;

            public Color? overrideSelectedBGColor;
        }

        private Option[] characterOptions;

        public GridLayoutGroup gridlayoutGroup;

        public RectTransform buttonContainer;

        public GameObject buttonPrefab;

        public Image[] coloredImages;

        public Image[] darkColoredImages;

        public int maxColumnCount;

        public bool useLockSpriteForUnavailableOptions = true;

        private UIElementAllocator<MPButton> buttonAllocator;

        public bool shouldChangeButtonFrameColor;

        public bool shouldLeaveDisabledButtonsInteractable;

        [SerializeField]
        [Tooltip("If the root of the panel is a button, keep this false!")]
        private bool automaticButtonNavigation;

        private bool _inspectFeatureIsEnabled;

        private Sprite lockedSprite;

        public Action OnPanelSetupComplete;

        private bool cancelled = true;

        private void Awake() {
            maxColumnCount = Mathf.Max(1, Options.uiColumns.Value);
            buttonAllocator = new UIElementAllocator<MPButton>(buttonContainer, buttonPrefab) {
                onCreateElement = OnCreateButton
            };
            gridlayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridlayoutGroup.constraintCount = maxColumnCount;
            if (EventSystem.current && EventSystem.current is MPEventSystem mpEventSystem) {
                _inspectFeatureIsEnabled = mpEventSystem.localUser?.userProfile?.useInspectFeature == true;
            }
        }

        public void SetupOptions(PlayerCharacterMasterController player) {
            this.player = player;
            List<Option> options = new List<Option>();
            foreach (var survivorDef in SurvivorCatalog.survivorDefs) {
                if (!survivorDef.hidden &&
                    survivorDef.bodyPrefab &&
                    (!survivorDef.bodyPrefab.TryGetComponent(out ExpansionRequirementComponent expansion) || expansion.PlayerCanUseBody(player))
                   ) {
                    options.Add(new Option() {
                        survivorIndex = survivorDef.survivorIndex,
                        available = !survivorDef.unlockableDef || EntitlementManager.localUserEntitlementTracker.UserHasEntitlementForUnlockable(LocalUserManager.GetFirstLocalUser(), survivorDef.unlockableDef),
                        texture = survivorDef.bodyPrefab.GetComponent<CharacterBody>()?.portraitIcon as Texture2D
                    });
                }
            }
            SetButtons(options.ToArray());
        }

        private void OnCreateButton(int index, MPButton button) {
            button.onClick.AddListener(() => {
                if (player && player.master) {
                    new SyncRespawnRequest(player.master.bodyInstanceId, SurvivorCatalog.GetSurvivorDef(characterOptions[index].survivorIndex).bodyPrefab.name).Send(NetworkDestination.Server);
                }
                cancelled = false;
                Destroy(gameObject);
            });
        }

        private void EnsureLockedSprite() {
            if (lockedSprite == null) {
                lockedSprite = Addressables.LoadAssetAsync<Sprite>(RoR2_Base_Common_MiscIcons.texUnlockIconSmall_png).WaitForCompletion();
            }
        }

        private void OnDestroy() {
            if (cancelled && player && player.master) {
                new SyncCloseUI(player.master.bodyInstanceId).Send(NetworkDestination.Server);
            }
        }

        public void RemovePickupButtonAvailability(int characterOption) {
            EnsureLockedSprite();
            characterOptions[characterOption].available = false;
            MPButton mPButton = buttonAllocator.elements[characterOption];
            RawImage rawImage = mPButton.GetComponent<ChildLocator>().FindChild("Icon").GetComponent<RawImage>();
            rawImage.color = Color.gray;
            rawImage.texture = lockedSprite.texture;
            if (!shouldLeaveDisabledButtonsInteractable) {
                mPButton.interactable = false;
            }
            if (mPButton.TryGetComponent(out TooltipProvider tooltipProvider)) {
                tooltipProvider.enabled = false;
            }
        }

        private void SetButtons(Option[] options) {
            characterOptions = options;
            buttonAllocator.AllocateElements(options.Length);
            ReadOnlyCollection<MPButton> elements = buttonAllocator.elements;
            EnsureLockedSprite();
            new StringBuilder().Append($"PANELDEBUG: Picker Panel for {gameObject.name}: {options.Length} options\r\n");
            for (int j = 0; j < options.Length; j++) {
                MPButton mPButton = elements[j];
                int num = j - j % maxColumnCount;
                int num2 = j % maxColumnCount;
                int num3 = num2 - maxColumnCount;
                int num4 = num2 - 1;
                int num5 = num2 + 1;
                int num6 = num2 + maxColumnCount;
                Navigation navigation = mPButton.navigation;
                if (automaticButtonNavigation) {
                    navigation.mode = Navigation.Mode.Automatic;
                } else {
                    navigation.mode = Navigation.Mode.Explicit;
                    if (num4 >= 0) {
                        MPButton selectOnLeft = elements[num + num4];
                        navigation.selectOnLeft = selectOnLeft;
                    }
                    if (num5 < maxColumnCount && num + num5 < options.Length) {
                        MPButton selectOnRight = elements[num + num5];
                        navigation.selectOnRight = selectOnRight;
                    }
                    if (num + num3 >= 0) {
                        MPButton selectOnUp = elements[num + num3];
                        navigation.selectOnUp = selectOnUp;
                    }
                    if (num + num6 < options.Length) {
                        MPButton selectOnDown = elements[num + num6];
                        navigation.selectOnDown = selectOnDown;
                    }
                }
                mPButton.navigation = navigation;
                UpdateButtonUI(j);
            }
            OnPanelSetupComplete?.Invoke();
        }

        private void UpdateButtonUI(int index) {
            MPButton mpButton = buttonAllocator.elements[index];
            Option option = characterOptions[index];
            SurvivorDef survivorDef = SurvivorCatalog.GetSurvivorDef(option.survivorIndex);
            ChildLocator childLocator = mpButton.GetComponent<ChildLocator>();
            RawImage rawImage = childLocator.FindChild("Icon").GetComponent<RawImage>();
            if (option.available) {
                rawImage.color = Color.white;
                rawImage.texture = option.texture;
                mpButton.interactable = true;
            } else {
                rawImage.color = Color.gray;
                if (useLockSpriteForUnavailableOptions) {
                    rawImage.texture = lockedSprite.texture;
                } else {
                    rawImage.texture = option.texture;
                }
                if (!shouldLeaveDisabledButtonsInteractable) {
                    mpButton.interactable = false;
                }
            }
            if (shouldChangeButtonFrameColor) {
                mpButton.image.color = (option.available ? Color.white : new Color(0.1f, 0.1f, 0.1f, 1f));
            }
            ColorBlock colors = mpButton.colors;
            bool overrideAnyColors = false;
            if (option.overrideUnavailableBGColor.HasValue && !option.available) {
                overrideAnyColors = true;
                colors.selectedColor = option.overrideUnavailableBGColor.Value;
            } else if (option.overrideSelectedBGColor.HasValue) {
                overrideAnyColors = true;
                colors.selectedColor = option.overrideSelectedBGColor.Value;
            }
            if (overrideAnyColors) {
                mpButton.colors = colors;
            }
            if (mpButton.TryGetComponent<TooltipProvider>(out var component4)) {
                component4.enabled = _inspectFeatureIsEnabled && option.available;
                Color visibleSurvivorColor = survivorDef.primaryColor;
                visibleSurvivorColor.a = 1f;
                component4.SetContent(new TooltipContent {
                    bodyColor = visibleSurvivorColor,
                    titleToken = survivorDef.displayNameToken,
                    titleColor = visibleSurvivorColor
                });
            }
        }
    }
}
