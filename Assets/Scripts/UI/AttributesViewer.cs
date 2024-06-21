using TMPro;
using UnityEngine;

namespace Brawl
{
    public class AttributesViewer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            foreach(var controller in CharacterManager.Instance.Controllers)
            {
                controller.OnAttributeChange += (name, value, origin) => OnAttributeChange(name, value, origin, controller);
            }
        }

        private void OnAttributeChange(string name, float value, float? origin, Controller controller)
        {
            if (origin.HasValue)
            {
                text.text += $"[{controller}] {name}: {origin}=>{value}\n";
            }
            else
            {
                text.text += $"[{controller}] {name}: {value}\n";
            }
        }
    }
}