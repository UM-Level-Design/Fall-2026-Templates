using UnityEngine;
using LevelDesign.Tags;

// Summary: 
// A System devised to start / load the player, and update points in which the player should respawn to

namespace LevelDesign.Gameplay.Levels
{
    public class CheckpointManager : MonoBehaviour
    {
        public Transform SpawnPoint { get; private set; }

        [Header("Debug")]
        [SerializeField] private int currentCheckpointID;
        [SerializeField] private TAG_Checkpoint[] checkpoints;
        [SerializeField] private TAG_CheckpointOverride[] checkpointOverrides;

        private int overrideIndex = -1;

        private void Awake() {
            ScourScene();
        }

        private void ScourScene() {
            checkpoints = FindObjectsByType<TAG_Checkpoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            checkpointOverrides = FindObjectsByType<TAG_CheckpointOverride>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); 

            DefineSpawnCheckpoint();
        }    

        private void DefineSpawnCheckpoint() {
            if(checkpointOverrides != null && checkpointOverrides.Length > 0) {
                overrideIndex = 0;
                SetActiveCheckpoint(checkpointOverrides[overrideIndex].transform);
                return;
            }

            // Default to the checkpoint with the lowest ID
            TAG_Checkpoint lowest = null;
            foreach(TAG_Checkpoint checkpoint in checkpoints) {
                if(lowest == null || checkpoint.checkpointID < lowest.checkpointID) { 
                    lowest = checkpoint;
                }
            }

            if(lowest != null) {
                currentCheckpointID = lowest.checkpointID;
                SetActiveCheckpoint(lowest.transform);
            }
        }

        #region Public Accessors
        public void SetActiveCheckpoint(Transform spawnPoint) {
            if(spawnPoint == null) { return; }

            SpawnPoint = spawnPoint;
        }

        public void CycleOverrideCheckpoint() {
            if(checkpointOverrides == null || checkpointOverrides.Length == 0) { return; }

            overrideIndex = (overrideIndex + 1) % checkpointOverrides.Length;
            SetActiveCheckpoint(checkpointOverrides[overrideIndex].transform);
        }
        #endregion
    }
}