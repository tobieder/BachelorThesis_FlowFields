using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlowFieldDebug : MonoBehaviour
{
    public enum DisplayInfo
    {
        None,
        Cost,
        Integration,
        FlowFieldDirection
    };

    public DisplayInfo displayInfo;

    private void OnDrawGizmos()
    {
        if(GridCreator.grid == null)
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch(displayInfo)
        {
            case DisplayInfo.None:
                break;
            case DisplayInfo.Cost:
                foreach(Cell cell in GridCreator.grid.GetGridArray())
                {
                    Handles.Label(new Vector3(cell.xPos, 0.0f, cell.zPos), cell.GetCost().ToString(), style);
                };
                break;
            case DisplayInfo.Integration:
                foreach (Cell cell in GridCreator.grid.GetGridArray())
                {
                    Handles.Label(new Vector3(cell.xPos, 0.0f, cell.zPos), cell.GetBestCost().ToString(), style);
                };
                break;
            default:
                break;
        };
    }
}
