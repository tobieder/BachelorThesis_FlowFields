using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum DisplayInfo
{
    None,
    Index,
    Cost,
    Integration,
    FlowFieldDirection
};

public class FlowFieldDebug : MonoBehaviour
{

    public DisplayInfo m_DisplayInfo;

    public byte m_FlowFieldLayer = byte.MaxValue;

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if(GridCreator.s_Grid == null)
        {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch(m_DisplayInfo)
        {
            case DisplayInfo.None:
                break;
            case DisplayInfo.Index:
                foreach (Cell cell in GridCreator.s_Grid.GetGridArray())
                {
                    Handles.Label(new Vector3(cell.m_XPos, 0.0f, cell.m_ZPos), "(" + cell.m_XIndex + ", " + cell.m_ZIndex + ")", style);
                };
                break;
            case DisplayInfo.Cost:
                foreach(Cell cell in GridCreator.s_Grid.GetGridArray())
                {
                    Handles.Label(new Vector3(cell.m_XPos, 0.0f, cell.m_ZPos), cell.GetCost().ToString(), style);
                };
                break;
            case DisplayInfo.Integration:
                foreach (Cell cell in GridCreator.s_Grid.GetGridArray())
                {
                    Handles.Label(new Vector3(cell.m_XPos, 0.0f, cell.m_ZPos), cell.GetIntegration().ToString(), style);
                };
                break;
            case DisplayInfo.FlowFieldDirection:
                foreach (Cell cell in GridCreator.s_Grid.GetGridArray())
                {
                    DrawArrow.ForDebug(new Vector3(cell.m_XPos, 0.0f, cell.m_ZPos), cell.GetFlowFieldDirection(m_FlowFieldLayer).normalized * 0.5f, Color.red);
                };
                break;
            default:
                break;
        };
#endif
    }
}
