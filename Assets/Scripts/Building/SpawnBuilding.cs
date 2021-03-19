using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.EventSystems;

public class SpawnBuilding : MonoBehaviour
{
    public GameObject m_Parent;

    public GameObject m_Building;
    public Material m_PreviewMaterial;
    public Material m_PreviewMaterialNegative;

    public int2 m_Size;

    private GameObject m_Preview;

    private bool m_Even;
    private Vector3 m_Offset;

    private int m_LayerMask;

    private void Start()
    {
        m_LayerMask = SetLayerMask("Ground");

        m_Even = false;
        if(m_Size.x % 2 == 0)
        {
            m_Offset.x = 0.5f;
            m_Even = true;
        }
        if(m_Size.y % 2 == 0)
        {
            m_Even = true;
            m_Offset.z = 0.5f;
        }
    }

    public void OnSelected()
    {
        m_Preview = Instantiate(m_Building, m_Parent.transform);
        foreach (var renderer in m_Preview.GetComponentsInChildren<Renderer>(true))
        {
            renderer.sharedMaterial = m_PreviewMaterial;
        }
        m_Preview.transform.position = new Vector3(-1000.0f, 1000.0f, -1000.0f);
    }

    public void OnDeselected()
    {
        Destroy(m_Preview);
    }

    private void Update()
    {
        if(!m_Preview)
        {
            return;
        }

        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50000.0f, m_LayerMask))
            {
                Vector3 currPos;
                if(m_Even)
                {
                    currPos = new Vector3((int)(hit.point.x) + m_Offset.x, 0.0f,(int)(hit.point.z) + m_Offset.z);
                }
                else
                {
                    currPos = new Vector3(Mathf.RoundToInt(hit.point.x) + m_Offset.x, 0.0f, Mathf.RoundToInt(hit.point.z) + m_Offset.z);
                }
                m_Preview.transform.position = currPos;

                if(!CheckUnbuildable(currPos))
                {
                    foreach (var renderer in m_Preview.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.sharedMaterial = m_PreviewMaterial;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        CreateBuilding(currPos);
                    }
                }
                else
                {
                    foreach (var renderer in m_Preview.GetComponentsInChildren<Renderer>(true))
                    {
                        renderer.sharedMaterial = m_PreviewMaterialNegative;
                    }
                }
            }

        }
    }

    private void CreateBuilding(Vector3 _position)
    {
        GameObject instance = Instantiate(m_Building, _position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), m_Parent.transform);
        instance.GetComponent<Collider>().enabled = true;
        if (m_Even)
        {
            // Set Cost Field
            for (int x = Mathf.FloorToInt(-m_Size.x / 2f); x < (int)(m_Size.x / 2f); x++)
            {
                for (int z = Mathf.FloorToInt(-m_Size.y / 2f); z < (int)(m_Size.y / 2f); z++)
                {
                    Vector3 currPos = new Vector3(_position.x + x + m_Offset.x, 0.0f, _position.z + z + m_Offset.z);
                    GridCreator.s_Grid.getCellFromPosition(currPos.x, currPos.z).SetCost(byte.MaxValue);
                }
            }
        }
        else
        {
            GridCreator.s_Grid.getCellFromPosition(_position.x, _position.z).SetCost(byte.MaxValue);
            for (int x = Mathf.CeilToInt(-m_Size.x / 2f); x < Mathf.CeilToInt(m_Size.x / 2f); x++)
            {
                for (int z = Mathf.CeilToInt(-m_Size.y / 2f); z < Mathf.CeilToInt(m_Size.y / 2f); z++)
                {
                    Vector3 currPos = new Vector3(_position.x + x + m_Offset.x, 0.0f, _position.z + z + m_Offset.z);
                    GridCreator.s_Grid.getCellFromPosition(currPos.x, currPos.z).SetCost(byte.MaxValue);
                }
            }
        }
    }

    private bool CheckUnbuildable(Vector3 _position)
    {
        if (m_Even)
        {
            // Size != 1,1 && even size
            for (int x = Mathf.FloorToInt(-m_Size.x / 2f); x < (int)(m_Size.x / 2f); x++)
            {
                for (int z = Mathf.FloorToInt(-m_Size.y / 2f); z < (int)(m_Size.y / 2f); z++)
                {
                    Vector3 currPos = new Vector3(_position.x + x + m_Offset.x, 0.0f, _position.z + z + m_Offset.z);
                    Cell currCell = GridCreator.s_Grid.getCellFromPosition(currPos.x, currPos.z);
                    if (currCell == null)
                    {
                        return true;
                    }
                    if (currCell.GetCost() == byte.MaxValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            Cell currCell = GridCreator.s_Grid.getCellFromPosition(_position.x, _position.z);
            if(currCell == null)
            {
                return true;
            }
            if(currCell.GetCost() == byte.MaxValue)
            {
                return true;
            }
            return false;
        }
    }

    public int SetLayerMask(params string[] layerNames)
    {
        int num = 0;
        
        for(int i = 0; i < layerNames.Length; i++)
        {
            string layerName = layerNames[i];
            int num2 = LayerMask.NameToLayer(layerName);
            if(num2 != -1)
            {
                num |= 1 << num2;
            }
        }

        return num;
    }
}
