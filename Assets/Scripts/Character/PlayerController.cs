using UnityEngine;

namespace Brawl
{
    [RequireComponent(typeof(Controller))]
    public class PlayerController : MonoBehaviour
    {
        private readonly Collider[] hitColliders = new Collider[5];

        public static PlayerController Player { get; private set; }

        public Controller Controller { get; private set; }

        private void Awake()
        {
            Player = this;
            Controller = GetComponent<Controller>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Controller.Agent.SetDestination(hit.point);
                    if (TryGetComponent<AgentController>(out var agent))
                    {
                        agent.TransitionToState(new State.PlayerState(agent));
                    }
                }
            }

            // 攻击最近的敌人
            int count = Physics.OverlapSphereNonAlloc(transform.position, Controller.Melee.attackRange, hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = hitColliders[i];
                Controller target = hitCollider.GetComponent<Controller>();
                if (target != null && target.FactionId != Controller.FactionId)
                {
                    if(target.Melee.Target != target.Health)
                    {
                        target.Melee.Target = target.Health;
                        break;
                    }
                }
            }
        }
    }
}