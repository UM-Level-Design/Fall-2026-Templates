using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "EventDamagePlayer", menuName = "ScriptableObjects/Events/Player/DamagePlayer", order = 2)]
    public class DamagePlayerEventChannelSO : ScriptableObject
    {
        public event Action OnDamagePlayer;

        public void RaiseEvent()
        {
            OnDamagePlayer?.Invoke();
        }
    }
}