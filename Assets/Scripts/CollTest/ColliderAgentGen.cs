using Assets.Scripts.CollTest;
using System.Collections;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ColliderAgentManager))]
[CanEditMultipleObjects]
public class ColliderAgentGen : Editor
{
    private ColliderAgentManager colliderMgr =>target as ColliderAgentManager;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("生成单位"))
        {
            colliderMgr.CreateAgent();
        }




    }
}
