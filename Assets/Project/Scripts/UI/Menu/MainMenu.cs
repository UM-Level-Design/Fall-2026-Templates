using UnityEngine;
using TinyInspector;
using QFSW.QC;
using UnityEditor;
using PurrNet;
using PurrNet.Transports;
using System;
using TMPro;

namespace LevelDesign.UI {
    public class MainMenu : MonoBehaviour {
        public enum MenuState {
            Main,
            Connect,
            HostedLobby,
            Lobby
        }

        #region Fields
        [BoxGroup("Scene Refs")]
        public RectTransform imageDivider;
        [BoxGroup("Scene Refs")]
        public RectTransform followUpDivider;

        [BoxGroup("Scene Refs/Page Refs")]
        public GameObject mainPage;
        [BoxGroup("Scene Refs/Page Refs")]
        public GameObject connectPage;
        [BoxGroup("Scene Refs/Page Refs")]
        public GameObject hostPage;
        [BoxGroup("Scene Refs/Page Refs")]
        public GameObject lobbyPage;

        [BoxGroup("Scene Refs/Network")]
        public PurrTransport transport;
        [BoxGroup("Scene Refs/Network")]
        public NetworkManager networkM;
        [BoxGroup("Scene Refs/Network")]
        public TMP_Text lobbyCode;

        [BoxGroup("Transition")]
        public float transitionDuration = 0.5f;
        [BoxGroup("Transition")]
        public float collapsedScaleY = 1f;
        [BoxGroup("Transition")]
        public float imageExpandedScaleY = 30f;
        [BoxGroup("Transition")]
        public float followUpExpandedScaleY = 14f;

        [BoxGroup("Debug")]
        public MenuState menuState = MenuState.Main;

        private float imageFromY;
        private float imageToY;
        private float followUpFromY;
        private float followUpToY;
        private float transitionTime = float.MaxValue;
        #endregion

        #region Unity Calls
        private void Update() {
            if(transitionTime > transitionDuration) { return; }

            transitionTime += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, transitionTime / transitionDuration);

            SetScaleY(imageDivider, Mathf.Lerp(imageFromY, imageToY, t));
            SetScaleY(followUpDivider, Mathf.Lerp(followUpFromY, followUpToY, t));

            switch(menuState)
            {
                case MenuState.Main:
                    mainPage.SetActive(true);
                    break;
                case MenuState.Connect:
                    connectPage.SetActive(true);
                    break;
                case MenuState.HostedLobby:
                    hostPage.SetActive(true);
                    lobbyPage.SetActive(true);
                    break;
                case MenuState.Lobby:
                    lobbyPage.SetActive(true);
                    break;
            }
        }
        #endregion

        #region Core
        private void TransitionTo(MenuState state) {
            DisableAll();

            imageFromY = imageDivider.localScale.y;
            followUpFromY = followUpDivider.localScale.y;

            bool imageExpanded = state == MenuState.Connect || state == MenuState.HostedLobby || state == MenuState.Lobby;
            imageToY = imageExpanded ? imageExpandedScaleY : collapsedScaleY;
            followUpToY = state == MenuState.Lobby || state == MenuState.HostedLobby ? followUpExpandedScaleY : collapsedScaleY;

            transitionTime = 0f;
            menuState = state;
        }

        private void DisableAll() {
            mainPage.SetActive(false);
            hostPage.SetActive(false);
            connectPage.SetActive(false);
            lobbyPage.SetActive(false);
        }

        private static void SetScaleY(RectTransform target, float y) {
            Vector3 scale = target.localScale;
            scale.y = y;
            target.localScale = scale;
        }
        
        private string RandomCodeGenerator() {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            char[] code = new char[6];

            for(int i = 0; i < code.Length; i++) {
                code[i] = chars[UnityEngine.Random.Range(0, chars.Length)];
            }

            lobbyCode.text = new string(code);
            return new string(code);
        }
        #endregion

        #region Public Accessors
        [Command("debug_ui_menu_transitionmain")]
        public void TransitionToMain() {
            TransitionTo(MenuState.Main);
        }

        [Command("debug_ui_menu_transitionconnect")]
        public void TransitionToConnect() {
            TransitionTo(MenuState.Connect);
        }

        [Command("debug_ui_menu_transitionhost")]
        public void TransitionToHost() {
            TransitionTo(MenuState.HostedLobby);
            transport.roomName = RandomCodeGenerator();
            // Start Server
            // Start Client
        }

        [Command("debug_ui_menu_transitionlobby")]
        public void TransitionToLobby() {
            TransitionTo(MenuState.Lobby);
        }
        public void CloseConnection()
        {
            
        }
        #endregion
    }
}