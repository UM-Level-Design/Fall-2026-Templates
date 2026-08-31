using UnityEngine;
using LevelDesign.Gameplay.Levels;
using LevelDesign.Data;
using UnityEngine.SceneManagement;

namespace LevelDesign.Systems.Player
{
    public class Player : MonoBehaviour
    {
        [Header("State Machine")]
        [SerializeField] private PlayerStateMachine PSM;
        
        [Header("Managers")]
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [SerializeField] private _MovementController playerCharacter;
        [SerializeField] private Health healthM;

        [Header("Managers")]
        [SerializeField] private CharacterDataManager characterDataM;
        [Space]
        [SerializeField] private CheckpointManager checkpointM;

        [Header("Events")]
        [SerializeField] private KillPlayerEventChannelSO e_killPlayer;

        private Transform cameraFocalTarget;
        private Transform spectatorCameraTarget;

        #region Unity Calls
        private void OnEnable() {
            if(e_killPlayer != null) {
                e_killPlayer.OnKillRequested += KillPlayer;
            }
        }

        private void OnDisable() {
            if(e_killPlayer != null) {
                e_killPlayer.OnKillRequested -= KillPlayer;
            }
        }

        private void Start() {
            playerCamera.Initialize(PSM);
            characterDataM.Initialize();

            ScourScene();
        }

        private void Update() {
            RoutineChecks();
            ProcessControllers();
        }

        private void LateUpdate()
        {
            UpdateCameraTarget();
        }
        #endregion

        #region Controllers
        // Both
        public void ProcessControllers() {
            if(playerCamera != null)
            {
                playerCamera.UpdateCameraInput();
                // playerCamera.UpdateRotation();
                playerCamera.gameObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
            if(playerCharacter != null && playerCamera != null)
            {
                playerCharacter._UpdateBody(Time.deltaTime);
            }
        }

        // Camera Controller
        public void UpdateCameraTarget()
        {
            if (playerCharacter == null || playerCamera == null)
                return;

            if (!PSM.isCinematic)
            {
                spectatorCameraTarget = null;
                if(playerCharacter != null)
                {
                    cameraFocalTarget = playerCharacter._GetCameraTarget();
                }
                playerCamera.UpdatePosition(cameraFocalTarget);
            }
            else
            {
                if (spectatorCameraTarget == null)
                {
                    return;
                }

                if (spectatorCameraTarget != null)
                {
                    playerCamera.UpdatePositionSmooth(spectatorCameraTarget);
                    playerCamera.UpdateRotationSmooth(spectatorCameraTarget.transform.eulerAngles);
                }
            }
        }

        // Movement
        public void ClearMovement() { }
        #endregion

        #region Checks
        private void RoutineChecks() {
            // Character Data
            if(characterDataM.currentMovementController != null) {
                if(characterDataM.currentMovementController._isInitialized) { return; }

                playerCharacter = characterDataM.currentMovementController;
                playerCharacter._Initialize(PSM);
                healthM = playerCharacter.GetComponent<Health>();

                if(checkpointM != null && checkpointM.SpawnPoint != null) {
                    playerCharacter._Teleport(checkpointM.SpawnPoint.position);
                }
            }
        }

        private void ScourScene() {
            // Checkpoint Manager
            if(checkpointM == null) {
                checkpointM = FindFirstObjectByType<CheckpointManager>();

                if(checkpointM == null) {
                    Debug.LogWarning($"[{nameof(Player)}] No CheckpointManager found in scene.", this);
                }
            }
        }
        #endregion

        #region Public Accessors
        // Cinematic Controls
        public void SetAndLoadCinematic(Transform cinematicPosition) {
            spectatorCameraTarget = cinematicPosition;
            PSM.isCinematic = true;
        }

        public void ExitCinematic() {
            PSM.isCinematic = false;
        }

        public void KillPlayer()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(activeScene.buildIndex);
        }
        #endregion
    }
}
