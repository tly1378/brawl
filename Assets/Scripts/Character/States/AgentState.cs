using System;
using System.Reflection;
using UnityEngine;

namespace Brawl.State
{
    public abstract class AgentState
    {
        public AgentController Agent { get; }

        public AgentState(AgentController agent)
        {
            Agent = agent;
            Agent.Controller.OnAttributeChange += HandleAttributeChange;
        }

        ~AgentState()
        {
            Agent.Controller.OnAttributeChange -= HandleAttributeChange;
        }

        private void HandleAttributeChange(string attributeName, float newValue, float? origin)
        {
            Type type = GetType();
            PropertyInfo property = type.GetProperty(attributeName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (property != null && property.CanWrite)
            {
                try
                {
                    property.SetValue(this, newValue);
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Failed to set the value for property {0}: {1}", attributeName, ex.Message);
                }
            }
        }


        public delegate string StateChecker();

        public event StateChecker OnUpdateState;

        public virtual void EnterState() { }

        public virtual void UpdateState()
        {
            if (OnUpdateState != null)
            {
                var invocationList = OnUpdateState.GetInvocationList();
                for (int i = 0; i < invocationList.Length; i++)
                {
                    var method = (StateChecker)invocationList[i];
                    var result = method.Invoke();
                    if (!string.IsNullOrEmpty(result))
                    {
                        Agent.TransitionToState(result);
                        return;
                    }
                }
            }
        }

        public virtual void ExitState() { }
    }
}
