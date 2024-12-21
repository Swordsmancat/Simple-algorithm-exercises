using Assets.Scripts.QctTree;
using System.Collections;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(QcTreeTest))]
[CanEditMultipleObjects]
public class QcTreeGen : Editor
{
    private QcTreeTest qtTree =>target as QcTreeTest;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("生成八叉树"))
        {
            qtTree.GenQtree();
        }

        if (GUILayout.Button("随机插入点"))
        {
            qtTree.RandomInsert();
        }

        if (GUILayout.Button("随机大量插入点"))
        {
            qtTree.RandomInsertCount();
        }

        if (GUILayout.Button("查询"))
        {
            qtTree.TestQuery();
        }

        if (GUILayout.Button("随机范围查询"))
        {
            qtTree.TestRandomQuery();
        }


    }
}
