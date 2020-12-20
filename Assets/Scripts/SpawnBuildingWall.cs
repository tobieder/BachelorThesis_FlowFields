using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnBuildingWall: MonoBehaviour
{
    public GameObject parent;

    public GameObject building;
    public Material previewMaterial;
    public Material previewMaterialNegative;

    private GameObject preview;

    private int layerMask;

    private bool dragBuild;

    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;

    private void Start()
    {
        layerMask = SetLayerMask("Ground");
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

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if(Input.GetMouseButtonDown(0))
            {
                dragStartPosition = Input.mousePosition;
            }

            if(Input.GetMouseButton(0))
            {
                if((dragStartPosition - Input.mousePosition).magnitude > 0.5f)
                {
                    dragBuild = true;
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                if(!dragBuild)
                {

                }
            }
        }
    }

    public int SetLayerMask(params string[] layerNames)
    {
        int num = 0;

        for (int i = 0; i < layerNames.Length; i++)
        {
            string layerName = layerNames[i];
            int num2 = LayerMask.NameToLayer(layerName);
            if (num2 != -1)
            {
                num |= 1 << num2;
            }
        }

        return num;
    }
}
