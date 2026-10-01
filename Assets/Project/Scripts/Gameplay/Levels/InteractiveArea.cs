using UnityEngine;

namespace LevelDesign.Systems
{
    public enum AreaType
    {
        Fire,
        InstantDamage,
        HealingZone,
        HealingPickup
    }

    [RequireComponent(typeof(BoxCollider))]
    public class InteractiveArea : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private AreaType areaType;
        [SerializeField] private float interval;
        [SerializeField] private float amount = 10f;
        
        private float lastInteraction;

        private void OnTriggerEnter(Collider other) { 
            Health healthComp = other.GetComponent<Health>();
            if(areaType == AreaType.InstantDamage) {
                healthComp.TakeDamage(amount);
            }
            if(areaType == AreaType.HealingPickup) {
                healthComp.Heal(amount);
                this.gameObject.SetActive(false);
            }
        }

        private void OnTriggerStay(Collider other) { 
            Health healthComp = other.GetComponent<Health>();
            if((lastInteraction + interval) <= Time.time && healthComp != null) { 
                switch(areaType){
                    case AreaType.Fire:
                        healthComp.TakeDamage(amount);
                        lastInteraction = Time.time;
                        break;
                    case AreaType.HealingZone:
                        healthComp.Heal(amount);
                        lastInteraction = Time.time;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}