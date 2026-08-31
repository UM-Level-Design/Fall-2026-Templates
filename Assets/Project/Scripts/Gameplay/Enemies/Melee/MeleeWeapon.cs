using UnityEngine;

namespace LevelDesign.Systems.Enemy
{
    public class RotateObject : MonoBehaviour
    {
        public float rotationSpeed = 90f; // degrees per second
        public float damage = 10f;

        [SerializeField] private BoxCollider hitCollider;

        private float currentAngle = 0f;

        void Awake()
        {
            if (hitCollider == null)
            {
                hitCollider = GetComponent<BoxCollider>();
            }

            if (hitCollider == null)
            {
                Debug.LogWarning($"{name}: no BoxCollider assigned, damage won't be dealt.", this);
                return;
            }

            hitCollider.isTrigger = true;
        }

        void Update()
        {
            currentAngle += rotationSpeed * Time.deltaTime;
            currentAngle = WrapAngle(currentAngle);

            transform.rotation = Quaternion.Euler(0f, currentAngle, 0f);
        }

        void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Keeps the angle in [0, 360)
        float WrapAngle(float angle)
        {
            return (angle % 360f + 360f) % 360f;
        }
    }
}