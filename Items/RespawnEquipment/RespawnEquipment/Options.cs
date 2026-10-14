using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using UnityEngine;

namespace RespawnEquipment {

    public class Options {

        private static bool? _rooEnabled;

        public static bool rooEnabled {
            get {
                if (_rooEnabled == null) {
                    _rooEnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                }
                return (bool)_rooEnabled;
            }
        }

        public static ConfigEntry<int> uiColumns { get; set; }

        public static ConfigEntry<float> immunityPeriod { get; set; }

        public static ConfigEntry<float> immunityDuration { get; set; }

        public static ConfigEntry<float> cooldown { get; set; }

        public static void Init() {
            uiColumns = RespawnEquipment.config.Bind("Umbral Transceiver", "UI Columns", 8, "The number of columns in the character selection UI. Increase this if icons start to overflow. (Client-side)");

            immunityPeriod = RespawnEquipment.config.Bind("Umbral Transceiver", "Immunity Period", 60f, "The minimum period, in seconds, between the application of temporary immunity buffs. (Server-side)");

            immunityDuration = RespawnEquipment.config.Bind("Umbral Transceiver", "Immunity Duration", 5f, "The maximum amount of seconds the temporary immunity buff can last while in the menu. (Server-side)");

            cooldown = RespawnEquipment.config.Bind("Umbral Transceiver", "Cooldown", 300f, "The cooldown of the equipment, in seconds. (Server-side)");
            cooldown.SettingChanged += (object sender, System.EventArgs e) => {
                UmbrousTransceiver.equipmentDef.cooldown = cooldown.Value;
            };

            if (rooEnabled) {
                RoOInit();
            }
        }

        private static void RoOInit() {
            ModSettingsManager.AddOption(new IntSliderOption(uiColumns, new IntSliderConfig() { min = 1, max = 30 }));
            ModSettingsManager.AddOption(new StepSliderOption(immunityPeriod, new StepSliderConfig() { min = 0f, max = 3000f, increment = 0.1f }));
            ModSettingsManager.AddOption(new StepSliderOption(immunityDuration, new StepSliderConfig() { min = 0f, max = 600f, increment = 0.1f }));
            ModSettingsManager.AddOption(new StepSliderOption(cooldown, new StepSliderConfig() { min = 0f, max = 3000f, increment = 0.1f }));

            ModSettingsManager.SetModDescription("Config options for the Umbrous Transceiver mod.");
            ModSettingsManager.SetModIcon(Assets.assetBundle.LoadAsset<Sprite>("texRadioIcon"));
        }
    }
}
