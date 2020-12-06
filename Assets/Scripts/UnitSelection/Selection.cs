using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP
using UnityEditor;

public class Selection : MonoBehaviour
{
    public LayerMask selectableLayer;
    public LayerMask groundLayer;

    public Color rectColor;
    public Color rectBorderColor;

    private SelectedDictionary selectedDictionary;

    private RaycastHit hit;
    private bool dragSelect;

    private MeshCollider selectionBox;
    private Mesh selectionMesh;

    private Vector2[] corners;
    private Vector3[] vertices;
    private Vector3[] vecs;

    private Vector3 dragStartPosition;
    private Vector3 dragEndPosition;

    void Start()
    {
        selectedDictionary = GetComponent<SelectedDictionary>();
        dragSelect = false;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(0))
        {
            if((dragStartPosition - Input.mousePosition).magnitude > 40.0f)
            {
                dragSelect = true;
            }
        }


        if(Input.GetMouseButtonUp(0))
        {
            if (!dragSelect)
            {
                Ray ray = Camera.main.ScreenPointToRay(dragStartPosition);
                
                if(Physics.Raycast(ray, out hit, 50000.0f, selectableLayer))
                {
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        selectedDictionary.AddSelected(hit.transform.gameObject);
                    }
                    else
                    {
                        selectedDictionary.DeselectAll();
                        selectedDictionary.AddSelected(hit.transform.gameObject);
                    }
                }
                else
                {
                    if(!Input.GetKey(KeyCode.LeftShift))
                    {
                        selectedDictionary.DeselectAll();
                    }
                }
            }
            else
            {
                vertices = new Vector3[4];
                vecs = new Vector3[4];
                int i = 0;
                dragEndPosition = Input.mousePosition;
                corners = getBoundingBox(dragStartPosition, dragEndPosition);

                foreach (Vector2 corner in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out hit, 50000.0f, groundLayer))
                    {
                        vertices[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        vecs[i] = ray.origin - hit.point;
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 10.0f);
                    }
                    i++;
                }

                selectionMesh = GenerateSelectionMesh(vertices, vecs);

                selectionBox = gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    selectedDictionary.DeselectAll();
                }

                Destroy(selectionBox, 0.02f);
            }

            dragSelect = false;
        }
    }

    private void OnGUI()
    {
        if(dragSelect)
        {
            var rect = Utils.GetScreenRect(dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, rectColor);
            Utils.DrawScreenRectBorder(rect, 2, rectBorderColor);
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
        selectedDictionary.AddSelected(other.gameObject);
    }
}
