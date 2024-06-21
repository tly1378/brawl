using UnityEngine;
using ChatGPT;
using ChatGPT.JsonModels;
using Newtonsoft.Json.Linq;
using Cysharp.Text;
using System.Reflection;
using System;
using Brawl.State;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace Brawl.AI
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Chatbot))]
    public class ChatbotEditor : UnityEditor.Editor
    {
        private string textFieldValue = string.Empty;
        private string stateName = string.Empty;

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

            stateName = UnityEditor.EditorGUILayout.TextField("动态状态", stateName);
            if (GUILayout.Button("加载"))
            {
                if (target is Chatbot chatbot && !string.IsNullOrEmpty(stateName))
                {
                    string result = chatbot.LoatState(stateName);
                    UnityEngine.Debug.Log(result);
                }
            }
            if (GUILayout.Button("切换"))
            {
                if (target is Chatbot chatbot && !string.IsNullOrEmpty(stateName))
                {
                    chatbot.ChangeState(stateName);
                }                
            }
        }
    }
#endif
    public class Chatbot : MonoBehaviour
    {
        private readonly Session session = new("gpt-4o");
        [SerializeField] private AgentController agent;

        public AgentController Agent => agent;

        private void Awake()
        {
            ECA.ECA.SetMethods(ECA.ECAMap.sMethods);
            Session.Invoke = ECA.ECA.Invoke;
            //Session.onLog = Log;

            // 逃跑：阈值、概率
            var adjEscapeFunction = Session.CreateFunction("AdjEscape", "Adjusts the escape behavior parameters for the controller based on the given threshold and probability.");
            adjEscapeFunction = Session.AddParameter(adjEscapeFunction, "threshold", "A float between 0 and 1 representing the HP threshold below which an escape attempt is triggered.", required: false);
            adjEscapeFunction = Session.AddParameter(adjEscapeFunction, "probability", "A float between 0 and 1 representing the probability of escaping when the escape condition is met.", required: false);
            
            // 巡逻：巡逻半径、追击距离
            var adjPatrolFunction = Session.CreateFunction("AdjPatrol", "Adjusts the patrol behavior parameters for the controller based on the given wander radius and maximum chase range.");
            adjPatrolFunction = Session.AddParameter(adjPatrolFunction, "wanderRadius", "A float representing the radius within which the character can wander during patrol.", required: false);
            adjPatrolFunction = Session.AddParameter(adjPatrolFunction, "maxChaseRange", "A float representing the maximum distance to chase an enemy during patrol.", required: false);

            // 切换状态：状态名
            var changeStateFunction = Session.CreateFunction("ChangeState", "Changes the state of the robot to the specified state.");
            changeStateFunction = Session.AddParameter(changeStateFunction, "state", "A string representing the state to transition to. Possible values are: 'FollowState' (following the player), 'HealState' (returning to base for healing), 'PatrolState' (wandering around, patrolling), 'GuardState' (enter PatrolState and set the patrol radius to 0). Custom statuses will not be listed.");

            //// 开发状态：功能集名、需求
            //var createStateFunction = Session.CreateFunction("CreateStates", "Create new states to meet requirements that cannot be met by the existing state. This function will call the AI ​​model dedicated to generating the states.");
            //createStateFunction = Session.AddParameter(createStateFunction, "scriptName", "The name of the script that contains the implementation of one or more states.");
            //createStateFunction = Session.AddParameter(createStateFunction, "requirement", "Functions that the state needs to implement");

            session.Tools = new JArray { adjEscapeFunction, adjPatrolFunction, changeStateFunction, /*createStateFunction*/ };
            session.Instance = this;
            session.AddMessage(Role.system, "Your answer needs to be concise and humanized. Answer in Chinese.");
        }

        private void Log(string content, Session.Log log)
        {
            switch (log)
            {
                case Session.Log.Send:
                    Debug.Log(content);
                    break;
                case Session.Log.Receive:
                    Debug.Log(content);
                    break;
                case Session.Log.Error:
                    Debug.Log(content);
                    break;
            }
        }

        internal async void Chat(string content)
        {
            var player = CinemachineManager.Instance.CurrentController;
            player.OnSpeak(content);
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
                    var target = CinemachineManager.Instance.CurrentController.transform;
                    var follow = agent.stateDict[state] as FollowState;
                    follow.Set(target);
                    break;
                case "GuardState":
                    AdjPatrol(wanderRadius: 0);
                    agent.TransitionToState(nameof(PatrolState));
                    return "The status changes to patrolling, and the wander radius has been adjusted to 0.";
            }

            agent.TransitionToState(state);
            return $"The status changes to {state}.";
        }

        [ECA.Action("CreateStates")]
        public async Task<string> CreateStates(string scriptName, string requirement)
        {
            Debug.LogFormat("开始生成新状态：{0}，{1}", scriptName, requirement);
            (var success, var log) = await CodeGenerater.TryCreateNewStates(scriptName, requirement);
            if(success)
            {
                log += "\n\n" + LoatState(scriptName);
                Debug.LogFormat("新状态生成成功：{0}", log);
                return log;
            }
            else
            {
                Debug.LogFormat("新状态生成失败：{0}", log);
                return log;
            }
        }

        public string LoatState(string scriptName)
        {
            string assemblyPath = $"D:\\Projects\\Brawl\\brawl_dynamic\\{scriptName}.dll";
            Assembly assembly = Assembly.LoadFrom(assemblyPath);

            // 获取所有继承自StateBase的类型
            var derivedTypes = assembly.GetTypes()
                                       .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(AgentState)))
                                       .ToList();

            // 实例化所有找到的类型并存储到agent.stateDict中
            string[] names = new string[derivedTypes.Count];
            for (int i = 0; i < derivedTypes.Count; i++)
            {
                Type type = derivedTypes[i];
                string typeName = type.Name;
                AgentState state = Activator.CreateInstance(type, args: agent) as AgentState;
                agent.stateDict[typeName] = state;
                names[i] = '\"' + typeName + '\"';
            }

            return $"The new state {string.Join(',', names)} was generated successfully";
        }
    }
}
