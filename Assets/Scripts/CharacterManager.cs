using UnityEngine;

namespace Brawl
{
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager Instance { get; private set; }
        public Controller[] Controllers { get; private set; }

        private void Awake()
        {
            Instance = this;
            Controllers = FindObjectsByType<Controller>(FindObjectsSortMode.None);
        }
    }
}
