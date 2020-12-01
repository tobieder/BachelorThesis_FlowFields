using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class SpawnBuilding : MonoBehaviour
{
    public GameObject building;
    public Material previewMaterial;
    public Material previewMaterialNegative;

    public int2 size;
    public int2 anchor;

    private GameObject preview;

    private bool unbuildable = false;

    public void OnSelected()
    {
        preview = Instantiate(building);
        foreach (var renderer in preview.GetComponentsInChildren<Renderer>(true))
        {
            renderer.sharedMaterial = previewMaterial;
        }
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
                Vector3 currPos = new Vector3(((int)hit.point.x) + 0.5f, 0.0f, ((int)hit.point.z) + 0.5f);
                preview.transform.position = currPos;

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
        GameObject instance = Instantiate(building, _position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
        instance.GetComponent<MeshCollider>().enabled = true;
        // Set Cost Field
        for(int x = -(int)(size.x/2); x < (int)(size.x/2); x++)
        {
            for (int z = -(int)(size.y / 2); z < (int)(size.y / 2); z++)
            {
                Vector3 currPos = new Vector3(_position.x + x + 0.5f, 0.0f, _position.z + z + 0.5f);
                Debug.Log(currPos);
                GridCreator.grid.getCellFromPosition(currPos.x, currPos.z).SetCost(byte.MaxValue);
            }
        }
    }
}
