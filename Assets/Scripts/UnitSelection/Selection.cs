using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    public LayerMask m_SelectableLayer;
    public LayerMask m_GroundLayer;

    public Color m_RectColor;
    public Color m_RectBorderColor;

    private SelectedDictionary m_SelectedDictionary;

    private RaycastHit m_Hit;
    private bool m_DragSelect;

    private MeshCollider m_SelectionBox;
    private Mesh m_SelectionMesh;

    private Vector2[] m_Corners;
    private Vector3[] m_Vertices;
    private Vector3[] m_Vecs;

    private Vector3 m_DragStartPosition;
    private Vector3 m_DragEndPosition;

    void Start()
    {
        m_SelectedDictionary = GetComponent<SelectedDictionary>();
        m_DragSelect = false;
    }

    void Update()
    {
        Select();
    }

    private void Select()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_DragStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            if ((m_DragStartPosition - Input.mousePosition).magnitude > 40.0f)
            {
                m_DragSelect = true;
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            if (!m_DragSelect)
            {
                Ray ray = Camera.main.ScreenPointToRay(m_DragStartPosition);

                if (Physics.Raycast(ray, out m_Hit, 50000.0f, m_SelectableLayer))
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        m_SelectedDictionary.AddSelected(m_Hit.transform.gameObject);
                    }
                    else
                    {
                        m_SelectedDictionary.DeselectAll();
                        m_SelectedDictionary.AddSelected(m_Hit.transform.gameObject);
                    }
                }
                else
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        m_SelectedDictionary.DeselectAll();
                    }
                }
            }
            else
            {
                m_Vertices = new Vector3[4];
                m_Vecs = new Vector3[4];
                int i = 0;
                m_DragEndPosition = Input.mousePosition;
                m_Corners = getBoundingBox(m_DragStartPosition, m_DragEndPosition);

                foreach (Vector2 corner in m_Corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out m_Hit, 50000.0f, m_GroundLayer))
                    {
                        m_Vertices[i] = new Vector3(m_Hit.point.x, m_Hit.point.y, m_Hit.point.z);
                        m_Vecs[i] = ray.origin - m_Hit.point;
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), m_Hit.point, Color.red, 10.0f);
                    }
                    i++;
                }

                m_SelectionMesh = GenerateSelectionMesh(m_Vertices, m_Vecs);

                m_SelectionBox = gameObject.AddComponent<MeshCollider>();
                m_SelectionBox.sharedMesh = m_SelectionMesh;
                m_SelectionBox.convex = true;
                m_SelectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    m_SelectedDictionary.DeselectAll();
                }

                Destroy(m_SelectionBox, 0.02f);
            }

            m_DragSelect = false;
        }
    }

    private void OnGUI()
    {
        if(m_DragSelect)
        {
            var rect = Utils.GetScreenRect(m_DragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, m_RectColor);
            Utils.DrawScreenRectBorder(rect, 2, m_RectBorderColor);
        }
    }

    private Vector2[] getBoundingBox(Vector2 _dragStartPos, Vector2 _dragEndPos)
    {
        Vector2 p1, p2, p3, p4;

        if(_dragStartPos.x < _dragEndPos.x)
        {
            if(_dragStartPos.y > _dragEndPos.y)
            {
                p1 = _dragStartPos;
                p2 = new Vector2(_dragEndPos.x, _dragStartPos.y);
                p3 = new Vector2(_dragStartPos.x, _dragEndPos.y);
                p4 = _dragEndPos;
            }
            else
            {
                p1 = new Vector2(_dragStartPos.x, _dragEndPos.y);
                p2 = _dragEndPos;
                p3 = _dragStartPos ;
                p4 = new Vector2(_dragEndPos.x, _dragStartPos.y);
            }
        }
        else
        {
            if (_dragStartPos.y > _dragEndPos.y)
            {
                p1 = new Vector2(_dragEndPos.x, _dragStartPos.y);
                p2 = _dragStartPos;
                p3 = _dragEndPos;
                p4 = new Vector2(_dragStartPos.x, _dragEndPos.y);
            }
            else
            {
                p1 = _dragEndPos;
                p2 = new Vector2(_dragStartPos.x, _dragEndPos.y);
                p3 = new Vector2(_dragEndPos.x, _dragStartPos.y);
                p4 = _dragStartPos;
            }
        }

        Vector2[] corners = { p1, p2, p3, p4 };
        return corners;
    }

    private Mesh GenerateSelectionMesh(Vector3[] _corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] triangles = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };

        for (int i = 0; i < 4; i++)
        {
            verts[i] = _corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = _corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = triangles;

        return selectionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_SelectedDictionary.AddSelected(other.gameObject);
    }
}
