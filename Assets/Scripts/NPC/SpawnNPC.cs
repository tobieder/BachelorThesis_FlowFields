using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnNPC : MonoBehaviour
{
    public GameObject npc;

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
            if (hit.point.x < -(0.5 * GridCreator.grid.GetCellSize()) || 
                hit.point.z < -(0.5 * GridCreator.grid.GetCellSize()) || 
                hit.point.x > ((GridCreator.grid.GetWidth() * GridCreator.grid.GetCellSize()) - (GridCreator.grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.grid.GetHeight() * GridCreator.grid.GetCellSize() - (GridCreator.grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to spawn NPC out of grid bounds!");
            }
            else
            {
                Instantiate(npc, hit.point, Quaternion.Euler(1.0f, 0.0f, 0.0f));
            }
        }
    }
}
