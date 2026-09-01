using UnityEngine;
using UnityEngine.SceneManagement;
using LevelDesign.Data;

namespace LevelDesign.Systems.Levels
{
    public class ResetManager : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private bool shouldReset;

        [Header("Scene Refs")]
        [SerializeField] private KillPlayerEventChannelSO e_playerKilled;

        void OnEnable() {
            if(shouldReset) {
                e_playerKilled.OnKillRequested += ResetScene;
            }
        }

        void OnDisable() {
            if(shouldReset) {
                e_playerKilled.OnKillRequested -= ResetScene;
            }
        }

        private void ResetScene()
        {
            Scene active = SceneManager.GetActiveScene();
            SceneManager.LoadScene(active.buildIndex);
        }
    }
}
