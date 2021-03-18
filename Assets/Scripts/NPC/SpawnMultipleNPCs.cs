using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnMultipleNPCs : MonoBehaviour
{
    public Transform parent;
    public GameObject npc;
    public int m_RootOfNumberOfNPCs;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            CreateNPCsAtMousePos();
        }
    }

    void CreateNPCsAtMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.point.x < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.x + m_RootOfNumberOfNPCs > ((GridCreator.grid.GetWidth() * GridCreator.grid.GetCellSize()) - (GridCreator.grid.GetCellSize() / 2.0f)) ||
                hit.point.z + m_RootOfNumberOfNPCs > ((GridCreator.grid.GetHeight() * GridCreator.grid.GetCellSize() - (GridCreator.grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to spawn NPC out of grid bounds!");
            }
            else
            {
                for(int i = 0; i < m_RootOfNumberOfNPCs; i++)
                {
                    for(int j = 0; j < m_RootOfNumberOfNPCs; j++)
                    {
                        CreateNPCAtPos(new Vector3(hit.point.x + i, 0.0f, hit.point.z + j));
                    }
                }
            }
        }
    }

    void CreateNPCAtPos(Vector3 _pos)
    {
        Instantiate(npc, _pos, Quaternion.Euler(1.0f, 0.0f, 0.0f), parent);
    }
}
