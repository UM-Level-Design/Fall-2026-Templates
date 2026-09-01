using System;
using UnityEngine;
using UnityEngine.Events;
using LevelDesign.Data;

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
        public float current;
        public float Max => maxHealth;
        public float Normalized => maxHealth > 0f ? Current / maxHealth : 0f;
        
        public float Current { get => current; private set => current = value; }
        public bool IsDead => Current <= 0f;

        [Header("Events")]
        [SerializeField] private CinematicCameraEventChannelSO e_cinematicCamera;

        private bool inCinematic;
        private bool ShouldTakeDamage() => !inCinematic;

        private void OnEnable() {
            if (e_cinematicCamera == null) { return; }
            e_cinematicCamera.OnRequestCamera += HandleCinematicStart;
            e_cinematicCamera.OnReleaseCamera += HandleCinematicEnd;
        }

        private void OnDisable() {
            if (e_cinematicCamera == null) { return; }
            e_cinematicCamera.OnRequestCamera -= HandleCinematicStart;
            e_cinematicCamera.OnReleaseCamera -= HandleCinematicEnd;
        }

        private void Awake() {
            Current = maxHealth;
        }

        public void TakeDamage(float amount)
        {
            if(IsDead || amount <= 0f || !ShouldTakeDamage()) { return; }
            Current = Mathf.Max(0f, Current - amount);

            if(IsDead) {
                onDeath?.Invoke();
                return;
            }
            
            onDamageTaken?.Invoke();
        }

        public void ResetHealth() {
            Current = maxHealth;
        }

        private void HandleCinematicStart(Transform _) => inCinematic = true;
        private void HandleCinematicEnd() => inCinematic = false;
    }
}