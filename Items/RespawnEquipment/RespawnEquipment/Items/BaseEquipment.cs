using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ColorCatalog;

namespace RespawnEquipment {
    // no this wasn't necessary I just copying and pasting stuff man
    public abstract class BaseEquipment<T> where T : BaseEquipment<T> {

        protected static string prefix = "DEFAULT_DEFAULT";

        protected static string name = "DEFAULT NAME";

        protected static string desc = "DEFAULT DESC";

        protected static string pickup = "DEFAULT PICKUP";

        protected static string lore = "<style=cMono>///EMPTY TRANSMISSION///</style>";

        protected static Func<Sprite> pickupIconSprite;

        protected static Func<GameObject> pickupModelPrefab;

        protected static Func<ItemDisplayRuleDict> itemDisplayRuleDict;

        public static EquipmentDef equipmentDef;

        protected static ColorIndex colorIndex = ColorIndex.Equipment;

        protected static float cooldown = 60f;

        protected static bool canDrop = true;

        protected static bool enigmaCompatible = true;

        protected static bool isLunar = false;

        protected static bool isBoss = false;

        protected static bool foodRelated = false;

        protected static bool appearsInSinglePlayer = true;

        protected static bool appearsInMultiPlayer = true;

        protected static bool canBeRandomlyTriggered = true;

        protected void CreateItem() {
            equipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();

            equipmentDef.name = $"{prefix}_NAME";
            equipmentDef.nameToken = $"{prefix}_NAME";
            equipmentDef.pickupToken = $"{prefix}_PICKUP";
            equipmentDef.descriptionToken = $"{prefix}_DESC";
            equipmentDef.loreToken = $"{prefix}_LORE";
            equipmentDef.cooldown = cooldown;
            equipmentDef.canDrop = canDrop;
            equipmentDef.enigmaCompatible = enigmaCompatible;
            equipmentDef.isLunar = isLunar;
            equipmentDef.isBoss = isBoss;
            equipmentDef.foodRelated = foodRelated;
            equipmentDef.appearsInSinglePlayer = appearsInSinglePlayer;
            equipmentDef.appearsInMultiPlayer = appearsInMultiPlayer;
            equipmentDef.pickupIconSprite = pickupIconSprite?.Invoke() ?? Addressables.LoadAssetAsync<Sprite>(RoR2_Base_Common_MiscIcons.texMysteryIcon_png).WaitForCompletion();
            equipmentDef.pickupModelPrefab = pickupModelPrefab?.Invoke() ?? Addressables.LoadAssetAsync<GameObject>(RoR2_Base_Mystery.PickupMystery_prefab).WaitForCompletion();

            LanguageAPI.Add($"{prefix}_NAME", name);
            LanguageAPI.Add($"{prefix}_DESC", desc);
            LanguageAPI.Add($"{prefix}_PICKUP", pickup);
            LanguageAPI.Add($"{prefix}_LORE", lore);

            var displayRules = itemDisplayRuleDict?.Invoke() ?? new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomEquipment(equipmentDef, displayRules));
            On.RoR2.EquipmentSlot.PerformEquipmentAction += CheckForEquipment;
        }

        protected abstract bool ActivateEquipment(EquipmentSlot slot);

        protected virtual bool CheckForEquipment(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef activatedEquipmentDef) {
            if (activatedEquipmentDef == equipmentDef) {
                return ActivateEquipment(self);
            }
            return orig(self, equipmentDef);
        }

        public virtual void Init() {
            CreateItem();
        }
    }
}
