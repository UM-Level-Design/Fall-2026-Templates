using UnityEngine;
using LevelDesign.Data;

namespace LevelDesign.Systems.Player
{
    public class UIManager : MonoBehaviour
    {
        [Header("Scene Refs")]
        [SerializeField] private GameObject GameOverUIObject;

        [Header("Events")]
        [SerializeField] private KillPlayerEventChannelSO e_playerkilled;

        private void Start() {
            e_playerkilled.OnKillRequested += GameOverUI;
        }

        private void OnDestroy() {
            e_playerkilled.OnKillRequested -= GameOverUI;
        }

        private void GameOverUI() {
            GameOverUIObject.SetActive(true);
        }
    }
}
