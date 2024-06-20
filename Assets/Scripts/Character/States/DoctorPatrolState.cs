using System.Collections.Generic;
using UnityEngine;

namespace Brawl.State
{
    public class DoctorPatrolState : PatrolState
    {
        private readonly List<Controller> teammates = new();

        public DoctorPatrolState(AgentController agent) : base(agent)
        {
            foreach (Controller controller in CharacterManager.Instance.Controllers)
            {
                if(controller.FactionId == agent.Controller.FactionId)
                {
                    teammates.Add(controller);
                }
            }

            OnUpdateState += CheckPartnerToTreat;
        }

        private string CheckPartnerToTreat()
        {
            foreach (Controller teammate in teammates)
            {
                if (teammate.Health.IsAlive && teammate.Health.CurrentHealth < teammate.Health.MaxHealth)
                {
                    var state = Agent.stateDict[nameof(ChaseState)] as HealChaseState;
                    state.Set(teammate);
                    return nameof(ChaseState);
                }
            }

            return null;
        }
    }
}
