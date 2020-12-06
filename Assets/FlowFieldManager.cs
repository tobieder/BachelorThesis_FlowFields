using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    // Singleton
    private static FlowFieldManager _instance;
    public static FlowFieldManager Instance{get { return _instance; } }
    // ---------

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
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (SelectedDictionary.selectedDictionary.Count == 0)
            {
                Debug.Log("No units selected");
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
                return GridCreator.grid.getCell((int)hit.point.x, (int)hit.point.z);
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

        currentlyUsedFlowFields = usedIndices;
    }

    public Cell getDestinationCell()
    {
        return destinationCell;
    }
}
