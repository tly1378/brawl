using UnityEngine;
using Brawl.State;
using Cysharp.Threading.Tasks;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class PlayerController : MonoBehaviour
    {
        public Controller Controller { get; private set; }

        private AgentController agentController;

        private void Awake()
        {
            Controller = GetComponent<Controller>();
            agentController = GetComponent<AgentController>();
        }

        private async void Start()
        {
            CinemachineManager.Instance.OnSwitchTarget += Instance_OnSwitchTarget;
            await UniTask.WaitUntil(() => CinemachineManager.Instance.CurrentController != null);
            enabled = CinemachineManager.Instance.CurrentController == Controller;
        }

        private void Instance_OnSwitchTarget(Controller current)
        {
            enabled = current == Controller;
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