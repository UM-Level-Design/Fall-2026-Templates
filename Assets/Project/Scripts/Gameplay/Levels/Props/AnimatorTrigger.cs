using UnityEngine;

namespace LevelDesign.Gameplay.Levels
{
    public class AnimatorTrigger : MonoBehaviour
    {
        [SerializeField] private string animationBool;
        [SerializeField] private Animator animator;

        public void SetTrue(string animationString = null)
        {
            animator.SetBool(string.IsNullOrEmpty(animationString) ? animationBool : animationString, true);
        }

        public void SetFalse(string animationString = null)
        {
            animator.SetBool(string.IsNullOrEmpty(animationString) ? animationBool : animationString, false);
        }
    }
}