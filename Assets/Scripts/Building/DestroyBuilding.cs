using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyBuilding : MonoBehaviour
{
    public LayerMask layerMask;

    public Material previewDestroy;

    private Material[] originalMaterials;
    private GameObject oldGO;

    private void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50000.0f, layerMask))
            {
                GameObject goToDestroy = hit.transform.gameObject;
                Renderer goRenderer = goToDestroy.GetComponent<Renderer>();
                Vector3 goSize = goRenderer.bounds.size;

                if(oldGO == null)
                {
                    originalMaterials = goRenderer.materials;
                }

                goRenderer.material = previewDestroy;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 offset = new Vector3();
                    bool even = false;
                    if ((int)(goSize.x) % 2 == 0)
                    {
                        offset.x = 0.5f;
                        even = true;
                    }
                    if ((int)(goSize.z) % 2 == 0)
                    {
                        even = true;
                        offset.z = 0.5f;
                    }

                    if (even)
                    {
                        for (int x = Mathf.FloorToInt(-goSize.x / 2f); x < (int)(goSize.x / 2f); x++)
                        {
                            for (int z = Mathf.FloorToInt(-goSize.z / 2f) + 1; z < (int)(goSize.z / 2f); z++)
                            {
                                Vector3 currPos = new Vector3(goToDestroy.transform.position.x + x + offset.x, 0.0f, goToDestroy.transform.position.z + z + offset.z);
                                Cell currCell = GridCreator.grid.getCellFromPosition(currPos.x, currPos.z);
                                currCell.SetCost(currCell.GetOriginalCost());
                            }
                        }
                    }
                    else
                    {
                        // TODO: Only 1x1 objects working rn
                        Vector3 currPos = currPos = new Vector3(goToDestroy.transform.localPosition.x, 0.0f, goToDestroy.transform.localPosition.z);
                        Cell currCell = GridCreator.grid.getCellFromPosition(currPos.x, currPos.z);
                        currCell.SetCost(currCell.GetOriginalCost());
                    }

                    Destroy(goToDestroy, 0.02f);
                }
                oldGO = goToDestroy;
            }
            else
            {
                if(oldGO != null)
                {
                    Debug.Log("Moved away from " + oldGO.name);
                    oldGO.GetComponent<Renderer>().materials = originalMaterials;
                }
                oldGO = null;
            }
        }
    }
}
