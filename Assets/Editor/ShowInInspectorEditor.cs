using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Brawl
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ShowInInspectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var targetObject = target;
            var type = targetObject.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(ShowInInspectorAttribute), false))
                {
                    var value = property.GetValue(targetObject, null);

                    if (value is int intValue)
                    {
                        int newValue = EditorGUILayout.IntField(property.Name, intValue);
                        if (newValue != intValue)
                        {
                            property.SetValue(targetObject, newValue, null);
                        }
                    }
                    else if (value is float floatValue)
                    {
                        float newValue = EditorGUILayout.FloatField(property.Name, floatValue);
                        if (newValue != floatValue)
                        {
                            property.SetValue(targetObject, newValue, null);
                        }
                    }
                    else if (value is string stringValue)
                    {
                        string newValue = EditorGUILayout.TextField(property.Name, stringValue);
                        if (newValue != stringValue)
                        {
                            property.SetValue(targetObject, newValue, null);
                        }
                    }
                    else if (value is bool boolValue)
                    {
                        bool newValue = EditorGUILayout.Toggle(property.Name, boolValue);
                        if (newValue != boolValue)
                        {
                            property.SetValue(targetObject, newValue, null);
                        }
                    }
                    // 你可以根据需要扩展更多类型的处理
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}