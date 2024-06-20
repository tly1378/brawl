using UnityEngine;
using Brawl.State;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Player { get; private set; }

        public Controller Controller { get; private set; }

        private AgentController agentController;

        private void Awake()
        {
            Player = this;
            Controller = GetComponent<Controller>();
            agentController = GetComponent<AgentController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (agentController.CurrentState is PlayerState)
                {
                    agentController.TransitionToState(nameof(PatrolState));
                }
                else
                {
                    agentController.TransitionToState(nameof(PlayerState));
                }
            }
        }
    }
}