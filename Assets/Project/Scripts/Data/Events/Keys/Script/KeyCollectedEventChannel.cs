using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "EventKeyCollected", menuName = "ScriptableObjects/Events/Keys", order = 3)]
    public class KeyCollectedEventChannelSO : ScriptableObject
    {
        public event Action OnKeyCollected;

        public void RaiseEvent()
        {
            OnKeyCollected?.Invoke();
        }
    }
}