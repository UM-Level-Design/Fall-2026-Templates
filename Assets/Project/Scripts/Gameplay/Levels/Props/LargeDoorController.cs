using UnityEngine;
using TMPro;
using LevelDesign.Gameplay.Levels;
using LevelDesign.Async.Auth;
using LevelDesign.Data;

namespace LevelDesign.Gameplay.Levels
{
    public class LargeDoorController : _InputAuth
    {
        [Header("Config")]
        [SerializeField] private int maxKeysNeeded;
        [SerializeField] private float timeForDoor;

        [Header("Scene Refs")]
        [SerializeField] private AnimatorTrigger propAnimator;
        [SerializeField] private KeyCollectedEventChannelSO e_KeyCollected;
        [SerializeField] private TMP_Text keysRemainingText;
        [SerializeField] private Transform cameraPoint;
        
        [Header("Events")]
        [SerializeField] private CinematicCameraEventChannelSO e_cinematicCamera;

        private int currentKeyCount;
        private bool doorOpenRequested;
        private float doorOpenTime;

        private void OnEnable()
        {
            e_KeyCollected.OnKeyCollected += HandleKeyCollected;
        }
 
        private void OnDisable()
        {
            e_KeyCollected.OnKeyCollected -= HandleKeyCollected;
        }

        public void Update() {
            keysRemainingText.text = (maxKeysNeeded - currentKeyCount).ToString();
            if(currentKeyCount == maxKeysNeeded) {
                propAnimator.SetTrue();
                keysRemainingText.gameObject.SetActive(false);

                if(!doorOpenRequested) {
                    OpenDoorCinematic();
                }
            }

            if(doorOpenRequested && Time.time > doorOpenTime + timeForDoor) {
                if(e_cinematicCamera == null) {
                    Debug.Log("Missing Event: Cinematic Camera");
                    return;
                }
                
                e_cinematicCamera.RaiseReleaseEvent();
                InputAuthManager.Instance.RelinquishRequest(this);
            }
        }

        private void OpenDoorCinematic() {
            e_cinematicCamera.RaiseRequestEvent(cameraPoint);
            doorOpenTime = Time.time;
            InputAuthManager.Instance.RequestInput(this);
            doorOpenRequested = true;
        }

        private void HandleKeyCollected() {
            currentKeyCount++;
        }
    }
}
