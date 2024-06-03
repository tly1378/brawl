using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIAdjuster))]
public class AIAdjusterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AIAdjuster myScript = (AIAdjuster)target;
        if (GUILayout.Button("Adjuster"))
        {
            myScript.RequestForAdjustment();
        }
    }
}
