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
    private GameObject targetFlag;
    private Dictionary<byte, GameObject> placedFlags;

    private FlowField flowField;
    private Cell destinationCell;

    private List<byte> currentlyUsedFlowFields;

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
        flowField = new FlowField();

        currentlyUsedFlowFields = new List<byte>();

        placedFlags = new Dictionary<byte, GameObject>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (SelectedDictionary.selectedDictionary.Count == 0)
            {
                //Debug.Log("No units selected");
                return;
            }
            else
            {
                byte indexToUse = 0;
                for(; indexToUse < byte.MaxValue; indexToUse++) // Search free FlowFieldIndex
                {
                    if(!currentlyUsedFlowFields.Contains(indexToUse))
                    {
                        currentlyUsedFlowFields.Add(indexToUse);
                        foreach(KeyValuePair<int, GameObject> npc in SelectedDictionary.selectedDictionary)
                        {
                            npc.Value.GetComponentInParent<NPC>().SetFlowMapIndex(indexToUse);
                        }
                        break;
                    }
                }
                destinationCell = getClickedCell();
                if (destinationCell != null)
                {
                    flowField.CreateIntegrationField(indexToUse, destinationCell);
                }

                placedFlags.Add(indexToUse, Instantiate<GameObject>(targetFlag, new Vector3(destinationCell.xPos, 0.0f, destinationCell.zPos), Quaternion.Euler(-90.0f, Random.Range(0.0f, 360.0f), 0.0f)));

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
            // TEMP: limit spawn area to grid size
            if (hit.point.x < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.x > ((GridCreator.grid.GetWidth() * GridCreator.grid.GetCellSize()) - (GridCreator.grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.grid.GetHeight() * GridCreator.grid.GetCellSize() - (GridCreator.grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to get the clicked on cell. Out of bounds!");
            }
            else
            {
                return GridCreator.grid.getCell(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.z));
            }
        }

        return null;
    }

    private void CleanUpCurrentlyUsedFlowFields()
    {
        List<byte> usedIndices = new List<byte>();

        for (byte i = 0; i < byte.MaxValue; i++)
        {
            foreach (NPC npc in NPCManager.Instance.npcs)
            {
                if (npc.GetFlowMapIndex() == i)
                {
                    usedIndices.Add(i);
                    break;
                }
            }
        }

        List<byte> entriesToRemove = new List<byte>();
        foreach (byte b in placedFlags.Keys)
        {
            if (!usedIndices.Contains(b))
            {
                GameObject temp = placedFlags[b];
                entriesToRemove.Add(b);
                Destroy(temp);
            }
        }

        foreach(byte b in entriesToRemove)
        {
            placedFlags.Remove(b);
        }

        currentlyUsedFlowFields = usedIndices;
    }

    public Cell getDestinationCell()
    {
        return destinationCell;
    }

    public List<byte> GetCurrentlyUsedFlowFields()
    {
        return currentlyUsedFlowFields;
    }
}
