using Assets.Scripts.QuadTree;
using System.Collections;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(QTreeTest))]
[CanEditMultipleObjects]
public class QuadTreeGen : Editor
{
    private QTreeTest qtTree =>target as QTreeTest;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("生成四叉树"))
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
