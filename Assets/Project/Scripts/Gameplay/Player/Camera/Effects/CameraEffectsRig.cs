using UnityEngine;
using MoreMountains.Feedbacks;
using LevelDesign.Data;

namespace LevelDesign.Systems.Player
{
    public class CameraEffectsRig : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private MMF_Player cameraShakeEffect;

        [Header("Events")]
        [SerializeField] private DamagePlayerEventChannelSO e_playerDamaged;

        private void OnEnable() {
            e_playerDamaged.OnDamagePlayer += RunCameraShake;
        }

        private void OnDisable() {
            e_playerDamaged.OnDamagePlayer -= RunCameraShake;
        }

        private void RunCameraShake() {
            cameraShakeEffect.PlayFeedbacks();
        }
    }
}
