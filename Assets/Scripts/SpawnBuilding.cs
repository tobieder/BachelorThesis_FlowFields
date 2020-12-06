using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class SpawnBuilding : MonoBehaviour
{
    public GameObject parent;

    public GameObject building;
    public Material previewMaterial;
    public Material previewMaterialNegative;

    public int2 size;

    private GameObject preview;

    private bool unbuildable = false;
    private bool even;
    private Vector3 offset;

    private void Start()
    {
        even = false;
        if(size.x % 2 == 0)
        {
            offset.x = 0.5f;
            even = true;
        }
        if(size.y % 2 == 0)
        {
            even = true;
            offset.z = 0.5f;
        }
    }

    public void OnSelected()
    {
        preview = Instantiate(building, parent.transform);
        foreach (var renderer in preview.GetComponentsInChildren<Renderer>(true))
        {
            renderer.sharedMaterial = previewMaterial;
        }
        preview.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
    }

    public void OnDeselected()
    {
        Destroy(preview);
    }

    private void Update()
    {
        if(!preview)
        {
            return;
        }

        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 currPos;
                if(even)
                {
                    currPos = new Vector3((int)(hit.point.x) + offset.x, 0.0f,(int)(hit.point.z) + offset.z);
                }
                else
                {
                    currPos = new Vector3(Mathf.RoundToInt(hit.point.x) + offset.x, 0.0f, Mathf.RoundToInt(hit.point.z) + offset.z);
                }
                preview.transform.position = currPos;

                CheckUnbuildable(currPos);

                if(!unbuildable)
                {
                    foreach (var renderer in preview.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.sharedMaterial = previewMaterial;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        CreateBuilding(currPos);
                    }
                }
                else
                {
                    foreach (var renderer in preview.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.sharedMaterial = previewMaterialNegative;
                    }
                }
            }

        }
    }

    private void CreateBuilding(Vector3 _position)
    {
        GameObject instance = Instantiate(building, _position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), parent.transform);
        instance.GetComponent<MeshCollider>().enabled = true;
        if (even)
        {
            // Set Cost Field
            for (int x = Mathf.FloorToInt(-size.x / 2f); x < (int)(size.x / 2f); x++)
            {
                for (int z = Mathf.FloorToInt(-size.y / 2f); z < (int)(size.y / 2f); z++)
                {
                    Vector3 currPos = new Vector3(_position.x + x + offset.x, 0.0f, _position.z + z + offset.z);
                    GridCreator.grid.getCellFromPosition(currPos.x, currPos.z).SetCost(byte.MaxValue);
                }
            }
        }
        else
        {
            GridCreator.grid.getCellFromPosition(_position.x, _position.z).SetCost(byte.MaxValue);
        }
    }

    private void CheckUnbuildable(Vector3 _position)
    {
        if (even)
        {
            // Size != 1,1 && even size
            for (int x = Mathf.FloorToInt(-size.x / 2f); x < (int)(size.x / 2f); x++)
            {
                for (int z = Mathf.FloorToInt(-size.y / 2f); z < (int)(size.y / 2f); z++)
                {
                    Vector3 currPos = new Vector3(_position.x + x + offset.x, 0.0f, _position.z + z + offset.z);
                    Cell currCell = GridCreator.grid.getCellFromPosition(currPos.x, currPos.z);
                    if (currCell == null)
                    {
                        unbuildable = true;
                        return;
                    }
                    if (currCell.GetCost() == byte.MaxValue)
                    {
                        unbuildable = true;
                        return;
                    }
                }
            }
            unbuildable = false;
        }
        else
        {
            Cell currCell = GridCreator.grid.getCellFromPosition(_position.x, _position.z);
            if(currCell == null)
            {
                unbuildable = true;
                return;
            }
            if(currCell.GetCost() == byte.MaxValue)
            {
                unbuildable = true;
                return;
            }
            unbuildable = false;
        }
    }
}
