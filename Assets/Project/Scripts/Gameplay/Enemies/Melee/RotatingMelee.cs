using UnityEngine;
using LevelDesign.Systems;

namespace LevelDesign.Systems.Enemy
{
    public class RotatingMelee : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float damageInterval;

        [SerializeField] private BoxCollider hitCollider;

        private float currentAngle = 0f;
        private float lastDamageTime;

        void Awake()
        {
            if(hitCollider == null) { hitCollider = GetComponent<BoxCollider>(); }

            if(hitCollider == null)
            {
                Debug.LogWarning($"{name}: no BoxCollider assigned, damage won't be dealt.", this);
                return;
            }

            hitCollider.isTrigger = true;
        }

        void Update()
        {
            currentAngle = (currentAngle + rotationSpeed * Time.deltaTime) % 360f;
            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
            CheckHits();
        }

        void CheckHits()
        {
            if(hitCollider == null) { return; }

            Vector3 center = hitCollider.transform.TransformPoint(hitCollider.center);
            Vector3 halfExtents = Vector3.Scale(hitCollider.size, hitCollider.transform.lossyScale) * 0.5f;

            Collider[] hits = Physics.OverlapBox(center, halfExtents, hitCollider.transform.rotation);
            foreach(Collider other in hits)
            {
                if(other == hitCollider) { continue; }

                Health health = other.GetComponentInParent<Health>();
                if(health != null) { 
                    if(Time.time < lastDamageTime + damageInterval) { return; }
                    lastDamageTime = Time.time;
                    health.TakeDamage(damage);
                } 
            }
        }
    }
}