using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyBuilding : MonoBehaviour
{
    public LayerMask m_LayerMask;

    public Material m_PreviewDestroy;

    private Material[] m_OriginalMaterials;
    private GameObject m_OldGO;

    private void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50000.0f, m_LayerMask))
            {
                GameObject goToDestroy = hit.transform.gameObject;
                Renderer goRenderer = goToDestroy.GetComponent<Renderer>();
                Vector3 goSize = goRenderer.bounds.size;

                if(m_OldGO == null)
                {
                    m_OriginalMaterials = goRenderer.materials;
                }

                goRenderer.material = m_PreviewDestroy;

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
                                Cell currCell = GridCreator.s_Grid.getCellFromPosition(currPos.x, currPos.z);
                                currCell.SetCost(currCell.GetOriginalCost());
                            }
                        }
                    }
                    else
                    {
                        // TODO: Only 1x1 objects working rn
                        Vector3 currPos = currPos = new Vector3(goToDestroy.transform.localPosition.x, 0.0f, goToDestroy.transform.localPosition.z);
                        Cell currCell = GridCreator.s_Grid.getCellFromPosition(currPos.x, currPos.z);
                        currCell.SetCost(currCell.GetOriginalCost());
                    }

                    Destroy(goToDestroy, 0.02f);
                }
                m_OldGO = goToDestroy;
            }
            else
            {
                if(m_OldGO != null)
                {
                    Debug.Log("Moved away from " + m_OldGO.name);
                    m_OldGO.GetComponent<Renderer>().materials = m_OriginalMaterials;
                }
                m_OldGO = null;
            }
        }
    }
}
