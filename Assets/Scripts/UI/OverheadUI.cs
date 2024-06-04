using System;
using UnityEngine;

namespace Brawl.UI
{
    public class OverheadUI : MonoBehaviour
    {
        private static new Camera camera;
        private Transform target;
        [SerializeField] private HPBar hpBar;
        [SerializeField] private StateBar stateBar;
        [SerializeField] private TextBox textBox;

        private void Awake()
        {
            if (camera == null) camera = Camera.main;
        }

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 screenPosition = camera.WorldToScreenPoint(target.position);
            transform.position = screenPosition;
        }

        internal void Set(AgentController agent)
        {
            target = agent.UIPosition;
            stateBar.Set(agent);
            hpBar.Set(agent.Health);
            textBox.Set(agent);
        }

        internal void Set(PlayerController player)
        {
            target = player.UIPosition;
            stateBar.gameObject.SetActive(false);
            hpBar.Set(player.Health);
            textBox.Set(player);
        }
    }
}
