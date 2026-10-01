using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Summary:
// Physics overlap trigger that fires UnityEvents on enter/exit, with optional re-arm.

namespace LevelDesign.Gameplay.Levels
{
    public class ColliderTrigger : MonoBehaviour
    {
        [Header("Detection")]
        public float detectionRadius = 1f;
        public LayerMask detectionLayers = ~0;

        [Header("Behaviour")]
        public bool reArmAfterEmpty = true;

        [Header("Unity Events")]
        public UnityEvent triggered;
        public UnityEvent untriggered;

        private readonly List<Collider> trackedColliders = new List<Collider>();
        private readonly HashSet<Collider> overlapSeenThisFrame = new HashSet<Collider>();

        public bool IsTriggered => trackedColliders.Count > 0;
        public IReadOnlyList<Collider> Colliders => trackedColliders;

        private void FixedUpdate()
        {
            PerformPhysicsOverlap();
        }

        private void PerformPhysicsOverlap()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers, QueryTriggerInteraction.Collide);

            overlapSeenThisFrame.Clear();

            foreach(Collider col in hits)
            {
                if(col == null) continue;

                overlapSeenThisFrame.Add(col);

                if(!trackedColliders.Contains(col))
                {
                    AddTracked(col);
                }
            }

            for(int i = trackedColliders.Count - 1; i >= 0; i--)
            {
                Collider col = trackedColliders[i];
                if(col == null || !overlapSeenThisFrame.Contains(col))
                {
                    RemoveTrackedAt(i);
                }
            }
        }

        private void AddTracked(Collider col)
        {
            trackedColliders.Add(col);

            if(trackedColliders.Count == 1)
            {
                triggered?.Invoke();
            }
        }

        private void RemoveTrackedAt(int index)
        {
            trackedColliders.RemoveAt(index);

            if(trackedColliders.Count == 0)
            {
                untriggered?.Invoke();

                if(!reArmAfterEmpty)
                {
                    enabled = false;
                }
            }
        }

    #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsTriggered ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    #endif
    }
}