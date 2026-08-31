using UnityEngine;
using LevelDesign.Async.Auth;
using LevelDesign.Systems;

namespace LevelDesign.Gameplay.Enemies
{
    public class TurretController : MonoBehaviour
    {
        [Header("Drops")]
        [SerializeField] private bool dropsKey;
        [SerializeField] private GameObject keyFab;

        public void KillTurret() {
            if(dropsKey) { Instantiate(keyFab, transform.position, transform.rotation);}
            Destroy(this.gameObject);
        }
    }
}
