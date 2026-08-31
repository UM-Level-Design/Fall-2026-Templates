using UnityEngine;
using LevelDesign.Async.Auth;
using LevelDesign.Systems;

namespace LevelDesign.Gameplay.Enemies
{
    public class TurretController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float damage;
        [SerializeField] private float fireRate;
        [SerializeField] private float bulletVelocity;

        [Header("Scene Refs")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject projectile;

        private float lastFiredTime;

        void Update() {
            TrySpawnVFX();
        }

        void TrySpawnVFX() {
            if(firePoint == null && projectile == null) { return; }
            if(Time.time < (fireRate + lastFiredTime)) { return; }
            
            lastFiredTime = Time.time;
            GameObject vfx = Instantiate(projectile, firePoint.position, firePoint.rotation);
            vfx.GetComponent<ProjectileBehaviour>()?.Init(damage, bulletVelocity);        
        }

        public void KillTurret() {
            Destroy(this.gameObject);
        }
    }
}
