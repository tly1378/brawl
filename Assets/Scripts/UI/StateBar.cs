using Brawl.State;
using TMPro;
using UnityEngine;

namespace Brawl
{
    public class StateBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private AgentController agent;

        public void Set(AgentController agent)
        {
            this.agent = agent;
            agent.OnStateChange += OnStateChange;
            agent.Health.OnDead += OnAgentDead;
            agent.Health.OnRespawn += OnAgentRespawn;
        }

        public void Set(string text)
        {
            this.text.SetText($"({text})");
        }

        private void OnAgentRespawn()
        {
            gameObject.SetActive(true);
        }

        private void OnAgentDead(Health _)
        {
            gameObject.SetActive(false);
        }

        private void OnStateChange(AgentState newState)
        {
            text.SetText($"({newState})");
        }

        private void OnDestroy()
        {
            if (agent)
            {
                agent.OnStateChange -= OnStateChange;
                agent.Health.OnDead -= OnAgentDead;
                agent.Health.OnRespawn -= OnAgentRespawn;
            }
        }
    }
}