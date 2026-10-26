using UnityEngine;
using TinyInspector;

namespace LevelDesign.Systems.Player
{
    public class Player : MonoBehaviour
    {
        [BoxGroup("State Machine")]
        [SerializeField] private PlayerStateMachine PSM;
        [BoxGroup("State Machine")]
        [SerializeField] private LocalPlayerState LPS;
        
        [BoxGroup("Controllers")]
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [BoxGroup("Controllers")]
        [SerializeField] private _MovementController playerCharacter;
        [BoxGroup("Controlelrs")]
        [SerializeField] private ArmController playerArmController;

        [BoxGroup("Managers")]
        [SerializeField] private CharacterDataManager characterDataM;

        private Transform cameraFocalTarget;
        private Transform spectatorCameraTarget;

        #region Unity Calls
        public void Initialize() {
            if(LPS.isLocalPlayer) { LocalInit(); }
            else { RemoteInit(); }
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

        #region Initializations
        private void LocalInit() {
            playerCamera.Initialize(PSM);
            characterDataM.Initialize();
        }
        private void RemoteInit() {
            playerCamera.RemoteInit();
        }
        #endregion

        #region Controllers
        // Both
        public void ProcessControllers() {
            if(playerCamera != null)
            {
                playerCamera.UpdateCameraInput();
                playerCamera.UpdateRotation();
            }
            if(playerCharacter != null && playerCamera != null)
            {
                playerCharacter._UpdateBody(Time.deltaTime, playerCamera.gameObject.transform);
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
            if(!LPS.isLocalPlayer) {
                if(playerCharacter == null) { 
                    playerCharacter = GetComponentInChildren<_MovementController>();
                    
                    if(playerCharacter != null && playerCharacter.enabled) {
                       playerCharacter._RemoteInit(); 
                    }
                }
                
                // Update the Data of the Weapon Manager
                if(playerCharacter != null) {
                    RigInfo rig = playerCharacter._GetCurrentRigInfo();
                }
            }
            
            // Controller Search Using Character Data
            if(characterDataM.currentMovementController != null && LPS.isLocalPlayer) {
                if(!characterDataM.currentMovementController._isInitialized) {
                    playerCharacter = characterDataM.currentMovementController;

                    playerCharacter._Initialize(PSM, characterDataM.currentCharacterData);
                }
            }

            // Arm Search using Character Data
            if(characterDataM.currentArmController != null && LPS.isLocalPlayer) {
                if(!characterDataM.currentArmController.isInitialized) {
                    playerArmController = characterDataM.currentArmController;
                    
                    if(playerCharacter != null)
                    {
                        playerArmController.Initialize(playerRef: this, controller: playerCharacter);
                    }
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
        #endregion
    }
}
