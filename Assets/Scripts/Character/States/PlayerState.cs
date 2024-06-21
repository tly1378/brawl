using UnityEngine;

namespace Brawl.State
{
    public class PlayerState : AgentState
    {
        private readonly Collider[] hitColliders = new Collider[5];
        private bool isFocused = false;

        public PlayerState(AgentController agent) : base(agent)
        {
            CinemachineManager.Instance.OnSwitchTarget += Instance_OnSwitchTarget;
        }

        public override void EnterState()
        {
            base.EnterState();
            isFocused = Agent.Controller == CinemachineManager.Instance.CurrentController;
        }

        private void Instance_OnSwitchTarget(Controller current)
        {
            isFocused = Agent.Controller == current;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (isFocused && Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Agent.Controller.NavAgent.SetDestination(hit.point);
                }
            }

            // 没有目标或目标太远，则寻找新目标
            if (!Agent.Controller.Attack.Target || Vector3.Distance(Agent.Controller.Attack.Target.transform.position, Agent.transform.position) > Agent.Controller.Attack.attackRange)
            {
                int count = Physics.OverlapSphereNonAlloc(Agent.transform.position, Agent.Controller.Attack.attackRange, hitColliders);
                Controller nearest = null;
                float minDistance = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    Collider hitCollider = hitColliders[i];
                    if (hitCollider.TryGetComponent<Controller>(out var target))
                    {
                        bool targetShouldBeFriend = Agent.Controller.Attack.TargetIsFriend;
                        bool targetIsFriend = target.FactionId == Agent.Controller.FactionId;
                        if (targetShouldBeFriend == targetIsFriend)
                        {
                            var distance = Vector3.Distance(target.transform.position, Agent.transform.position);
                            if (distance < minDistance)
                            {
                                nearest = target;
                                minDistance = distance;
                            }
                        }
                    }
                }

                if (nearest)
                {
                    Agent.Controller.Attack.Target = nearest.Health;
                }
            }
        }
    }
}