using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "CinemaCameraEvent", menuName = "ScriptableObjects/Events/Player/CinemaCamera", order = 3)]
    public class CinematicCameraEventChannelSO : ScriptableObject
    {
        public event Action<Transform> OnRequestCamera;
        public event Action OnReleaseCamera;

        public void RaiseRequestEvent(Transform cameraPos)
        {
            OnRequestCamera?.Invoke(cameraPos);
        }

        public void RaiseReleaseEvent()
        {
            OnReleaseCamera?.Invoke();
        }
    }
}