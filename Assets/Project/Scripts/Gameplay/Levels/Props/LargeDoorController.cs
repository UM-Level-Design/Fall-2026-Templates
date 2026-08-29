using UnityEngine;
using LevelDesign.Gameplay.Levels;
using LevelDesign.Data;

namespace LevelDesign.Gameplay.Levels
{
    public class LargeDoorController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private int maxKeysNeeded;

        [Header("Scene Refs")]
        [SerializeField] private AnimatorTrigger propAnimator;
        [SerializeField] private KeyCollectedEventChannelSO keyCollectedChannel;

        private int currentKeyCount;

        private void OnEnable()
        {
            keyCollectedChannel.OnKeyCollected += HandleKeyCollected;
        }
 
        private void OnDisable()
        {
            keyCollectedChannel.OnKeyCollected -= HandleKeyCollected;
        }

        public void Update() {
            if(currentKeyCount == maxKeysNeeded) {
                propAnimator.SetTrue();
            }
        }

        private void HandleKeyCollected() {
            currentKeyCount++;
        }
    }
}
