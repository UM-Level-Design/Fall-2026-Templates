using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "KeyCollectedEvent", menuName = "ScriptableObjects/Events/Keys", order = 1)]
    public class KeyCollectedEventChannelSO : ScriptableObject
    {
        public event Action OnKeyCollected;

        public void RaiseEvent()
        {
            OnKeyCollected?.Invoke();
        }
    }
}