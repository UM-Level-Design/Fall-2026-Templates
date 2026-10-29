using UnityEngine;
using PurrNet;
using PurrNet.Transports;
using TinyInspector;
using Evo.UI;

namespace LevelDesign.UI
{
    public class PlayButtonController : MonoBehaviour
    {
        [BoxGroup("Scene Refs")]
        [SerializeField] private Evo.UI.Button interactableButton;
        [BoxGroup("Scene Refs")]
        [SerializeField] private NetworkManager networkM;

        private void FixedUpdate() {
            if(networkM.clientState == ConnectionState.Connected) {
                interactableButton.SetInteractable(true);
            }
            else {
                interactableButton.SetInteractable(false);
            }
        }
    }
}
