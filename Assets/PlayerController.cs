using UnityEngine;
using UnityEngine.AI;

namespace Brawl
{
    public class PlayerController : MonoBehaviour
    {
        private NavMeshAgent agent;

        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            // 检测鼠标左键点击
            if (Input.GetMouseButtonDown(0))
            {
                // 创建射线
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // 检测射线是否击中地面
                if (Physics.Raycast(ray, out hit))
                {
                    // 设置NavMeshAgent的目标位置
                    agent.SetDestination(hit.point);
                }
            }
        }
    }
}