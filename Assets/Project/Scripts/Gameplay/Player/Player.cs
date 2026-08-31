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
        
        [Header("Controllers")]
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [SerializeField] private _MovementController playerCharacter;

        [Header("Managers")]
        [SerializeField] private CharacterDataManager characterDataM;
        [SerializeField] private UIManager uiM;
        [Space]
        [SerializeField] private CheckpointManager checkpointM;
        public Health healthM;

        [Header("Events")]
        [SerializeField] private KillPlayerEventChannelSO e_killPlayer;
        [SerializeField] private CinematicCameraEventChannelSO e_cinematicCamera;

        private Transform cameraFocalTarget;
        private Transform spectatorCameraTarget;

        #region Unity Calls
        private void OnEnable() {
            if(e_killPlayer != null) {
                e_killPlayer.OnKillRequested += KillPlayer;
            }

            if(e_cinematicCamera != null) {
                e_cinematicCamera.OnRequestCamera += SetAndLoadCinematic;
                e_cinematicCamera.OnReleaseCamera += ExitCinematic;
            }
        }

        private void OnDisable() {
            if(e_killPlayer != null) {
                e_killPlayer.OnKillRequested -= KillPlayer;
            }

            if(e_cinematicCamera != null) {
                e_cinematicCamera.OnRequestCamera -= SetAndLoadCinematic;
                e_cinematicCamera.OnReleaseCamera -= ExitCinematic;
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
            if (playerCamera != null)
            {
                playerCamera.UpdateCameraInput();
            }
            if (playerCharacter != null && playerCamera != null)
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
                cameraFocalTarget = playerCharacter._GetCameraTarget();
                playerCamera.UpdatePosition(cameraFocalTarget);
                playerCamera.SetRotation(new Vector3(90f, 0f, 0f)); 
            }
            else if (spectatorCameraTarget != null)
            {
                playerCamera.UpdatePositionSmooth(spectatorCameraTarget);
                playerCamera.UpdateRotationSmooth(spectatorCameraTarget.eulerAngles);
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
            playerCharacter._Teleport(checkpointM.SpawnPoint.position);

            if(healthM != null) {
                healthM.ResetHealth();
            }
        }
        #endregion
    }
}
