using UnityEngine;
using LevelDesign.Systems.Player;

namespace LevelDesign.Systems.Enemy
{
    public class EnemyMovementController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 3f;
        public float stopDistance = 1f;

        [Header("Detection Ranges")]
        public float detectRange = 3f;
        public float chaseRange = 8f;

        [Header("Target Search")]
        public float searchInterval = 0.5f;

        private Transform target;
        private bool chasing;
        private float nextSearchTime;

        void Update()
        {
            if(target == null)
            {
                if(Time.time >= nextSearchTime)
                {
                    nextSearchTime = Time.time + searchInterval;
                    TryFindTarget();
                }
                return;
            }
            
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            bool inDetect = Mathf.Abs(toTarget.x) <= detectRange && Mathf.Abs(toTarget.z) <= detectRange;
            bool inChase  = Mathf.Abs(toTarget.x) <= chaseRange  && Mathf.Abs(toTarget.z) <= chaseRange;

            if(inDetect) { chasing = true; }
            else if(!inChase) { chasing = false; } 
            
            if(!chasing) { return; }
            if (toTarget.magnitude > stopDistance) {
                transform.position += toTarget.normalized * moveSpeed * Time.deltaTime;
            }
        }

        private void TryFindTarget()
        {
            _MovementController controller = FindObjectOfType<_MovementController>();
            if(controller != null) { target = controller.transform; }
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