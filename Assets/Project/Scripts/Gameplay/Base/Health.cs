using System;
using UnityEngine;
using UnityEngine.Events;

namespace LevelDesign.Systems
{
    public class Health : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private UnityEvent onDamageTaken;
        public UnityEvent OnDamageTaken => onDamageTaken;
        [SerializeField] private UnityEvent onDeath;
        public UnityEvent OnDeath => onDeath;

        [Header("Debug")]
        [SerializeField] private float current;
        
        public float Current { get => current; private set => current = value; }
        public bool IsDead => Current <= 0f;

        private void Awake() {
            Current = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if(IsDead || amount <= 0f) { return; }
            Current = Mathf.Max(0f, Current - amount);

            if(IsDead) {
                onDeath?.Invoke();
                return;
            }
            
            onDamageTaken?.Invoke();
        }
    }
}