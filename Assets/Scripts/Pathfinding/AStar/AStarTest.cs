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
            Cell clickedCell = getClickedCell();
            if (clickedCell != null)
            {
                Debug.Log(clickedCell.ToString());
                List<Cell> path = pathfinding.FindPath(GridCreator.grid.getCell(0, 0), clickedCell);

                if (path != null)
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        //Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos) * 10f + Vector3.one * 5f, Color.green, 1000.0f);
                        Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos), new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos), Color.green, 1000.0f);
                    }
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
