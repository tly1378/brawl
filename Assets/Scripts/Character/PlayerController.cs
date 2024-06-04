using UnityEngine;

namespace Brawl
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(PlayerController))]
    public class PlayerControllerEditor : ControllerEditor { }
#endif

    public class PlayerController : Controller
    {
        private readonly Collider[] hitColliders = new Collider[5];

        private async void Start()
        {
            await UI.UIManager.Instance.CreateOverheadUI(this);
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Agent.SetDestination(hit.point);
                }
            }

            // 攻击最近的敌人
            int count = Physics.OverlapSphereNonAlloc(transform.position, Melee.attackRange, hitColliders);
            for (int i = 0; i < count; i++)
            {
                Collider hitCollider = hitColliders[i];
                Controller controller = hitCollider.GetComponent<Controller>();
                if (controller != null && controller.factionId != factionId)
                {
                    if(Melee.Target != controller.Health)
                    {
                        Melee.Target = controller.Health;
                        break;
                    }
                }
            }
        }
    }
}