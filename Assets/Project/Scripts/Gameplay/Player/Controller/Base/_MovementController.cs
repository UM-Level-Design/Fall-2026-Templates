using UnityEngine;
using LevelDesign.Async.Auth;
using LevelDesign.Data;

// Summary:
// A base movement class that allows for us to have any controller exist and work, as long as we follow base implementation.

namespace LevelDesign.Systems.Player
{
    public enum Stance { Stand, Crouch, Dash, Air }

    public abstract class _MovementController : _InputAuth
    {
        [Header("_MovementController/Scene Refs")]
        public CharacterDataSO _characterData;
        public Transform _visualSpawnPoint;

        [Header("_MovementController/Debug")]
        public bool _isInitialized;
        public Stance stanceMirror;

        // Must inherit for basic controls
        public abstract void _Initialize(PlayerStateMachine psm, CharacterDataSO characterdata);
        public abstract void _RemoteInit();
        
        public abstract void _UpdateBody(float deltaTime, Transform playerCam);
        public abstract void _UpdateInput();

        // Helpers
        public abstract Transform _GetCameraTarget();
        public abstract RigInfo _GetCurrentRigInfo();
        
        // Level Design Calls
        public abstract void _Teleport(Vector3 position);
        public abstract void _SetRotation(Quaternion rotation);
    }
}

