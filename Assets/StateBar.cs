using TMPro;
using UnityEngine;

public class StateBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private static new Camera camera;
    private EnemyController enemyController;

    public void Init(EnemyController enemyController)
    {
        this.enemyController = enemyController;
        enemyController.OnStateChange += EnemyController_OnStateChange;
    }

    private void EnemyController_OnStateChange(EnemyState newState)
    {
        text.SetText($"({newState})");
    }

    private void Awake()
    {
        camera = camera != null ? camera : Camera.main;
    }

    private void LateUpdate()
    {
        if (enemyController == null) return;
        Vector3 screenPosition = camera.WorldToScreenPoint(enemyController.UIPosition.position);
        transform.position = screenPosition;
    }
}
