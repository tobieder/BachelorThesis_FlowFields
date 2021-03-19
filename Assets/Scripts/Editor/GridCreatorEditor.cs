using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridCreator))]
public class GridCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridCreator gridCreator = (GridCreator)target;

        EditorGUILayout.Space(10);

        gridCreator.m_Width = EditorGUILayout.IntField("Width", gridCreator.m_Width);
        gridCreator.m_Height = EditorGUILayout.IntField("Height", gridCreator.m_Height);
        gridCreator.m_CellSize = EditorGUILayout.FloatField("Cell Size", gridCreator.m_CellSize);
    }
}
