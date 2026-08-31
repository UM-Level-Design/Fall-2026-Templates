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
        [SerializeField] private KeyCollectedEventChannelSO e_KeyCollected;

        private int currentKeyCount;

        private void OnEnable()
        {
            e_KeyCollected.OnKeyCollected += HandleKeyCollected;
        }
 
        private void OnDisable()
        {
            e_KeyCollected.OnKeyCollected -= HandleKeyCollected;
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
