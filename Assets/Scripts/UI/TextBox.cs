using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Brawl.UI
{
    public class TextBox : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private Controller agent;

        public void Set(Controller agent)
        {
            this.agent = agent;
            agent.OnSpeak += OnAgentSpeak;
        }

        private void OnAgentSpeak(string text)
        {
            this.text.SetText(text);
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        private void OnDestroy()
        {
            agent.OnSpeak -= OnAgentSpeak;
        }
    }
}
