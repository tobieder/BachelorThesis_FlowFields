using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


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
                    DrawArrow.ForDebug(new Vector3(cell.xPos, 0.0f, cell.zPos), cell.GetFlowFieldDirection(byte.MaxValue), Color.red);
                };
                break;
            default:
                break;
        };
#endif
    }
}
