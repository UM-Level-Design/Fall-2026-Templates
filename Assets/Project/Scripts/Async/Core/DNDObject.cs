using UnityEngine;

// Summary: Short for "Do Not Destroy Object"
// An alternative way to loading Singleton elements into the "Do Not Destroy"
// Allows for the quick search and discovery of scripts that try to load or aren't loading into the "Do Not Destroy"

namespace LevelDesign.Async
{
    public class DNDObject : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(this);
        }
    }
}
