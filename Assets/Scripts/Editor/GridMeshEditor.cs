using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridMesh))]
public class GridMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GridMesh gridMesh = (GridMesh)target;

        if(GUILayout.Button("Update Mesh"))
        {
            gridMesh.MeshUpdate();
        }

        EditorGUILayout.Separator();

        base.OnInspectorGUI();
    }
}
