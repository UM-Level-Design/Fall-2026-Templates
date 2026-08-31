using UnityEngine;
using MagicPigGames;

namespace LevelDesign.Systems.Player
{
    public class UIManager : MonoBehaviour
    {
        [Header("Scene Refs")]
        [SerializeField] private Player player;
        [SerializeField] private ProgressBar healthBar;
        [Space]
        [SerializeField] private Health healthM;

        private void Update() {
            if(player.healthM == null) { 
                healthBar.gameObject.SetActive(false);
                return; 
            }
            else {
                healthBar.gameObject.SetActive(true);
            }

            healthBar.SetProgress(player.healthM.Normalized);
        }

    }
}
