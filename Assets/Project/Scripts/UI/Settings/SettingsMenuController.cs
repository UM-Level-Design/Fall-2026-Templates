using UnityEngine;
using LevelDesign.Async.Auth;

namespace LevelDesign.UI
{
    public class SettingsMenuController : _InputAuth
    {
        [Header("Scene Refs")]
        public GameObject settingsMenuToggle;
        private bool settingsOpen;

        void Start() {
            aInputInit();
            _input = _inputActions.Gameplay;
        }

        void Update() {
            if(_input.Pause.WasPressedThisFrame()) {
                if(settingsOpen) {
                    CloseSettings();
                }
                else {
                    OpenSettings();
                }
            }
        }

        void OpenSettings() {
            settingsOpen = true;

            settingsMenuToggle.SetActive(true);

            CursorStateManager.Instance.RequestUnlock(this);
            InputAuthManager.Instance.RequestInput(this);
        }

        void CloseSettings() {
            settingsOpen = false;

            settingsMenuToggle.SetActive(false);

            CursorStateManager.Instance.RelinquishRequest(this);
            InputAuthManager.Instance.RelinquishRequest(this);
        }
    }
}
