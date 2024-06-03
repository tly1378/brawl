using TMPro;
using UnityEngine;

public class AttributesViewer : MonoBehaviour
{
    [SerializeField] private AgentController agent;
    [SerializeField] private TextMeshProUGUI text;

    private void Start()
    {
        agent.OnAttributeChange += OnAttributeChange;
    }

    private void OnAttributeChange(string name, float value, float? origin)
    {
        if(origin.HasValue)
        {
            text.text += $"{name}: {origin}=>{value}\n";
        }
        else
        {
            text.text += $"{name}: {value}\n";
        }
    }
}
