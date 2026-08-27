using System.Collections;
using UnityEngine;
using TinyInspector;
using LevelDesign.Async.Auth;
using UnityEngine.SceneManagement;

namespace LevelDesign.Async
{
    // Summary:
    // Safety net for entering Play Mode from any scene (or loading a scene directly) without going through Bootstrap first.

    public class BootstrapLoader : MonoBehaviour
    {
        [BoxGroup("Config")]
        [SerializeField] private string bootstrapSceneName = "Bootstrap";
        [BoxGroup("Config")]
        [SerializeField] private float initTimeoutSeconds = 5f;

        private static bool _isLoading;

        private void Awake()
        {
            if(SingletonsReady())
            {
                enabled = false;
                return;
            }

            if(!_isLoading)
            {
                StartCoroutine(LoadBootstrapRoutine());
            }
        }

        private static bool SingletonsReady()
        {
            return InputAuthManager.Instance != null && CursorStateManager.Instance != null;
        }

        private IEnumerator LoadBootstrapRoutine()
        {
            _isLoading = true;

            Scene bootstrapScene = SceneManager.GetSceneByName(bootstrapSceneName);
            if(!bootstrapScene.isLoaded)
            {
                AsyncOperation loadOp = SceneManager.LoadSceneAsync(bootstrapSceneName, LoadSceneMode.Additive);

                if (loadOp == null)
                {
                    Debug.LogError("Bootstrap Scene, " + bootstrapSceneName +   " could not be loaded.");
                    _isLoading = false;
                    yield break;
                }

                yield return loadOp;
            }

            float deadline = Time.realtimeSinceStartup + initTimeoutSeconds;
            while (!SingletonsReady())
            {
                if (Time.realtimeSinceStartup > deadline)
                {
                    Debug.LogError("Bootstrap Loading timed out");

                    _isLoading = false;
                    yield break;
                }
                yield return null;
            }

            bootstrapScene = SceneManager.GetSceneByName(bootstrapSceneName);
            if(bootstrapScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(bootstrapScene);
            }

            _isLoading = false;
        }
    }
}