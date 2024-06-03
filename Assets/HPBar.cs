using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    private static new Camera camera;
    [SerializeField] private Image image;
    private Health health;

    private void Awake()
    {
        camera = camera != null ? camera : Camera.main;
    }

    public void Init(Health health)
    {
        this.health = health;
        health.OnHPChange += OnHPChange;
    }

    private void OnHPChange()
    {
        SetHP(health.CurrentHealth / health.MaxHealth);
    }

    private void LateUpdate()
    {
        if (health == null) return;
        Vector3 screenPosition = camera.WorldToScreenPoint(health.UIPosition.position);
        transform.position = screenPosition;
    }

    private void SetHP(float rate)
    {
        image.fillAmount = rate;
    }
}
