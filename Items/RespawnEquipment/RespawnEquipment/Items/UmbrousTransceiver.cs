using R2API.Networking;
using R2API.Networking.Interfaces;
using RespawnEquipment.Components;
using RespawnEquipment.Networking;
using RoR2;
using UnityEngine;

namespace RespawnEquipment {
    public class UmbrousTransceiver : BaseEquipment<UmbrousTransceiver> {

        public override void Init() {
            prefix = "RESPAWNEQUIPMENT_UMBROUSTRANSCEIVER";
            name = "Umbrous Transceiver";
            desc = "<style=cIsUtility>Transform</style> into another survivor of your choice. Gain brief <style=cIsUtility>immunity</style> while selecting.";
            pickup = "Transform into another survivor of your choice.";
            lore =
                "> Patient claimed to be one of the troops who went missing. Tried prescribing quetiapine.\n\n" +
                "> Drug seemed to have no effect. Patient continuing to state that a \"weird device\" transformed them. Considering stronger medication.\n\n" +
                "> Patient revealed a concerning amount of accurate information about the missing troop. Suspect they were involved in foul play.\n\n" +
                "> Patient managed to contact the family of the missing troop. Don't know why the caretakers allowed that. But, for some reason, the family is corroborating the patient's claims.\n\n" +
                "> Patient has been released to the family of the missing troop, despite clearly being something alien. What a mess.";
            cooldown = Options.cooldown.Value;
            canBeRandomlyTriggered = false;
            pickupIconSprite = () => Assets.assetBundle.LoadAsset<Sprite>("texRadioIcon");
            pickupModelPrefab = () => Assets.pickupTransceiverPrefab;

            base.Init();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot) {
            if (slot.TryGetComponent(out CharacterPickerFlag pickerFlag)) {
                Object.Destroy(pickerFlag);
                return true;
            }

            if (slot.characterBody && slot.characterBody.master && slot.characterBody.master.playerCharacterMasterController) {
                new SyncOpenUI(slot.characterBody.master.bodyInstanceId).Send(NetworkDestination.Clients);
                if (!slot.TryGetComponent(out TransceiverImmunityController _)) {
                    slot.gameObject.AddComponent<TransceiverImmunityController>();
                }
            }

            return false;
        }
    }
}
