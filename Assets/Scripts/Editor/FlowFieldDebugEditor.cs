using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowFieldDebug))]
public class FlowFieldDebugEditor : Editor
{
    int buttonIndex = 0;
    public override void OnInspectorGUI()
    {
        FlowFieldDebug flowFieldDebug = (FlowFieldDebug)target;

        flowFieldDebug.displayInfo = (DisplayInfo)EditorGUILayout.EnumPopup("Display Info", flowFieldDebug.displayInfo);

        if (flowFieldDebug.displayInfo == DisplayInfo.FlowFieldDirection)
        {
            if(FlowFieldManager.Instance == null)
            {
                return;
            }

            List<string> selStrings = new List<string>();
            List<byte> usedFlowFields = FlowFieldManager.Instance.GetCurrentlyUsedFlowFields();

            if (usedFlowFields.Count > 0)
            {
                foreach (byte b in usedFlowFields)
                {
                    selStrings.Add("Layer " + b.ToString());
                }

                buttonIndex = GUILayout.SelectionGrid(buttonIndex, selStrings.ToArray(), 1);
                flowFieldDebug.flowFieldLayer = usedFlowFields[buttonIndex];
                Repaint();
            }
        }


    }
}
