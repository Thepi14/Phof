using System.Collections;
using System.Collections.Generic;
using HabilitySystem;
using UnityEditor;
using UnityEngine;
using LangSystem;
using Unity.VisualScripting;

[CustomEditor(typeof(Language))] // 'Test' here is your component class name
public class TestUI : Editor
{
    /*public override void OnInspectorGUI()
    {
        Language test = target as Language;
        EditorGUILayout.LabelField("Description");
        foreach (var habilityInfo in test.habilityInfosList)
        {
            if (test.habilityInfosList.Contains(habilityInfo))
            {
                int index = test.habilityInfosList.IndexOf(habilityInfo);
                test.habilityInfosList[index].description = EditorGUILayout.TextArea(test.habilityInfosList[index].description);
            }
        }
    }*/
}
