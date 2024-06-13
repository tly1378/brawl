using UnityEngine;
using UnityEngine.AI;

namespace Brawl.State
{
    public class PatrolState : AgentState
    {
        public const float ViewRadius = 15f;
        private const float WanderInterval = 5f;
        private float wanderTimer;
        private Vector3 originalPosition;
        private readonly Collider[] hitColliders = new Collider[5];
        private float wanderRadius;
        private float maxChaseRange;

        public PatrolState(AgentController agent) : base(agent)
        {
            originalPosition = agent.transform.position;
            wanderRadius = agent.Controller.GetAttribute("WanderRadius") ?? 10f;
            maxChaseRange = agent.Controller.GetAttribute("MaxChaseRange") ?? 15f;

            // 每帧都会执行的事件，当函数返回状态名时切换到对应的状态
            OnUpdateState += CheckHPToHeal;
            OnUpdateState += CheckEnemyToChase;
        }

        // 可重载：进入状态时调用
        public override void EnterState()
        {
            wanderTimer = WanderInterval;
            Agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        // 检查是否需要治疗
        private string CheckHPToHeal(AgentState currentState)
        {
            var health = currentState.Agent.Controller.Health;
            return health.CurrentHealth < health.MaxHealth ? nameof(HealState) : null;
        }

        // 检查是否有敌人需要追击
        private string CheckEnemyToChase(AgentState currentState)
        {
            if (currentState is not PatrolState patrolState)
            {
                Debug.LogError("CheckEnemy can only work during PatrolState.");
                return null;
            }

            int count = Physics.OverlapSphereNonAlloc(currentState.Agent.transform.position, Mathf.Min(patrolState.maxChaseRange, ViewRadius), patrolState.hitColliders);
            for (int i = 0; i < count; i++)
            {
                var controller = patrolState.hitColliders[i].GetComponent<Controller>();
                if (controller != null && controller.FactionId != currentState.Agent.Controller.FactionId)
                {
                    (currentState.Agent.stateDict[nameof(ChaseState)] as ChaseState).Set(controller, patrolState.maxChaseRange);
                    return nameof(ChaseState);
                }
            }
            return null;
        }

        // 可重载：每帧调用
        public override void UpdateState()
        {
            base.UpdateState();
            if (wanderRadius <= 0) return;

            wanderTimer += Time.deltaTime;
            if (wanderTimer >= WanderInterval && Vector3.Distance(Agent.Controller.Agent.destination, Agent.transform.position) < 1f)
            {
                Agent.Controller.Agent.SetDestination(RandomNavSphere(originalPosition, wanderRadius));
                wanderTimer = 0;
            }
        }

        // 可重载：退出状态时调用
        public override void ExitState()
        {
            Agent.Controller.OnAttributeChange -= HandleAttributeChange;
        }

        // 获取随机导航点
        private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask = -1)
        {
            var randDirection = Random.insideUnitSphere * dist + origin;
            NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
            return navHit.position;
        }

        // 处理属性变化（Agent.Controller.SetAttribute）
        private void HandleAttributeChange(string name, float value, float? origin)
        {
            if (name == "WanderRadius") wanderRadius = value;
            if (name == "MaxChaseRange") maxChaseRange = value;
        }
    }
}
