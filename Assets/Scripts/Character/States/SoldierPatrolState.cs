using UnityEngine;

namespace Brawl.State
{
    public class SoldierPatrolState : PatrolState
    {
        public SoldierPatrolState(AgentController agent) : base(agent)
        {
            OnUpdateState += CheckEnemyToChase;
        }

        // 可重载：每帧调用
        public override void UpdateState()
        {
            base.UpdateState();
            if (WanderRadius <= 0) return;

            wanderTimer += Time.deltaTime;
            if (wanderTimer >= WanderInterval && Vector3.Distance(Agent.Controller.NavAgent.destination, Agent.transform.position) < 1f)
            {
                Agent.Controller.NavAgent.SetDestination(RandomNavSphere(originalPosition, WanderRadius));
                wanderTimer = 0;
            }
        }

        // 检查是否有敌人需要追击
        protected string CheckEnemyToChase()
        {
            int count = Physics.OverlapSphereNonAlloc(Agent.transform.position, Mathf.Min(MaxChaseRange, VIEW_RADIUS), hitColliders);
            for (int i = 0; i < count; i++)
            {
                var controller = hitColliders[i].GetComponent<Controller>();
                if (controller != null && controller.FactionId != Agent.Controller.FactionId)
                {
                    ChaseState state = Agent.stateDict[nameof(ChaseState)] as ChaseState;
                    state.Set(controller, MaxChaseRange);
                    return nameof(ChaseState);
                }
            }
            return null;
        }
    }
}
