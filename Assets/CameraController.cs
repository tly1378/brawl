using UnityEngine;

namespace Brawl
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 10.0f;
        public float rotationSpeed = 100.0f;

        void Update()
        {
            // Q和E键控制水平旋转
            float yaw = 0.0f;

            if (Input.GetKey(KeyCode.Q))
            {
                yaw -= rotationSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.E))
            {
                yaw += rotationSpeed * Time.deltaTime;
            }

            var rotation = transform.eulerAngles;
            rotation.y += yaw;
            transform.eulerAngles = rotation;

            // 键盘控制水平移动
            float horizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A, D 或 左, 右
            float vertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W, S 或 上, 下

            Vector3 move = transform.right * horizontal + transform.forward * vertical;
            transform.position += new Vector3(move.x, 0, move.z); // 仅水平移动

            // 键盘控制垂直移动
            if (Input.GetKey(KeyCode.Space)) // 空格键上升
            {
                transform.position += moveSpeed * Time.deltaTime * Vector3.up;
            }
            if (Input.GetKey(KeyCode.LeftControl)) // Ctrl键下降
            {
                transform.position += moveSpeed * Time.deltaTime * Vector3.down;
            }
        }
    }
}