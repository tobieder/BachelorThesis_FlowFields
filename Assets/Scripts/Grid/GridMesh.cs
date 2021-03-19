using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMesh : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float m_GridPercentage = 0.9f;
    public Material m_GridMeshMaterial;

    private GameObject m_GridMesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    void Start()
    {
        GridCreator gridCreator = GetComponent<GridCreator>();

        MeshUpdate();
    }

    public void MeshUpdate()
    {
        GridCreator gridCreator = GetComponent<GridCreator>();
        if (GridCreator.s_Grid == null)
        {
            gridCreator.InitialzeGrid();
        }

        if (!m_GridMesh)
        {
            m_GridMesh = new GameObject("GridMesh");
            m_GridMesh.transform.parent = this.transform;

            m_MeshRenderer = m_GridMesh.AddComponent<MeshRenderer>();
        }
        if (!m_MeshFilter)
        {
            m_MeshFilter = m_GridMesh.GetComponent<MeshFilter>();
            if (!m_MeshFilter)
            {
                m_MeshFilter = m_GridMesh.AddComponent<MeshFilter>();
            }
        }
        if (!m_MeshRenderer)
        {
            m_MeshRenderer = m_GridMesh.GetComponent<MeshRenderer>();
            if (!m_MeshRenderer)
            {
                m_MeshRenderer = m_GridMesh.AddComponent<MeshRenderer>();
            }
        }

        CreateMesh(GridCreator.s_Grid, gridCreator);
    }

    void CreateMesh(Grid s_Grid, GridCreator gridCreator)
    {
        Vector3[] vertices = new Vector3[(gridCreator.m_Width * gridCreator.m_Height * 4) + 4];
        Vector2[] uvs = new Vector2[(gridCreator.m_Width * gridCreator.m_Height * 4) + 4];
        int[] triangles = new int[gridCreator.m_Width * gridCreator.m_Height * 6 * 3 + (8 * 3)];

        float halfCellsize = gridCreator.m_CellSize / 2.0f;

        // ----- Set Vertices -----
        int vertexCount = 0;
        for (int z = 0; z < gridCreator.m_Height; z++)
        {
            for (int x = 0; x < gridCreator.m_Width; x++)
            {
                vertices[vertexCount + 0] = s_Grid.GetWorldPositon(x, z) + (new Vector3(-halfCellsize, 0.0f, -halfCellsize) * m_GridPercentage);
                vertices[vertexCount + 1] = s_Grid.GetWorldPositon(x, z) + (new Vector3(-halfCellsize, 0.0f, halfCellsize) * m_GridPercentage);
                vertices[vertexCount + 2] = s_Grid.GetWorldPositon(x, z) + (new Vector3(halfCellsize, 0.0f, halfCellsize) * m_GridPercentage);
                vertices[vertexCount + 3] = s_Grid.GetWorldPositon(x, z) + (new Vector3(halfCellsize, 0.0f, -halfCellsize) * m_GridPercentage);

                vertexCount += 4;
            }
        }

        // ----- Set Triangles -----
        int triangleCount = 0;

        for (int z = 1; z < gridCreator.m_Height; z++)
        {
            triangles[triangleCount + 0] = (z * gridCreator.m_Width) * 4;
            triangles[triangleCount + 1] = ((z * gridCreator.m_Width) - gridCreator.m_Width) * 4 + 2;
            triangles[triangleCount + 2] = ((z * gridCreator.m_Width) - gridCreator.m_Width) * 4 + 1;

            triangles[triangleCount + 3] = (z * gridCreator.m_Width) * 4;
            triangles[triangleCount + 4] = (z * gridCreator.m_Width) * 4 + 3;
            triangles[triangleCount + 5] = ((z * gridCreator.m_Width) - gridCreator.m_Width) * 4 + 2;


            triangleCount += 6;
        }
        for (int x = 1; x < gridCreator.m_Width; x++)
        {
            triangles[triangleCount + 0] = x * 4;
            triangles[triangleCount + 1] = x * 4 - 1;
            triangles[triangleCount + 2] = x * 4 - 2;

            triangles[triangleCount + 3] = x * 4;
            triangles[triangleCount + 4] = x * 4 - 2;
            triangles[triangleCount + 5] = x * 4 + 1;

            triangleCount += 6;
        }

        int counter = gridCreator.m_Width + 1;
        for (int z = 1; z < gridCreator.m_Height; z++)
        {
            for (int x = 1; x < gridCreator.m_Width; x++)
            {
                int currIndex = counter * 4;

                // Left Border
                triangles[triangleCount + 0] = currIndex;                                       //16
                triangles[triangleCount + 1] = currIndex - 1;                                   //15
                triangles[triangleCount + 2] = currIndex - 2;                                   //14

                triangles[triangleCount + 3] = currIndex;                                       //16
                triangles[triangleCount + 4] = currIndex - 2;                                   //14
                triangles[triangleCount + 5] = currIndex + 1;                                   //17

                triangleCount += 6;

                // Lower Border;
                triangles[triangleCount + 0] = currIndex;                                       //16
                triangles[triangleCount + 1] = currIndex + 3;                                   //19
                triangles[triangleCount + 2] = currIndex - (gridCreator.m_Width * 4) + 2;         //6

                triangles[triangleCount + 3] = currIndex;                                       //16
                triangles[triangleCount + 4] = currIndex - (gridCreator.m_Width * 4) + 2;         //6
                triangles[triangleCount + 5] = currIndex - (gridCreator.m_Width * 4) + 1;         //5

                triangleCount += 6;

                // Square
                triangles[triangleCount + 0] = currIndex;
                triangles[triangleCount + 1] = currIndex - (gridCreator.m_Width * 4) + 1;
                triangles[triangleCount + 2] = currIndex - (gridCreator.m_Width * 4) - 2;

                triangles[triangleCount + 3] = currIndex;
                triangles[triangleCount + 4] = currIndex - (gridCreator.m_Width * 4) - 2;
                triangles[triangleCount + 5] = currIndex - 1;

                triangleCount += 6;

                counter++;
            }
            counter++;
        }

        // ----- Borders -----
        vertices[vertexCount + 0] = s_Grid.GetWorldPositon(0, 0) + (new Vector3(-halfCellsize, 0.0f, -halfCellsize) * (1.0f + (1.0f - m_GridPercentage)));
        vertices[vertexCount + 1] = s_Grid.GetWorldPositon(gridCreator.m_Width - 1, 0) + (new Vector3(halfCellsize, 0.0f, -halfCellsize) * (1.0f + (1.0f - m_GridPercentage)));
        vertices[vertexCount + 2] = s_Grid.GetWorldPositon(gridCreator.m_Width - 1, gridCreator.m_Height - 1) + (new Vector3(halfCellsize, 0.0f, halfCellsize) * (1.0f + (1.0f - m_GridPercentage)));
        vertices[vertexCount + 3] = s_Grid.GetWorldPositon(0, gridCreator.m_Height - 1) + (new Vector3(-halfCellsize, 0.0f, halfCellsize) * (1.0f + (1.0f - m_GridPercentage)));


        triangles[triangleCount + 0] = 0;
        triangles[triangleCount + 1] = vertexCount + 1;
        triangles[triangleCount + 2] = vertexCount + 0;

        triangles[triangleCount + 3] = 0;
        triangles[triangleCount + 4] = (gridCreator.m_Width - 1) * 4 + 3;
        triangles[triangleCount + 5] = vertexCount + 1;
        triangleCount += 6;

        triangles[triangleCount + 0] = (gridCreator.m_Width - 1) * 4 + 3;
        triangles[triangleCount + 1] = vertexCount + 2;
        triangles[triangleCount + 2] = vertexCount + 1;

        triangles[triangleCount + 3] = (gridCreator.m_Width - 1) * 4 + 3;
        triangles[triangleCount + 4] = ((gridCreator.m_Width * gridCreator.m_Height) - 1) * 4 + 2;
        triangles[triangleCount + 5] = vertexCount + 2;
        triangleCount += 6;

        triangles[triangleCount + 0] = ((gridCreator.m_Width * gridCreator.m_Height) - 1) * 4 + 2;
        triangles[triangleCount + 1] = vertexCount + 3;
        triangles[triangleCount + 2] = vertexCount + 2;

        triangles[triangleCount + 3] = ((gridCreator.m_Width * gridCreator.m_Height) - 1) * 4 + 2;
        triangles[triangleCount + 4] = (((gridCreator.m_Height - 1) * gridCreator.m_Width)) * 4 + 1;
        triangles[triangleCount + 5] = vertexCount + 3;
        triangleCount += 6;

        triangles[triangleCount + 0] = (((gridCreator.m_Height - 1) * gridCreator.m_Width)) * 4 + 1;
        triangles[triangleCount + 1] = vertexCount + 0;
        triangles[triangleCount + 2] = vertexCount + 3;

        triangles[triangleCount + 3] = (((gridCreator.m_Height - 1) * gridCreator.m_Width)) * 4 + 1;
        triangles[triangleCount + 4] = 0;
        triangles[triangleCount + 5] = vertexCount + 0;
        triangleCount += 6;


        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        m_MeshFilter.mesh = mesh;

        m_MeshRenderer.sharedMaterial = m_GridMeshMaterial;
        m_MeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}