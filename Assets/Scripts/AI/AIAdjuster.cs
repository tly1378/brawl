using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Brawl.AI
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(AIAdjuster))]
    public class AIAdjusterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AIAdjuster myScript = (AIAdjuster)target;
            if (GUILayout.Button("Adjuster"))
            {
                myScript.RequestForAdjustment();
            }
        }
    }
#endif

    public class AIAdjuster : MonoBehaviour
    {
        [SerializeField] private AgentController agent;
        [SerializeField][TextArea] private string message;

        public async void RequestForAdjustment()
        {
            var text = await OpenAIRequest.CallChatGPT(message, agent.Controller.ShowAttributes());
            var response = JObject.Parse(text);
            Debug.Log(response);
            string arguments = (string)response["choices"][0]["message"]["tool_calls"][0]["function"]["arguments"];
            var json = JObject.Parse(arguments);
            if (json.TryGetValue("EscapeThreshold", out var threshold))
            {
                agent.Controller.SetAttribute("EscapeThreshold", (float)threshold);
            }
            if (json.TryGetValue("EscapeProbability", out var probability))
            {
                agent.Controller.SetAttribute("EscapeProbability", (float)probability);
            }
        }
    }
}