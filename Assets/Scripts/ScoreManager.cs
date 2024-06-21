using System;
using TMPro;
using UnityEngine;

namespace Brawl.UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI blue;
        [SerializeField] private TextMeshProUGUI red;
        private int blueCount;
        private int redCount;
        private int win = 21;

        private void Start()
        {
            foreach (var controller in CharacterManager.Instance.Controllers)
            {
                controller.Health.OnDead += (h) => Health_OnDead(controller);
            }
        }

        private void Health_OnDead(Controller controller)
        {
            if (controller.FactionId == 0)
            {
                redCount++;
            }
            else
            {
                blueCount++;
            }

            UpdateText();
        }

        private void UpdateText()
        {
            blue.SetText($"{blueCount}/{win}");
            red.SetText($"{redCount}/{win}");

            if (blueCount > win || redCount > win)
            {
                blueCount = redCount = 0;
            }
        }
    }
}
