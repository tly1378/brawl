using UnityEngine;

namespace Brawl
{
    public class CharacterManager
    {
        public static CharacterManager Instance { get; } = new CharacterManager();
        public Controller[] Controllers { get; private set; }

        private CharacterManager()
        {
            Controllers = Object.FindObjectsByType<Controller>(FindObjectsSortMode.None);

            if (Controllers.Length == 0 || Controllers[0].Health == null)
            {
                Debug.LogError("CharacterManagerֻ���ڳ�ʼ����Ϻ󱻵���");
            }
        }
    }
}
