using UnityEngine;
using UnityEngine.InputSystem;
using LevelDesign.Async.Auth;
using LevelDesign.Gameplay.Levels;

// Summary
// An extension of the movement controller to handle top down / 2.5d logic

namespace LevelDesign.Systems.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class TDSController : _MovementController
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float turnSpeed = 720f; // deg/sec
        [SerializeField] private float maxSlopeAngle = 40f;
        [SerializeField] private float lowestYPoint = -50f;

        [Header("Ledge Guard")]
        [SerializeField] private float probeDistance = 0.4f;
        [SerializeField] private float maxDropHeight = 0.6f;
        [SerializeField] private LayerMask groundMask = ~0; // walkable layers
        [SerializeField] private bool debugDrawProbes = false;

        [Header("Camera")]
        [SerializeField] private Transform cameraTarget;

        private Rigidbody rb;
        private Collider col;
        private Camera cam;

        private Vector3 moveDir;
        private Vector3 lookDir;

        private bool isGrounded;
        private bool onUnapprovedSurface;
        private Vector3 groundNormal = Vector3.up;

        private Vector3 initializedLocation;
        private Vector3? lastSafeTeleport;
        private CheckpointManager checkpointManager;

        public override void _Initialize(PlayerStateMachine psm)
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;

            col = GetComponent<Collider>();
            cam = Camera.main;

            aInputInit(autoPopulateGame: true);
            InputAuthManager.Instance.RequestInput(this);
            CursorStateManager.Instance.RequestUnlock(this);

            initializedLocation = transform.position;

            _isInitialized = true;

            if(cameraTarget == null) { cameraTarget = transform; }
        }

        public override void _UpdateBody(float deltaTime)
        {
            if(transform.position.y <= lowestYPoint) {
                Respawn();
                return;
            }

            if(!_inputAuthorized)
            {
                moveDir = Vector3.zero;
                return;
            }

            CheckGround();
            ReadMove();
            ReadLook();
            Move(deltaTime);
            Turn(deltaTime);
        }

        private void CheckGround()
        {
            float footY = col.bounds.min.y;
            Vector3 origin = new Vector3(rb.position.x, footY + 0.1f, rb.position.z);

            isGrounded = false;
            onUnapprovedSurface = false;
            groundNormal = Vector3.up;

            int allButSelf = ~(1 << gameObject.layer);

            if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.3f, allButSelf, QueryTriggerInteraction.Ignore))
            {
                bool approvedLayer = (groundMask & (1 << hit.collider.gameObject.layer)) != 0;
                float angle = Vector3.Angle(hit.normal, Vector3.up);

                if(approvedLayer && angle <= maxSlopeAngle)
                {
                    isGrounded = true;
                    groundNormal = hit.normal;
                }
                else
                {
                    onUnapprovedSurface = true;
                }
            }

            rb.useGravity = !isGrounded;
        }

        private void ReadMove()
        {
            Vector2 input = _input.Move.ReadValue<Vector2>();
            moveDir = new Vector3(input.x, 0f, input.y);
            if(moveDir.sqrMagnitude > 1f) { moveDir.Normalize(); }
        }

        private void ReadLook()
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            Plane plane = new Plane(Vector3.up, rb.position);

            if(plane.Raycast(ray, out float dist))
            {
                Vector3 toCursor = ray.GetPoint(dist) - rb.position;
                toCursor.y = 0f;

                if(toCursor.sqrMagnitude > 0.001f) { lookDir = toCursor.normalized; }
            }
        }

        private void Move(float deltaTime)
        {
            Vector3 dir = moveDir;

            if(isGrounded && dir.sqrMagnitude > 0.0001f && !HasGroundAhead(dir))
            {
                Vector3 xOnly = new Vector3(dir.x, 0f, 0f);
                Vector3 zOnly = new Vector3(0f, 0f, dir.z);

                if(xOnly.sqrMagnitude > 0.0001f && HasGroundAhead(xOnly.normalized)) {
                    dir = xOnly;
                }
                else if(zOnly.sqrMagnitude > 0.0001f && HasGroundAhead(zOnly.normalized)) {
                    dir = zOnly;
                }
                else {
                    dir = Vector3.zero;
                }
            }

            float inputMagnitude = Mathf.Min(dir.magnitude, 1f);

            if(isGrounded)
            {
                Vector3 slopeDir = Vector3.ProjectOnPlane(dir, groundNormal);
                rb.linearVelocity = (slopeDir.sqrMagnitude > 0.0001f) ? slopeDir.normalized * moveSpeed * inputMagnitude : Vector3.zero;
            }
            else
            {
                Vector3 velocity = (dir == Vector3.zero) ? Vector3.zero : dir.normalized * moveSpeed * inputMagnitude;

                velocity.y = Mathf.Min(rb.linearVelocity.y, 0f);
                rb.linearVelocity = velocity;
            }
        }

        private void Turn(float deltaTime)
        {
            if(lookDir == Vector3.zero) { return; }

            Quaternion target = Quaternion.LookRotation(lookDir, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * deltaTime));
        }

        private bool HasGroundAhead(Vector3 dir)
        {
            float footY = col.bounds.min.y;

            float slopeAllowance = probeDistance * Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad);

            Vector3 origin = rb.position + dir * probeDistance;
            origin.y = footY + slopeAllowance + 0.1f;

            float rayLength = (slopeAllowance + 0.1f) + maxDropHeight + slopeAllowance;

            if(debugDrawProbes)
            {
                Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);
            }

            if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, GroundMaskNoSelf(), QueryTriggerInteraction.Ignore))
            {
                float drop = footY - hit.point.y;
                return drop <= maxDropHeight + slopeAllowance;
            }

            return false;
        }

        private int GroundMaskNoSelf()
        {
            return groundMask & ~(1 << gameObject.layer);
        }

        private Vector3 GetRespawnPosition()
        {
            if(checkpointManager != null && checkpointManager.SpawnPoint != null) {
                return checkpointManager.SpawnPoint.position;
            }

            if(lastSafeTeleport.HasValue) {
                return lastSafeTeleport.Value;
            }
                
            return initializedLocation;
        }

        private void Respawn()
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = GetRespawnPosition();
            rb.useGravity = true;
        }

        protected override void aOnInputDenied()
        {
            moveDir = Vector3.zero;
            rb.useGravity = true;
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        public override Transform _GetCameraTarget() { return cameraTarget; }

        public override void _Teleport(Vector3 position) { 
            rb.position = position;
            lastSafeTeleport = position;
            rb.useGravity = true;
        }

        public override void _SetRotation(Quaternion rotation) { rb.rotation = rotation; }

        void OnDestroy() {
            InputAuthManager.Instance.RelinquishRequest(this);
            CursorStateManager.Instance.RelinquishRequest(this);
        }
    }
}