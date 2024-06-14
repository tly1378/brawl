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
        private readonly Session session;
        private const string AGENT_CONTROLLER = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\AgentController.cs";
        private const string PATROL_STATE = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\States\\PatrolState.cs";
        private const string CHASE_STATE = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\States\\ChaseState.cs";
        private const string STATE_BASE = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\States\\AgentState.cs";
        private const string CONTROLLER = "D:\\Projects\\Brawl\\brawl_unity\\Assets\\Scripts\\Character\\Controller.cs";

        private CodeGenerater()
        {
            session = new("gpt-4o");
            session.AddMessage(Role.system, @$"You are a code generator for a Unity project, 
used to generate the states in the state machine of the NPCs in the project.
This is the controller that controls the state switching: {File.ReadAllText(AGENT_CONTROLLER)}
This is the base class for all states: {File.ReadAllText(STATE_BASE)}
This is 2 cases and also real existing states in the NPC's state machine: 
{File.ReadAllText(PATROL_STATE)}
{File.ReadAllText(CHASE_STATE)}
(However, unlike the previous examples, the state you create should not include a Set function or any external initialization methods. 
Your state needs to be self-contained and capable of running independently.)
This is the controller that stores properties and important game components: {File.ReadAllText(CONTROLLER)}

Here are some notes: 
1, The Agent.Controller.NavAgent field is a NavMeshAgent, please use this component to move;
2, You can get parameters by Agent.Controller.GetAttribute. Do not directly access private fields of other states;
3, You can get the Health component and assign to the Agent.Controller.Attack field to confirm the target of attack;
4, You can replace an existing state by creating a new one with the same type name. 
    For instance, if you want to update the patrol logic, name your new state PatrolState. 
    Just make sure to change the namespace to prevent any conflicts;
5, You can create multiple states in your code to fulfill user requirements. 
    Any class inheriting from AgentState will be included in the assembly. 
    Ensure you carefully manage the logic for state transitions;

Please ensure the code you generate is fully functional and does not require any further modifications by users or administrators.
If it's not possible to meet the requirements exactly, provide the best possible solution.
Your output will be compiled directly as code, so don't wrap your code with ```.
");
        }

        const string EXE_PATH = "D:\\Projects\\Brawl\\brawl_asm\\AsmTool\\bin\\Debug\\net8.0\\AsmTool.exe";
        const string CODE_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.cs";
        const string DLL_PATH = "D:\\Projects\\Brawl\\brawl_dynamic\\{0}.dll";

        internal static async UniTask<(bool success, string log)> TryCreateNewStates(string scriptName, string requirement)
        {
            var generater = new CodeGenerater();
            generater.session.AddMessage(Role.user, $"requirement: {requirement}");

            int retry = 3;
            while (retry-- > 0)
            {
                await generater.session.FetchAsync();
                string code = generater.session.LastMessage.Content;

                string codePath = ZString.Format(CODE_PATH, scriptName);
                File.WriteAllText(codePath, code);
                string dllPath = ZString.Format(DLL_PATH, scriptName);
                ProcessStartInfo startInfo = new()
                {
                    FileName = EXE_PATH,
                    Arguments = ZString.Concat(codePath, " ", dllPath),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                using Process process = new() { StartInfo = startInfo };
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                await UniTask.RunOnThreadPool(process.WaitForExit);
                int exitCode = process.ExitCode;

                if (exitCode == 0)
                {
                    return (true, $"Process completed successfully. Output: {output}");
                }
                else
                {
                    generater.session.AddMessage(Role.system, $"Compilation failed: {error}");
                }
            }
            
            return (false, $"Error: Compilation failed.");
        }
    }
}