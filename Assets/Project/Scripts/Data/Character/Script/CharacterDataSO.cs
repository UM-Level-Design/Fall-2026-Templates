using UnityEngine;
using LevelDesign.Systems.Player;

namespace LevelDesign.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/Character/CharacterData", order = 1)]
    public class CharacterDataSO : ScriptableObject
    {
        [Header("Controllers")]
        public GameObject GameplayController;

        [Header("Visuals")]
        public RigInfo thirdPersonVisuals;
    }
}
