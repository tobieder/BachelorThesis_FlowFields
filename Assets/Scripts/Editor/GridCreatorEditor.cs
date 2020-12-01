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

        gridCreator.useFile = GUILayout.Toggle(gridCreator.useFile, "Use File");

        if(!gridCreator.useFile)
        {
            gridCreator.defaultFlowFieldDirection = EditorGUILayout.Vector3Field("Default Flow Field Direction", gridCreator.defaultFlowFieldDirection);
        }

        EditorGUILayout.Space(10);

        gridCreator.width = EditorGUILayout.IntField("Width", gridCreator.width);
        gridCreator.height = EditorGUILayout.IntField("Height", gridCreator.height);
        gridCreator.cellSize = EditorGUILayout.FloatField("Cell Size", gridCreator.cellSize);


    }
}
