using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarTest : MonoBehaviour
{
    private AStarPathfinding pathfinding;

    public GameObject pathFollowingNPC;

    void Start()
    {
        pathfinding = new AStarPathfinding();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(SelectedDictionary.selectedDictionary.Count == 0)
            {
                Debug.Log("No Units selected.");
            }
            else
            {
                Cell clickedCell = getClickedCell();
                if (clickedCell != null)
                {
                    Debug.Log(clickedCell.ToString());
                    float averagePathLength = 0.0f;
                    float startTime = Time.realtimeSinceStartup;
                    foreach (KeyValuePair<int, GameObject> npc in SelectedDictionary.selectedDictionary)
                    {
                        //npc.Value.GetComponentInParent<NPC>().SetFlowMapIndex(indexToUse);
                        Vector3 pos = new Vector3(npc.Value.GetComponentInParent<NPC>().transform.position.x, 0.0f, npc.Value.GetComponentInParent<NPC>().transform.position.z);
                        List<Cell> path = pathfinding.FindPath(GridCreator.grid.getCellFromPosition(pos.x, pos.z), clickedCell);

                        if (path != null)
                        {
                            averagePathLength += path.Count;
                            for (int i = 0; i < path.Count - 1; i++)
                            {
                                //Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos) * 10f + Vector3.one * 5f, Color.green, 1000.0f);
                                Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos), new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos), Color.green, 10.0f);
                            }
                        }
                    }
                    float endTime = Time.realtimeSinceStartup;
                }
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
}
