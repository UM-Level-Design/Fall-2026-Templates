using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "EventKillPlayer", menuName = "ScriptableObjects/Events/Player/KillPlayer", order = 1)]
    public class KillPlayerEventChannelSO : ScriptableObject
    {
        public event Action OnKillRequested;

        public void RaiseEvent()
        {
            OnKillRequested?.Invoke();
        }
    }
}