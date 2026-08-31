using UnityEngine;
using LevelDesign.Async.Auth;

namespace LevelDesign.Gameplay.Player
{
    public class Pistol : _InputAuth
    {
        [Header("Config")]
        [SerializeField] private float fireRate;
        [SerializeField] private float bulletVelocity;

        [Header("Scene Refs")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private GameObject projectile;

        private float lastFiredTime;

        void Start() {
            aInputInit(true);
            InputAuthManager.Instance.RequestInput(this);
        }

        void Update() {
            if(_input.Fire.IsPressed() && _inputAuthorized) {
                TrySpawnVFX();
            }
        }
        void TrySpawnVFX() {
            if(firePoint == null && projectile == null) { return; }
            if(Time.time < (fireRate + lastFiredTime)) { return; }
            
            lastFiredTime = Time.time;
            GameObject vfx = Instantiate(projectile, firePoint.position, firePoint.rotation);
            vfx.GetComponent<ProjectileBehaviour>()?.Init(bulletVelocity);        
        }
    }
}
