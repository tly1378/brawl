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
            agent.Controller.Health.OnDead += OnAgentDead;
            agent.Controller.Health.OnRespawn += OnAgentRespawn;
        }

        public void Set(string text)
        {
            this.text.SetText(text);
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
            string newText = newState.ToString();
            //Debug.LogFormat("{0} => {1}", text.text, newText);
            text.text = newText;
        }

        private void OnDestroy()
        {
            if (agent)
            {
                agent.OnStateChange -= OnStateChange;
                agent.Controller.Health.OnDead -= OnAgentDead;
                agent.Controller.Health.OnRespawn -= OnAgentRespawn;
            }
        }
    }
}