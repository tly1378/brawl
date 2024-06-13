using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ChatGPT;
using ChatGPT.JsonModels;
using Cysharp.Text;
using Cysharp.Threading.Tasks;

namespace Brawl.AI
{
    internal class CodeGenerater
    {
        private static CodeGenerater Instance { get; } = new CodeGenerater();
        private readonly Session session;
        private readonly Message message;
        private const string AGENT_CONTROLLER = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\AgentController.cs";
        private const string STATE_CASE = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\States\\PatrolState.cs";
        private const string STATE_BASE = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\States\\AgentState.cs";
        private const string CONTROLLER = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\Controller.cs";

        private CodeGenerater()
        {
            session = new("gpt-4o");
            session.AddMessage(Role.system, @$"You are a code generator for a Unity project, 
used to generate the states in the state machine of the NPCs in the project.
This is the controller that controls the state switching: {File.ReadAllText(AGENT_CONTROLLER)}
This is the base class for all states: {File.ReadAllText(STATE_BASE)}
This is a case and also a real existing state in the NPC's state machine: {File.ReadAllText(STATE_CASE)}
This is the controller that stores properties and important game components: {File.ReadAllText(CONTROLLER)}
Here are some notes: 
1, The Agent.Controller.Agent field is a NavMeshAgent, please use this component to move;
2, You can get parameters by Agent.Controller.GetAttribute. Do not directly access private fields of other states;
Please ensure the code you generate is fully functional and does not require any further modifications by users or administrators.
If it's not possible to meet the requirements exactly, provide the best possible solution.
Your output will be compiled directly as code, so don't wrap your code with ```.
");
            message = session.AddMessage(Role.user, "");
        }

        const string EXE_PATH = "D:\\Projects\\Brawl\\brawl_asm\\AsmTool\\bin\\Debug\\net8.0\\AsmTool.exe";
        const string CODE_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.cs";
        const string DLL_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.dll";

        internal static async UniTask NewState(string stateName, string requirement)
        {
            Instance.message.Content = $"State name: {stateName}; requirement: {requirement}";
            await Instance.session.FetchAsync();
            string code = Instance.session.LastMessage.Content;
            Instance.session.messages.Remove(Instance.session.LastMessage);

            string codePath = ZString.Format(CODE_PATH, stateName);
            File.WriteAllText(codePath, code);
            string dllPath = ZString.Format(DLL_PATH, stateName);
            ProcessStartInfo startInfo = new()
            {
                FileName = EXE_PATH,
                Arguments = ZString.Concat(codePath, " ", dllPath)
            };
            using Process process = Process.Start(startInfo);
            await UniTask.RunOnThreadPool(process.WaitForExit);
            int exitCode = process.ExitCode;
        }
    }
}