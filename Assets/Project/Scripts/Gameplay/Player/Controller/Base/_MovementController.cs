using UnityEngine;
using LevelDesign.Async.Auth;
using LevelDesign.Data;
using TinyInspector;

// Summary:
// A base movement class that allows for us to have any controller exist and work, as long as we follow base implementation.

namespace LevelDesign.Systems.Player
{
    public enum Stance { Stand, Crouch, Slide, Air }

    public abstract class _MovementController : _InputAuth
    {
        [BoxGroup("_MovementController/Scene Refs")]
        public Transform _visualSpawnPoint;
        [Space]
        [BoxGroup("_MovementController/Scene Refs")]
        public CharacterDataSO _characterData;

        [BoxGroup("_MovementController/Debug")]
        public bool _isInitialized;
        [BoxGroup("_MovementController/Debug")]
        public Stance stanceMirror;

        // Must inherit for basic controls
        public abstract void _Initialize(PlayerStateMachine psm, CharacterDataSO characterdata);
        public abstract void _RemoteInit();
        public abstract void _UpdateBody(float deltaTime, Transform playerCam);

        // Visuals Spawn 
        public abstract void _SpawnVisuals();

        // Helpers
        public abstract Transform _GetCameraTarget();
        public abstract RigInfo _GetCurrentRigInfo();

        // Movement state for view model / camera effects (bob, air tilt, landing).
        // Override to return the body's real world-space velocity.
        public virtual Vector3 _GetVelocity() => Vector3.zero;
        // Speed (m/s) that counts as "full walk" for view model bob. Return 0 to let the weapon's SO decide.
        public virtual float _GetWalkSpeed() => 0f;
        
        // Level Design Calls
        public abstract void _Teleport(Vector3 position);
        public abstract void _SetRotation(Quaternion rotation);
    }
}