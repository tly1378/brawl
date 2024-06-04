using System;
using UnityEngine;
using UnityEngine.AI;

namespace Brawl
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Controller))]
    public class ControllerEditor : UnityEditor.Editor
    {
        private string textFieldValue = string.Empty;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            textFieldValue = UnityEditor.EditorGUILayout.TextField("对话框", textFieldValue);
            if (GUILayout.Button("发送"))
            {
                ((Controller)target).OnSpeak?.Invoke(textFieldValue);
            }
        }
    }
#endif

    [RequireComponent(typeof(Health), typeof(NavMeshAgent), typeof(MeleeAttack))]
    public class Controller : MonoBehaviour
    {
        public Action<string> OnSpeak;
        public Transform UIPosition;
        public int factionId;
        private Health health;
        private MeleeAttack melee;
        private NavMeshAgent agent;

        public Health Health => health;
        public MeleeAttack Melee => melee;
        public NavMeshAgent Agent => agent;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
            melee = GetComponent<MeleeAttack>();
            melee.factionId = factionId;            
        }
    }
}