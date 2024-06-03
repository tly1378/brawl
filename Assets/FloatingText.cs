using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float duration = 2.0f;
    [SerializeField] private float angleRange = 45.0f; // 角度范围

    private float timer;
    private Vector3 moveDirection;

    public void SetText(string message)
    {
        text.text = message;
        timer = 0;

        // 生成随机角度
        float angle = Random.Range(-angleRange / 2, angleRange / 2);
        float radians = angle * Mathf.Deg2Rad;

        // 计算移动方向
        moveDirection = new Vector3(Mathf.Sin(radians), Mathf.Cos(radians), 0);
    }

    void Update()
    {
        // 移动文本
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        // 计算透明度
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1.0f, 0.0f, timer / duration);

        // 设置文本颜色（包括透明度）
        Color color = text.color;
        color.a = alpha;
        text.color = color;

        // 完全透明后销毁对象
        if (alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
