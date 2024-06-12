using System.Collections.Generic;
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
                        agent.TransitionToState(nameof(State.PlayerState));
                    }
                }
            }

            // 没有目标或目标太远，则寻找新目标
            if(!Controller.Attack.Target || Vector3.Distance(Controller.Attack.Target.transform.position, transform.position) > Controller.Attack.attackRange)
            {
                int count = Physics.OverlapSphereNonAlloc(transform.position, Controller.Attack.attackRange, hitColliders);
                Controller nearest = null;
                float minDistance = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    Collider hitCollider = hitColliders[i];
                    Controller target = hitCollider.GetComponent<Controller>();
                    if (target != null && target.FactionId != Controller.FactionId)
                    {
                        var distance = Vector3.Distance(target.transform.position, transform.position);
                        if (distance < minDistance)
                        {
                            nearest = target;
                            minDistance = distance;
                        }
                    }
                }

                if (nearest)
                {
                    Controller.Attack.Target = nearest.Health;
                }
            }
        }
    }
}