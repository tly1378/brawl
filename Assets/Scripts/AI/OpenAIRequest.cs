using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Brawl.AI
{
    public static class OpenAIRequest
    {
        private const string apiUrl = "https://api.openai.com/v1/chat/completions";
        private const string apiKey = "sk-proj-tzMHHZdwzWqvLAPlv8T7T3BlbkFJD8oQ3sIiyfRTGx6rleym";

        public static async UniTask<string> CallChatGPT(string message, string attributes)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("API key is missing. Set the OPENAI_API_KEY environment variable.");
                return null;
            }

            var messages = CreateMessages(message, attributes);
            var tools = CreateTools();

            var requestBody = new JObject
            {
                { "model", "gpt-4o" },
                { "messages", JToken.FromObject(messages) },
                { "tools", JToken.FromObject(tools) }
            };

            string jsonBody = requestBody.ToString();
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

            using UnityWebRequest webRequest = new(apiUrl, "POST");
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", "Bearer " + apiKey);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            var operation = webRequest.SendWebRequest();

            while (!operation.isDone)
            {
                await UniTask.Yield();
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return webRequest.downloadHandler.text;
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }
        }

        private static List<Dictionary<string, string>> CreateMessages(string message, string attributes) => new()
        {
            new() { { "role", "system" }, { "content", "You are an AI that helps adjust parameters for a behavior tree in a MOBA game.\n" +
                    "The current properties are as follows(The absence of attributes indicates the default value.): " + attributes } },
            new() { { "role", "user" }, { "content", message } }
        };

        private static List<Dictionary<string, object>> CreateTools() => new()
        {
            new() {
                { "type", "function" },
                { "function", new Dictionary<string, object>
                    {
                        { "name", "adjust_behavior_tree_parameters" },
                        { "description", "Adjust the parameters for a behavior tree of robot in a MOBA game" },
                        { "parameters", new Dictionary<string, object>
                            {
                                { "type", "object" },
                                { "properties", new Dictionary<string, object>
                                    {
                                        { "EscapeThreshold", new Dictionary<string, string>
                                            {
                                                { "type", "number" },
                                                { "description", "The health percentage used to determine whether to escape. The higher the value, the more likely it is to escape. (decimal, 0-1). If the value passed in is 0.2, it means that when the health is less than 20%, you will consider whether you should escape." }
                                            }
                                        },
                                        { "EscapeProbability", new Dictionary<string, string>
                                            {
                                                { "type", "number" },
                                                { "description", "The probability of escaping when health is below the threshold. The higher the value, the more likely it is to escape (decimal, 0-1). If the value passed in is 0.5, it means that there is a 50% chance that you will choose to escape." }
                                            }
                                        }
                                    }
                                },
                                { "required", new List<string> { "EscapeThreshold", "EscapeProbability" } }
                            }
                        }
                    }
                }
            }
        };
    }
}