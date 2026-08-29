using UnityEngine;

namespace LevelDesign.Gameplay.Levels
{
    public class CollectableAnimator : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float spinRate = 90f;

        [Header("Scene Refs")]
        [SerializeField] private GameObject KeyObject;

        private void Update()
        {
            AnimateSpin();
        }

        private void AnimateSpin()
        {
            KeyObject.transform.Rotate(Vector3.up, spinRate * Time.deltaTime, Space.World);
        }
    }
}