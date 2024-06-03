using TMPro;
using UnityEngine;

public class StateBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private static new Camera camera;
    private AgentController agent;

    public void Init(AgentController agent)
    {
        this.agent = agent;
        agent.OnStateChange += OnStateChange;
        camera = camera != null ? camera : Camera.main;
        agent.Health.OnDead += OnAgentDead;
        agent.Health.OnRespawn += OnAgentRespawn;
    }

    private void OnAgentRespawn()
    {
        gameObject.SetActive(true);
    }

    private void OnAgentDead()
    {
        gameObject.SetActive(false);
    }

    private void OnStateChange(AgentState newState)
    {
        text.SetText($"({newState})");
    }

    private void LateUpdate()
    {
        if (agent == null) return;
        Vector3 screenPosition = camera.WorldToScreenPoint(agent.UIPosition.position);
        transform.position = screenPosition;
    }
}
