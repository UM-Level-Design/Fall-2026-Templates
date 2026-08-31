using UnityEngine;
using LevelDesign.Gameplay.Levels;
using LevelDesign.Systems.Player;

namespace LevelDesign.Systems.Enemy
{
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(LedgeController))]
    // Chases the nearest _MovementController within an X/Z detection box.
    public class EnemyMovementController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3f;
        [SerializeField] private float stopDistance = 1f;

        [Header("Detection Ranges")]
        [SerializeField] private float detectRange = 3f;
        [SerializeField] private float chaseRange = 8f;

        [Header("Target Search")]
        [SerializeField] private float searchInterval = 0.5f;

        [Header("Drops")]
        [SerializeField] private bool dropsKey;
        [SerializeField] private GameObject keyFab;

        private Rigidbody rb;
        private Transform target;
        private bool chasing;
        private float nextSearchTime;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            rb.constraints |= RigidbodyConstraints.FreezeRotation;
        }

        void Update()
        {
            if(target == null && Time.time >= nextSearchTime)
            {
                nextSearchTime = Time.time + searchInterval;
                TryFindTarget();
            }
        }

        void FixedUpdate()
        {
            Vector3 desiredPlanar = Vector3.zero;

            if(target != null)
            {
                Vector3 toTarget = target.position - rb.position;
                toTarget.y = 0f;

                bool inDetect = Mathf.Abs(toTarget.x) <= detectRange && Mathf.Abs(toTarget.z) <= detectRange;
                bool inChase = Mathf.Abs(toTarget.x) <= chaseRange && Mathf.Abs(toTarget.z) <= chaseRange;

                if(inDetect) { chasing = true; }
                else if(!inChase) { chasing = false; }

                if(chasing && toTarget.magnitude > stopDistance)
                {
                    desiredPlanar = toTarget.normalized * moveSpeed;
                }
            }

            rb.linearVelocity = new Vector3(desiredPlanar.x, rb.linearVelocity.y, desiredPlanar.z);
        }

        private void TryFindTarget()
        {
            _MovementController controller = FindObjectOfType<_MovementController>();
            if(controller != null) { target = controller.transform; }
        }

        public void KillMelee()
        {
            if(dropsKey) { Instantiate(keyFab, transform.position, transform.rotation); }
            Destroy(this.gameObject);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(detectRange * 2f, 0.1f, detectRange * 2f));

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(chaseRange * 2f, 0.1f, chaseRange * 2f));
        }
    }
}