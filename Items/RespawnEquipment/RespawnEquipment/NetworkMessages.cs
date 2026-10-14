using R2API.Networking;
using R2API.Networking.Interfaces;
using RespawnEquipment.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RespawnEquipment.Networking {
    public static class NetworkMessages {
        public static void Init() {
            NetworkingAPI.RegisterMessageType<SyncOpenUI>();
            NetworkingAPI.RegisterMessageType<SyncCloseUI>();
            NetworkingAPI.RegisterMessageType<SyncRespawnRequest>();
        }
    }

    public class SyncOpenUI : INetMessage {
        NetworkInstanceId netInstanceId;

        public SyncOpenUI() {
        }

        public SyncOpenUI(NetworkInstanceId netInstanceId) {
            this.netInstanceId = netInstanceId;
        }

        public void Serialize(NetworkWriter writer) {
            writer.Write(netInstanceId);
        }

        public void Deserialize(NetworkReader reader) {
            netInstanceId = reader.ReadNetworkId();
        }

        public void OnReceived() {
            GameObject bodyObject = Util.FindNetworkObject(netInstanceId);
            if (bodyObject && Util.HasEffectiveAuthority(bodyObject) && bodyObject.TryGetComponent(out CharacterBody body) && body.master && body.master.playerCharacterMasterController) {
                CameraRigController cameraRigController = null;
                foreach (CameraRigController rig in CameraRigController.readOnlyInstancesList) {
                    if (rig.target == bodyObject) {
                        cameraRigController = rig;
                        break;
                    }
                }

                if (cameraRigController) {
                    GameObject panel = Object.Instantiate(Assets.characterSelectUI, cameraRigController.hud.mainContainer.transform);
                    CharacterPickerPanel panelController = panel.GetComponent<CharacterPickerPanel>();
                    if (panelController) {
                        panelController.SetupOptions(body.master.playerCharacterMasterController);
                    }
                }
            }
        }
    }

    public class SyncCloseUI : INetMessage {
        NetworkInstanceId netInstanceId;

        public SyncCloseUI() {
        }

        public SyncCloseUI(NetworkInstanceId netInstanceId) {
            this.netInstanceId = netInstanceId;
        }

        public void Serialize(NetworkWriter writer) {
            writer.Write(netInstanceId);
        }

        public void Deserialize(NetworkReader reader) {
            netInstanceId = reader.ReadNetworkId();
        }

        public void OnReceived() {
            GameObject bodyObject = Util.FindNetworkObject(netInstanceId);
            if (bodyObject && bodyObject.TryGetComponent(out TransceiverImmunityController immunityController)) {
                immunityController.RemoveBuff();
            }
        }
    }

    public class SyncRespawnRequest : INetMessage {
        NetworkInstanceId netInstanceId;
        string bodyName;

        public SyncRespawnRequest() {
        }

        public SyncRespawnRequest(NetworkInstanceId netInstanceId, string bodyName) {
            this.netInstanceId = netInstanceId;
            this.bodyName = bodyName;
        }

        public void Serialize(NetworkWriter writer) {
            writer.Write(netInstanceId);
            writer.Write(bodyName);
        }

        public void Deserialize(NetworkReader reader) {
            netInstanceId = reader.ReadNetworkId();
            bodyName = reader.ReadString();
        }

        public void OnReceived() {
            GameObject bodyObject = Util.FindNetworkObject(netInstanceId);
            if (bodyObject && bodyObject.TryGetComponent(out CharacterBody body) && body.master) {
                EffectData effectData = new EffectData {
                    origin = bodyObject.transform.position
                };
                EffectManager.SpawnEffect(Assets.polymorphEffect, effectData, true);
                if (body.equipmentSlot) {
                    body.gameObject.AddComponent<CharacterPickerFlag>();
                    body.equipmentSlot.OnEquipmentExecuted(body.equipmentSlot.activeEquipmentLocation, UmbrousTransceiver.equipmentDef.equipmentIndex);
                }
                body.master.TransformBody(bodyName);
            }
        }
    }
}
