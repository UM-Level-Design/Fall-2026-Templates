using UnityEngine;

// Summary:
// Prevents a rigidbody from walking off ledges or onto surfaces steeper than maxSlopeAngle.

namespace LevelDesign.Gameplay.Levels
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(BoxCollider))]
    public class LedgeController : MonoBehaviour
    {
        [Header("Ledge Guard")]
        [SerializeField] private float maxDropHeight = 0.6f;
        [SerializeField] private float maxSlopeAngle = 50f;
        [SerializeField] private float pushBack = 0.02f;
        [SerializeField] private float skin = 0.05f;
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private bool debugDrawProbes = false;

        private Rigidbody rb;
        private BoxCollider box;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            box = GetComponent<BoxCollider>();
        }

        void FixedUpdate()
        {
            int mask = groundMask & ~(1 << gameObject.layer);

            Bounds b = box.bounds; // world-space AABB, rotation-proof
            Vector3 min = b.min;
            Vector3 max = b.max;
            Vector3 center = b.center;
            float tanSlope = Mathf.Tan(maxSlopeAngle * Mathf.Deg2Rad);

            Vector3[] probes =
            {
                new Vector3(min.x, min.y, min.z),
                new Vector3(max.x, min.y, min.z),
                new Vector3(min.x, min.y, max.z),
                new Vector3(max.x, min.y, max.z),
            };

            Vector3 correction = Vector3.zero;
            int offenders = 0;

            foreach(Vector3 v in probes)
            {
                float rXZ = new Vector2(v.x - center.x, v.z - center.z).magnitude;
                float slopeAllowance = rXZ * tanSlope;

                float originY = min.y + slopeAllowance + skin;
                Vector3 origin = new Vector3(v.x, originY, v.z);

                float rayLength = (originY - min.y) + slopeAllowance + maxDropHeight;

                if(debugDrawProbes) { Debug.DrawRay(origin, Vector3.down * rayLength, Color.red); }

                if(Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, mask,
                                    QueryTriggerInteraction.Ignore))
                {
                    float groundAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if(groundAngle <= maxSlopeAngle)
                    {
                        continue;
                    }
                }

                Vector3 outward = v - rb.position;
                outward.y = 0f;
                if(outward.sqrMagnitude < 0.0001f) { continue; }
                outward.Normalize();

                float outwardSpeed = Vector3.Dot(rb.linearVelocity, outward);
                if(outwardSpeed > 0f)
                {
                    rb.linearVelocity -= outward * outwardSpeed;
                }

                correction -= outward;
                offenders++;
            }

            if(offenders > 0)
            {
                Vector3 delta = Vector3.ClampMagnitude(correction / offenders, 1f) * pushBack;
                rb.MovePosition(rb.position + delta);
            }
        }
    }
}