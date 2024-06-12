using UnityEngine;
using ChatGPT;
using ChatGPT.JsonModels;
using Newtonsoft.Json.Linq;
using Cysharp.Text;
using System.Reflection;
using System;
using Brawl.State;
using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.IO;

namespace Brawl.AI
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Chatbot))]
    public class ChatbotEditor : UnityEditor.Editor
    {
        private string textFieldValue = string.Empty;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            textFieldValue = UnityEditor.EditorGUILayout.TextField("对话框", textFieldValue);
            if (GUILayout.Button("发送"))
            {
                if (target is Chatbot chatbot && !string.IsNullOrEmpty(textFieldValue))
                {
                    chatbot.Chat(textFieldValue);
                    textFieldValue = string.Empty;
                }
            }
        }
    }
#endif
    public class Chatbot : MonoBehaviour
    {
        private readonly Session session = new("gpt-4o");
        [SerializeField] private AgentController agent;

        private void Awake()
        {
            ECA.ECA.SetMethods(ECA.ECAMap.sMethods);
            Session.Invoke = ECA.ECA.Invoke;
            Session.onLog = Log;

            var adjEscapeFunction = Session.CreateFunction("AdjEscape", "Adjusts the escape behavior parameters for the controller based on the given threshold and probability.");
            adjEscapeFunction = Session.AddParameter(adjEscapeFunction, "threshold", "A float between 0 and 1 representing the HP threshold below which an escape attempt is triggered.", required: false);
            adjEscapeFunction = Session.AddParameter(adjEscapeFunction, "probability", "A float between 0 and 1 representing the probability of escaping when the escape condition is met.", required: false);
            
            var adjPatrolFunction = Session.CreateFunction("AdjPatrol", "Adjusts the patrol behavior parameters for the controller based on the given wander radius and maximum chase range.");
            adjPatrolFunction = Session.AddParameter(adjPatrolFunction, "wanderRadius", "A float representing the radius within which the character can wander during patrol.", required: false);
            adjPatrolFunction = Session.AddParameter(adjPatrolFunction, "maxChaseRange", "A float representing the maximum distance to chase an enemy during patrol.", required: false);

            var changeStateFunction = Session.CreateFunction("ChangeState", "Changes the state of the robot to the specified state.");
            changeStateFunction = Session.AddParameter(changeStateFunction, "state", "A string representing the state to transition to. Possible values are: 'FollowState' (following the player), 'HealState' (returning to base for healing), 'PatrolState' (wandering around, patrolling), 'GuardState' (enter PatrolState and set the patrol radius to 0). Custom statuses will not be listed.");

            var createStateFunction = Session.CreateFunction("CreateState", "Create a new state to meet requirements that cannot be met by the existing state. This function will call the AI ​​model dedicated to generating the state.");
            createStateFunction = Session.AddParameter(createStateFunction, "stateName", "The name of the new state cannot be the same as the existing state.");
            createStateFunction = Session.AddParameter(createStateFunction, "requirement", "Functions that the state needs to implement");

            session.Tools = new JArray { adjEscapeFunction, adjPatrolFunction, changeStateFunction, createStateFunction };
            session.Instance = this;
        }

        private void Log(string content, Session.Log log)
        {
            switch (log)
            {
                case Session.Log.Send:
                    UnityEngine.Debug.Log(content);
                    break;
                case Session.Log.Receive:
                    UnityEngine.Debug.Log(content);
                    break;
                case Session.Log.Error:
                    UnityEngine.Debug.Log(content);
                    break;
            }
        }

        internal async void Chat(string content)
        {
            PlayerController.Player.Controller.OnSpeak(content);
            session.AddMessage(Role.user, content);
            await session.FetchAsync();
            agent.Controller.OnSpeak(session.LastMessage?.Content);
        }

        [ECA.Action("AdjEscape")]
        public string AdjEscape(float threshold = -1f, float probability = -1f)
        {
            bool hasChanges = false;
            var report = ZString.CreateStringBuilder();

            if (threshold >= 0)
            {
                agent.Controller.SetAttribute("EscapeThreshold", threshold);
                report.AppendFormat("When HP falls below {0}%, an escape attempt will be triggered;", threshold * 100);
                hasChanges = true;
            }

            if (probability >= 0)
            {
                agent.Controller.SetAttribute("EscapeProbability", probability);
                report.AppendFormat("When determining whether to escape, there is a {0}% chance of escaping;", probability * 100);
                hasChanges = true;
            }

            return hasChanges ? report.ToString() : "No parameters were modified";
        }

        [ECA.Action("AdjPatrol")]
        public string AdjPatrol(float wanderRadius = -1, float maxChaseRange = -1)
        {
            bool hasChanges = false;
            var report = ZString.CreateStringBuilder();

            if (wanderRadius >= 0)
            {
                agent.Controller.SetAttribute("WanderRadius", wanderRadius);
                report.AppendFormat("During patrol, the character will not move beyond a {0}-meter range.", wanderRadius);
                hasChanges = true;
            }

            if (maxChaseRange >= 0)
            {
                agent.Controller.SetAttribute("MaxChaseRange", maxChaseRange);
                report.AppendFormat("When patrolling, chase the enemy up to {0} meters.", maxChaseRange);
                hasChanges = true;
            }

            return hasChanges ? report.ToString() : "No parameters were modified";
        }

        [ECA.Action("ChangeState")]
        public string ChangeState(string state)
        {
            if (!agent.Controller.Health.IsAlive)
            {
                return "You are currently in a state of death. You can only modify your status after resurrection.";
            }

            switch (state)
            {
                case nameof(FollowState):
                    (agent.stateDict[state] as FollowState).Set(PlayerController.Player.transform);
                    break;
                case "GuardState":
                    AdjPatrol(wanderRadius: 0);
                    agent.TransitionToState(state);
                    return "The status changes to patrolling, and the wander radius has been adjusted to 0.";
            }

            agent.TransitionToState(state);
            return $"The status changes to {state}.";
        }

        const string EXE_PATH = "D:\\Projects\\Brawl\\brawl_asm\\AsmTool\\bin\\Debug\\net8.0\\AsmTool.exe";
        const string CODE_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.cs";
        const string DLL_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.dll";

        [ECA.Action("CreateState")]
        public async Task<string> CreateState(string stateName, string requirement)
        {
            string codePath = ZString.Format(CODE_PATH, stateName);
            string code = $@"namespace Brawl.State
            {{
                /* {requirement} */
                public class {stateName} : AgentState
                {{
                    public {stateName}(AgentController agent) : base(agent)
                    {{
                    }}
                }}
            }}";
            File.WriteAllText(codePath, code);

            string dllPath = ZString.Format(DLL_PATH, stateName);
            ProcessStartInfo startInfo = new()
            {
                FileName = EXE_PATH,
                Arguments = ZString.Concat(codePath, " ", dllPath)
            };
            using (Process process = Process.Start(startInfo))
            {
                await UniTask.RunOnThreadPool(process.WaitForExit);
                int exitCode = process.ExitCode;
            }

            string assemblyPath = $"D:\\Projects\\Brawl\\brawl_dynamic\\{stateName}.dll";
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            Type type = assembly.GetType($"Brawl.State.{stateName}");
            AgentState state = Activator.CreateInstance(type, args: agent) as AgentState;
            agent.stateDict[stateName] = state;
            return $"The new state \"{stateName}\" was generated successfully";
        }
    }
}
