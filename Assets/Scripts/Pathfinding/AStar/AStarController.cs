using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarController : MonoBehaviour
{
    public Material m_PathMaterial;

    private AStarPathfinding m_Pathfinding;
    private List<GameObject> m_PathGOs = new List<GameObject>();

    void Start()
    {
        m_Pathfinding = new AStarPathfinding();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if(SelectedDictionary.s_SelectedDictionary.Count == 0)
            {
                Debug.Log("No Units selected.");
            }
            else
            {
                Cell clickedCell = getClickedCell();
                if (clickedCell != null)
                {
                    foreach(GameObject pathGO in m_PathGOs)
                    {
                        Destroy(pathGO, 0.5f);
                    }
                    m_PathGOs = new List<GameObject>();

                    foreach (KeyValuePair<int, GameObject> npc in SelectedDictionary.s_SelectedDictionary)
                    {
                        Vector3 pos = new Vector3(npc.Value.GetComponentInParent<NPC>().transform.position.x, 0.0f, npc.Value.GetComponentInParent<NPC>().transform.position.z);
                        List<Cell> path = m_Pathfinding.FindPath(GridCreator.s_Grid, GridCreator.s_Grid.getCellFromPosition(pos.x, pos.z), clickedCell);

                        if (path != null)
                        {
                            npc.Value.GetComponentInParent<NPC>().SetPathfindingAStar(path);

                            // ----- Display Path ------

                            Vector3[] pathPositions = new Vector3[path.Count];

                            for (int i = 0; i < path.Count - 1; i++)
                            {
                                pathPositions[i] = new Vector3(path[i].m_XPos, 0.01f, path[i].m_ZPos);
                            }

                            pathPositions[path.Count - 1] = new Vector3(path[path.Count - 1].m_XPos, 0.01f, path[path.Count - 1].m_ZPos);

                            GameObject pathGO = new GameObject("Unit" + npc.Key + "Path");
                            pathGO.transform.parent = this.transform;
                            m_PathGOs.Add(pathGO);

                            LineRenderer pathRenderer = pathGO.AddComponent<LineRenderer>();

                            pathRenderer.sharedMaterial = m_PathMaterial;
                            pathRenderer.startWidth = 0.2f;
                            pathRenderer.endWidth = 0.2f;
                            pathRenderer.positionCount = pathPositions.Length;
                            pathRenderer.SetPositions(pathPositions);
                            // ------------------------
                        }
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
            if (hit.point.x < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.x > ((GridCreator.s_Grid.GetWidth() * GridCreator.s_Grid.GetCellSize()) - (GridCreator.s_Grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.s_Grid.GetHeight() * GridCreator.s_Grid.GetCellSize() - (GridCreator.s_Grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to get the clicked on cell. Out of bounds!");
            }
            else
            {
                return GridCreator.s_Grid.getCell((int)hit.point.x, (int)hit.point.z);
            }
        }

        return null;
    }
}
