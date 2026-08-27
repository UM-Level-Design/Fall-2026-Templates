using UnityEngine;
using LevelDesign.Tags;

// Summary: 
// A System devised to start / load the player, and update points in which the player should respawn to

namespace LevelDesign.Gameplay.Levels
{
    public class CheckpointManager : MonoBehaviour
    {
        [Header("Debug - Runtime")]
        [SerializeField] private TAG_Checkpoint[] checkpoints;
        [SerializeField] private TAG_CheckpointOverride[] checkpointOverrides;

        [Space]
        [SerializeField] private TAG_Checkpoint activeCheckpoint;
        [SerializeField] private TAG_Checkpoint forcedCheckpoint;


        private void Awake() {
            ScourScene();
        }

        private void ScourScene() {
            checkpoints = FindObjectsByType<TAG_Checkpoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            checkpointOverrides = FindObjectsByType<TAG_CheckpointOverride>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); 
        }    

        #region Commands
            // Location for things such as loading a new checkpoint through commands
        #endregion
    }
}
