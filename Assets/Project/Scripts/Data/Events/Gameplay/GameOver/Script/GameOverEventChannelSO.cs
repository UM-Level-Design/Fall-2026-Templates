using System;
using UnityEngine;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "GameOverEvent", menuName = "ScriptableObjects/Events/Gameplay/GameOver", order = 1)]
    public class GameOverEventChannelSO : ScriptableObject
    {
        public event Action OnGameOver;

        public void RaiseEvent()
        {
            OnGameOver?.Invoke();
        }
    }
}