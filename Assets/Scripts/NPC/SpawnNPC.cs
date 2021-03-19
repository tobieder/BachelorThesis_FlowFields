using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnNPC : MonoBehaviour
{
    public Transform m_Parent;
    public GameObject m_NPC;

    void Start()
    {
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            CreateNPCAtMousePos();
        }
    }

    void CreateNPCAtMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            // TEMP: limit spawn area to grid size
            if (hit.point.x < -(0.5 * GridCreator.s_Grid.GetCellSize()) || 
                hit.point.z < -(0.5 * GridCreator.s_Grid.GetCellSize()) || 
                hit.point.x > ((GridCreator.s_Grid.GetWidth() * GridCreator.s_Grid.GetCellSize()) - (GridCreator.s_Grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.s_Grid.GetHeight() * GridCreator.s_Grid.GetCellSize() - (GridCreator.s_Grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to spawn NPC out of grid bounds!");
            }
            else
            {
                CreateNPCAtPos(hit.point);
            }
        }
    }

    void CreateNPCAtPos(Vector3 _pos)
    {
        Instantiate(m_NPC, _pos, Quaternion.Euler(1.0f, 0.0f, 0.0f), m_Parent);
    }
}
