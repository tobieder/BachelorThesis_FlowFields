using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum DisplayInfo
{
    None,
    Cost,
    Integration,
    FlowFieldDirection
};

public class FlowFieldDebug : MonoBehaviour
{

    public DisplayInfo displayInfo;

    public byte flowFieldLayer = 0;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
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
            case DisplayInfo.FlowFieldDirection:
                foreach (Cell cell in GridCreator.grid.GetGridArray())
                {
                    DrawArrow.ForDebug(new Vector3(cell.xPos, 0.0f, cell.zPos), cell.GetFlowFieldDirection(flowFieldLayer), Color.red);
                };
                break;
            default:
                break;
        };
#endif
    }
}
