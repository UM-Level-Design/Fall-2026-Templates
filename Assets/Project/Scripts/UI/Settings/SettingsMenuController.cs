using UnityEngine;
using TinyInspector;
using LevelDesign.Async.Auth;

namespace LevelDesign.UI
{
    public class SettingsMenuController : _InputAuth
    {
        [BoxGroup("Scene Refs")]
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
            CursorStateManager.Instance.RequestUnlock(this);
            InputAuthManager.Instance.RequestInput(this);
        }

        void CloseSettings() {
            settingsOpen = false;
            CursorStateManager.Instance.RelinquishRequest(this);
            InputAuthManager.Instance.RelinquishRequest(this);
        }
    }
}
