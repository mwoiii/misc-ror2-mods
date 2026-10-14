using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Networking;
using RespawnEquipment.Networking;

namespace RespawnEquipment {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(NetworkingAPI.PluginGUID)]
    [BepInDependency(ItemAPI.PluginGUID)]
    [BepInDependency(PrefabAPI.PluginGUID)]
    public class RespawnEquipment : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "mwmw";
        public const string PluginName = "RespawnEquipment";
        public const string PluginVersion = "1.0.0";

        public static ConfigFile config;

        public void Awake() {
            config = Config;
            Log.Init(Logger);
            Language.Init();
            Assets.Init();
            NetworkMessages.Init();
            Options.Init();
            new UmbrousTransceiver().Init();
        }
    }
}
