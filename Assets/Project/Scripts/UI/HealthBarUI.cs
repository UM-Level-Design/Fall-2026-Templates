using UnityEngine;
using DTT.UI.ProceduralUI;
using LevelDesign.Systems;

namespace LevelDesign.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Scene Refs")]
        [SerializeField] private RoundedImage healthBar;
        [SerializeField] private Health healthComponent;

        private void FixedUpdate() {
            transform.LookAt(Camera.main.transform.position, Vector3.up);

            healthBar.fillAmount = Mathf.Clamp01((float)healthComponent.current / healthComponent.Max);
        }
    }
}
