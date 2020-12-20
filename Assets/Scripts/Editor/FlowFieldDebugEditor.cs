using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowFieldDebug))]
public class FlowFieldDebugEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FlowFieldDebug flowFieldDebug = (FlowFieldDebug)target;

        flowFieldDebug.displayInfo = (DisplayInfo)EditorGUILayout.EnumPopup("Display Info", flowFieldDebug.displayInfo);

        if(flowFieldDebug.displayInfo == DisplayInfo.FlowFieldDirection)
        {
            flowFieldDebug.flowFieldLayer = (byte)EditorGUILayout.IntSlider("Flow Field Layer", (int)flowFieldDebug.flowFieldLayer, 0, 255);
        }
    }
}
