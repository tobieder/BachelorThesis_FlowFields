using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType { FlowField, AStar, Dynamic}

public class NPCController : MonoBehaviour
{
    [SerializeField]
    private FlowFieldManager m_FlowFieldManager;
    [SerializeField]
    private AStarController m_AStarController;

    private ControlType m_ControlType;

    private void Start()
    {
        m_ControlType = ControlType.FlowField;
        HandleInputData((int)m_ControlType);
    }

    public void HandleInputData(int val)
    {
        m_ControlType = (ControlType)val;

        switch(m_ControlType)
        {
            case ControlType.FlowField:
                {
                    m_FlowFieldManager.gameObject.SetActive(true);
                    m_FlowFieldManager.SetDynamicAStarSwitch(false);
                    m_AStarController.gameObject.SetActive(false);
                }
                break;
            case ControlType.AStar:
                {
                    m_AStarController.gameObject.SetActive(true);
                    m_FlowFieldManager.gameObject.SetActive(false);
                }
                break;
            case ControlType.Dynamic:
                {
                    m_FlowFieldManager.gameObject.SetActive(true);
                    m_FlowFieldManager.SetDynamicAStarSwitch(true);
                    m_AStarController.gameObject.SetActive(false);
                }
                break;
            default:
                Debug.LogError("Unavailable ControlType selected.");
                break;
        }
    }
}
