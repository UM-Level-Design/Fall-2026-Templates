using UnityEngine;

// Summary: Psudo Tag
// Devised to offer a scriptable reference to override the base checkpoint instead of using plain strings

namespace LevelDesign.Tags
{
    public class TAG_Checkpoint : MonoBehaviour
    {
        public int checkpointID;
        public event System.Action<TAG_Checkpoint> OnReached;

        public void ReachedCheckpoint() {
            OnReached?.Invoke(this);
        }
    }
}
