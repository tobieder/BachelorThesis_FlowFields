﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldDisplay : MonoBehaviour
{
    public enum Directions { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest, None};

    private static FlowFieldDisplay _instance;
    public static FlowFieldDisplay Instance { get { return _instance; } }

    public Material m_TextureAtlas;

    private GameObject m_GroundMesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(CreateFlowFieldDirectionMesh(byte.MaxValue));
    }

    public IEnumerator CreateFlowFieldDirectionMesh(byte _index, Cell _destination = null)
    {
        if (!m_GroundMesh)
        {
            m_GroundMesh = new GameObject("GroundMesh");
            m_GroundMesh.transform.parent = this.transform;

            m_MeshRenderer = m_GroundMesh.AddComponent<MeshRenderer>();
        }
        if (!m_MeshFilter)
        {
            m_MeshFilter = m_GroundMesh.GetComponent<MeshFilter>();
            if (!m_MeshFilter)
            {
                m_MeshFilter = m_GroundMesh.AddComponent<MeshFilter>();
            }
        }
        if (!m_MeshRenderer)
        {
            m_MeshRenderer = m_GroundMesh.GetComponent<MeshRenderer>();
            if (!m_MeshRenderer)
            {
                m_MeshRenderer = m_GroundMesh.AddComponent<MeshRenderer>();
            }
        }

        m_GroundMesh.transform.localPosition = new Vector3(0.0f, 0.01f, 0.0f);

        Grid grid = GridCreator.s_Grid;

        Vector3[] vertices = new Vector3[4 + (((grid.GetWidth() - 1) * 2) * 2) + (((grid.GetHeight() - 1) * 2) * 2) + (((grid.GetWidth() - 1) * (grid.GetHeight() - 1)) * 4)];
        Vector2[] uvs = new Vector2[4 + (((grid.GetWidth() - 1) * 2) * 2) + (((grid.GetHeight() - 1) * 2) * 2) + (((grid.GetWidth() - 1) * (grid.GetHeight() - 1)) * 4)];
        int[] triangles = new int[(grid.GetWidth() * grid.GetHeight()) * 2 * 3];

        // Set Vertices
        int vertexCount = 0;
        for (int z = 0; z < grid.GetHeight(); z++)
        {
            for (int x = 0; x < grid.GetWidth(); x++)
            {
                Cell currCell = grid.getCell(x, z);
                vertices[vertexCount + 0] = new Vector3(currCell.m_XPos - (grid.GetCellSize() * 0.5f), 0.0f, currCell.m_ZPos - (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 1] = new Vector3(currCell.m_XPos - (grid.GetCellSize() * 0.5f), 0.0f, currCell.m_ZPos + (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 2] = new Vector3(currCell.m_XPos + (grid.GetCellSize() * 0.5f), 0.0f, currCell.m_ZPos + (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 3] = new Vector3(currCell.m_XPos + (grid.GetCellSize() * 0.5f), 0.0f, currCell.m_ZPos - (grid.GetCellSize() * 0.5f));

                vertexCount += 4;
            }
        }

        // Set UVs
        Vector2[] curruvs;
        int uvIterator = 0;
        for (int x = 0; x < GridCreator.s_Grid.GetWidth(); x++)
        {
            for (int z = 0; z < GridCreator.s_Grid.GetHeight(); z++)
            {
                Cell currCell = GridCreator.s_Grid.getCell(z, x);
                float angle = Vector3.Angle(new Vector3(0.0f, 0.0f, 1.0f), currCell.GetFlowFieldDirection(_index));
                Vector3 cross = Vector3.Cross(new Vector3(0.0f, 0.0f, 1.0f), currCell.GetFlowFieldDirection(_index));
                if (cross.y < 0) angle = -angle;

                if(currCell.GetFlowFieldDirection(_index) == Vector3.zero || currCell == _destination)
                {
                    curruvs = GetUVs(Directions.None);
                }
                else if (angle == 0.0f)
                {
                    curruvs = GetUVs(Directions.North);
                }
                else if (angle == 45.0f)
                {
                    curruvs = GetUVs(Directions.NorthEast);
                }
                else if (angle == 90.0f)
                {
                    curruvs = GetUVs(Directions.East);
                }
                else if (angle == 135.0)
                {
                    curruvs = GetUVs(Directions.SouthEast);
                }
                else if (angle == 180.0f)
                {
                    curruvs = GetUVs(Directions.South);
                }
                else if (angle == -135.0)
                {
                    curruvs = GetUVs(Directions.SouthWest);
                }
                else if (angle == -90.0f)
                {
                    curruvs = GetUVs(Directions.West);
                }
                else if (angle == -45.0f)
                {
                    curruvs = GetUVs(Directions.NorthWest);
                }
                else
                {
                    curruvs = GetUVs(Directions.None);
                }

                uvs[uvIterator + 0] = curruvs[0];
                uvs[uvIterator + 1] = curruvs[1];
                uvs[uvIterator + 2] = curruvs[2];
                uvs[uvIterator + 3] = curruvs[3];

                uvIterator += 4;
            }

            yield return null;
        }

        // Set Triangles
        int triangleCount = 0;
        int triIterator = 0;
        for (int z = 0; z < grid.GetWidth(); z++)
        {
            for (int x = 0; x < grid.GetHeight(); x++)
            {
                triangles[triangleCount + 0] = triIterator;
                triangles[triangleCount + 1] = triIterator + 1;
                triangles[triangleCount + 2] = triIterator + 2;

                triangles[triangleCount + 3] = triIterator;
                triangles[triangleCount + 4] = triIterator + 2;
                triangles[triangleCount + 5] = triIterator + 3;

                triangleCount += 6;
                triIterator += 4;
            }
        }

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        m_MeshFilter.mesh = mesh;

        m_MeshRenderer.sharedMaterial = m_TextureAtlas;

        Vector3[] normals = new Vector3[vertices.Length];
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
        }
        mesh.normals = normals;

        mesh.RecalculateNormals();
    }

    public Vector2[] GetUVs(Directions _direction)
    {
        float imgPerSite = 3.0f;

        Vector2[] ret = new Vector2[4];

        switch (_direction)
        {
            case Directions.None:
                ret[0] = new Vector2(1.0f / imgPerSite, 1.0f / imgPerSite);     //links unten
                ret[1] = new Vector2(2.0f / imgPerSite, 1.0f / imgPerSite);     //rechts unten
                ret[2] = new Vector2(2.0f / imgPerSite, 2.0f / imgPerSite);     //rechts oben
                ret[3] = new Vector2(1.0f / imgPerSite, 2.0f / imgPerSite);     //links oben
                break;
            case Directions.North:
                ret[0] = new Vector2(2.0f / imgPerSite, 2.0f / imgPerSite);    //links unten // KORREKT
                ret[1] = new Vector2(2.0f / imgPerSite, 3.0f / imgPerSite);    //rechts unten
                ret[2] = new Vector2(1.0f / imgPerSite, 3.0f / imgPerSite);    //rechts oben
                ret[3] = new Vector2(1.0f / imgPerSite, 2.0f / imgPerSite);    //links oben
                break;
            case Directions.NorthEast:
                ret[0] = new Vector2(2.0f / imgPerSite, 2.0f / imgPerSite); // KORREKT
                ret[1] = new Vector2(3.0f / imgPerSite, 2.0f / imgPerSite);
                ret[2] = new Vector2(3.0f / imgPerSite, 3.0f / imgPerSite);
                ret[3] = new Vector2(2.0f / imgPerSite, 3.0f / imgPerSite);
                break;
            case Directions.East:
                ret[0] = new Vector2(2.0f / imgPerSite, 2.0f / imgPerSite); // KORREKT
                ret[1] = new Vector2(2.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(3.0f / imgPerSite, 1.0f / imgPerSite);
                ret[3] = new Vector2(3.0f / imgPerSite, 2.0f / imgPerSite);
                break;
            case Directions.SouthEast:
                ret[0] = new Vector2(3.0f / imgPerSite, 1.0f / imgPerSite); // KORREKT
                ret[1] = new Vector2(2.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(2.0f / imgPerSite, 0.0f / imgPerSite);
                ret[3] = new Vector2(3.0f / imgPerSite, 0.0f / imgPerSite);
                break;
            case Directions.South:
                ret[0] = new Vector2(2.0f / imgPerSite, 0.0f / imgPerSite);
                ret[1] = new Vector2(2.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(1.0f / imgPerSite, 1.0f / imgPerSite);
                ret[3] = new Vector2(1.0f / imgPerSite, 0.0f / imgPerSite);
                break;
            case Directions.SouthWest:
                ret[0] = new Vector2(0.0f / imgPerSite, 0.0f / imgPerSite);
                ret[1] = new Vector2(0.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(1.0f / imgPerSite, 1.0f / imgPerSite);
                ret[3] = new Vector2(1.0f / imgPerSite, 0.0f / imgPerSite);
                break;
            case Directions.West:
                ret[1] = new Vector2(0.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(1.0f / imgPerSite, 1.0f / imgPerSite);
                ret[3] = new Vector2(1.0f / imgPerSite, 2.0f / imgPerSite);
                ret[0] = new Vector2(0.0f / imgPerSite, 2.0f / imgPerSite);
                break;
            case Directions.NorthWest:
                ret[0] = new Vector2(1.0f / imgPerSite, 3.0f / imgPerSite);
                ret[1] = new Vector2(0.0f / imgPerSite, 3.0f / imgPerSite);
                ret[2] = new Vector2(0.0f / imgPerSite, 2.0f / imgPerSite);
                ret[3] = new Vector2(1.0f / imgPerSite, 2.0f / imgPerSite);
                break;
            default:
                Debug.LogError("Ground Type not available.");
                break;
        }

        return ret;
    }
}
