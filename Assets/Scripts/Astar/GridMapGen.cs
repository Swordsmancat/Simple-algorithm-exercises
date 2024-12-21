using System.Collections;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(GridMap))]
[CanEditMultipleObjects]
public class GridMapGen : Editor
{
    private GridMap map =>target as GridMap;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("生成地图"))
        {
            map.GenMap();
        }

        if (GUILayout.Button("清理"))
        {
            map.CleanAll();
        }

        if (GUILayout.Button("DFS"))
        {
            map.StartDFSFind();

        }

        if (GUILayout.Button("BFS"))
        {
            map.StartBFSFind();

        }

        if (GUILayout.Button("Dij"))
        {
            map.StartDijkkatraFind();

        }


        if (GUILayout.Button("AStar"))
        {
            map.StartAstarFind();

        }

        
    }
}
