using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    // Singleton
    private static FlowFieldManager _instance;
    public static FlowFieldManager Instance{get { return _instance; } }
    // ---------

    [SerializeField]
    private GameObject m_TargetFlag;
    [SerializeField]
    private Transform m_Parent;
    private Dictionary<byte, GameObject> m_PlacedFlags;

    private FlowField m_FlowField;
    private Cell m_DestinationCell;

    private List<byte> m_CurrentlyUsedFlowFields;

    private bool m_DynamicAStarSwitch = false;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        m_FlowField = new FlowField();

        m_CurrentlyUsedFlowFields = new List<byte>();

        m_PlacedFlags = new Dictionary<byte, GameObject>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (SelectedDictionary.s_SelectedDictionary.Count == 0)
            {
                //Debug.Log("No units selected");
                return;
            }
            else
            {
                byte indexToUse = 0;
                for(; indexToUse < byte.MaxValue; indexToUse++) // Look for free FlowFieldIndex
                {
                    if(!m_CurrentlyUsedFlowFields.Contains(indexToUse))
                    {
                        m_CurrentlyUsedFlowFields.Add(indexToUse);
                        break;
                    }
                }
                m_DestinationCell = getClickedCell();
                if (m_DestinationCell != null)
                {
                    int indexCounter = 0;
                    foreach (KeyValuePair<int, GameObject> npc in SelectedDictionary.s_SelectedDictionary)
                    {
                        if (m_DynamicAStarSwitch)
                        {
                            npc.Value.GetComponentInParent<NPC>().SetPathfindingDynamic(indexToUse, m_DestinationCell, indexCounter, SelectedDictionary.s_SelectedDictionary.Count);
                            indexCounter++;
                        }
                        else
                        {
                            npc.Value.GetComponentInParent<NPC>().SetPathfindingFlowField(indexToUse);
                        }
                    }

                    float startTime = Time.realtimeSinceStartup;

                    m_FlowField.FlowFieldPathfinding(GridCreator.s_Grid, indexToUse, m_DestinationCell);
                    StartCoroutine(FlowFieldDisplay.Instance.CreateFlowFieldDirectionMesh(indexToUse, m_DestinationCell));

                    float endTime = Time.realtimeSinceStartup;
                }

                m_PlacedFlags.Add(indexToUse, Instantiate<GameObject>(m_TargetFlag, new Vector3(m_DestinationCell.m_XPos, 0.0f, m_DestinationCell.m_ZPos), Quaternion.Euler(-90.0f, Random.Range(0.0f, 360.0f), 0.0f), m_Parent));

                CleanUpCurrentlyUsedFlowFields();
            }
        }
    }

    private Cell getClickedCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Limit spawn area to grid size.
            if (hit.point.x < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.x > ((GridCreator.s_Grid.GetWidth() * GridCreator.s_Grid.GetCellSize()) - (GridCreator.s_Grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.s_Grid.GetHeight() * GridCreator.s_Grid.GetCellSize() - (GridCreator.s_Grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to get the clicked on cell. Out of bounds!");
            }
            else
            {
                return GridCreator.s_Grid.getCell(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            }
        }

        return null;
    }

    private void CleanUpCurrentlyUsedFlowFields()
    {
        List<byte> usedIndices = new List<byte>();

        for (byte i = 0; i < byte.MaxValue; i++)
        {
            foreach (NPC npc in NPCManager.Instance.m_NPCs)
            {
                if (npc.GetFlowMapIndex() == i)
                {
                    usedIndices.Add(i);
                    break;
                }
            }
        }

        List<byte> entriesToRemove = new List<byte>();
        foreach (byte b in m_PlacedFlags.Keys)
        {
            if (!usedIndices.Contains(b))
            {
                GameObject temp = m_PlacedFlags[b];
                entriesToRemove.Add(b);
                Destroy(temp);
            }
        }

        foreach(byte b in entriesToRemove)
        {
            m_PlacedFlags.Remove(b);
        }

        m_CurrentlyUsedFlowFields = usedIndices;
    }

    public Cell getDestinationCell()
    {
        return m_DestinationCell;
    }

    public List<byte> GetCurrentlyUsedFlowFields()
    {
        return m_CurrentlyUsedFlowFields;
    }

    public void SetDynamicAStarSwitch(bool _value)
    {
        m_DynamicAStarSwitch = _value;
    }
}
