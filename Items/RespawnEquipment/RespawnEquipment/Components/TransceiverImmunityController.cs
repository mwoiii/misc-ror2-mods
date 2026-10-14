using RoR2;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RespawnEquipment.Components {
    public class TransceiverImmunityController : MonoBehaviour {
        private float stopwatch;

        private CharacterBody body;

        private bool removedBuff;

        private void Awake() {
            body = GetComponent<CharacterBody>();
            if (body) {
                body.AddBuff(Assets.immunityBd);
            }
        }

        private void FixedUpdate() {
            stopwatch += Time.deltaTime;
            if (stopwatch >= Options.immunityDuration.Value) {
                RemoveBuff();
            }

            if (stopwatch >= Options.immunityPeriod.Value) {
                Destroy(this);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBuff() {
            if (!removedBuff && body) {
                body.RemoveBuff(Assets.immunityBd);
                removedBuff = true;
            }
        }
    }
}
