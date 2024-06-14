using UnityEngine;

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

        private void OnEnable()
        {
            if(agentController) agentController.TransitionToState(nameof(State.PlayerState));
        }

        private void OnDisable()
        {
            if (agentController) agentController.TransitionToState(nameof(State.PatrolState));
        }

        private void OnGUI()
        {

        }
    }
}