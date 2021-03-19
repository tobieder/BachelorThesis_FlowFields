using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnMultipleNPCs : MonoBehaviour
{
    public Transform m_Parent;
    public GameObject m_NPC;
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
            if (hit.point.x < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.s_Grid.GetCellSize()) ||
                hit.point.x + m_RootOfNumberOfNPCs > ((GridCreator.s_Grid.GetWidth() * GridCreator.s_Grid.GetCellSize()) - (GridCreator.s_Grid.GetCellSize() / 2.0f)) ||
                hit.point.z + m_RootOfNumberOfNPCs > ((GridCreator.s_Grid.GetHeight() * GridCreator.s_Grid.GetCellSize() - (GridCreator.s_Grid.GetCellSize() / 2.0f))))
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
        Instantiate(m_NPC, _pos, Quaternion.Euler(1.0f, 0.0f, 0.0f), m_Parent);
    }
}
