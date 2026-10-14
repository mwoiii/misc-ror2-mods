using R2API;

namespace RespawnEquipment {
    public static class Language {

        public static string interactionHeader;

        private const string prefix = "MIYOWI_RESPAWNEQUIPMENT_";

        public static void Init() {
            interactionHeader = Add("INTERACTION_HEADER", "Select your Replacement.");
        }

        public static string Add(string token, string text) {
            string safe_token = $"{prefix}{token}";
            LanguageAPI.Add(safe_token, text);
            return safe_token;
        }
    }
}
