using UnityEngine;
using MagicPigGames;
using LevelDesign.Data;
using TMPro;

namespace LevelDesign.Systems.Player
{
    public class UIManager : MonoBehaviour
    {
        [Header("Scene Refs")]
        [SerializeField] private Player player;
        [SerializeField] private ProgressBar healthBar;
        [SerializeField] private TMP_Text keyCounter;
        [SerializeField] private GameObject gameplayUI;
        [SerializeField] private GameObject gameOverUI;
        [Space]
        [SerializeField] private Health healthM;

        [Header("Events")]
        [SerializeField] private KeyCollectedEventChannelSO e_keyCollected;
        [SerializeField] private GameOverEventChannelSO e_gameOver;
        // Game Over

        private int keyCount;
        private bool gameOver;

        private void OnEnable() {
            if (e_keyCollected == null) { return; }
            e_keyCollected.OnKeyCollected += HandleKeyCollected;
            e_gameOver.OnGameOver += HandleGameOver;
        }

        private void OnDisable() {
            if (e_keyCollected == null) { return; }
            e_keyCollected.OnKeyCollected -= HandleKeyCollected;
            e_gameOver.OnGameOver -= HandleGameOver;
        }

        private void Update() {
            if(!gameOver) {
                if(player.healthM == null) { 
                    healthBar.gameObject.SetActive(false);
                    return; 
                }
                else {
                    healthBar.gameObject.SetActive(true);
                }
            
                healthBar.SetProgress(player.healthM.Normalized);

                keyCounter.text = keyCount + "/3";
            }
            else {
                gameOverUI.SetActive(true);
                gameplayUI.SetActive(false);
            }
        }

        private void HandleKeyCollected() => keyCount++;
        private void HandleGameOver() => gameOver = true;
    }
}
