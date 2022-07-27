using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WPMF;

#if UNITY_EDITOR
[CustomEditor(typeof(EventSO))]
public class EventSOEditor : Editor
{
    SerializedProperty areaNamesProperty;

    private void OnEnable() {
        areaNamesProperty = serializedObject.FindProperty("AreaNames");
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        EventSO eventSOScript = (EventSO)target;
        serializedObject.Update();


        if (eventSOScript.Area == Area.Continental || eventSOScript.Area == Area.Subregional || eventSOScript.Area == Area.Local)
            EditorGUILayout.PropertyField(areaNamesProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif