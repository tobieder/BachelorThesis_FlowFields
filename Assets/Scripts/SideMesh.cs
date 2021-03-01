using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideMesh : MonoBehaviour
{
    public Material m_Material;

    public float m_Height = 10.0f;

    private GameObject m_SideMesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        CreateSideMesh();
    }

    private void CreateSideMesh()
    {
        if (!m_SideMesh)
        {
            m_SideMesh = new GameObject("SideMesh");
            m_SideMesh.transform.parent = this.transform;

            m_MeshRenderer = m_SideMesh.AddComponent<MeshRenderer>();
        }
        if (!m_MeshFilter)
        {
            m_MeshFilter = m_SideMesh.GetComponent<MeshFilter>();
            if (!m_MeshFilter)
            {
                m_MeshFilter = m_SideMesh.AddComponent<MeshFilter>();
            }
        }
        if (!m_MeshRenderer)
        {
            m_MeshRenderer = m_SideMesh.GetComponent<MeshRenderer>();
            if (!m_MeshRenderer)
            {
                m_MeshRenderer = m_SideMesh.AddComponent<MeshRenderer>();
            }
        }

        Vector3[] vertices = new Vector3[16];
        Vector2[] uvs = new Vector2[16];
        int[] triangles = new int[8 * 3];

        Grid grid = GridCreator.grid;

        #region MeshCreation

        vertices[0] = new Vector3(-0.5f, 0.0f, -0.5f);
        vertices[1] = new Vector3(grid.GetWidth() - 0.5f, 0.0f, -0.5f);
        vertices[2] = new Vector3(-0.5f, -m_Height, -0.5f);
        vertices[3] = new Vector3(grid.GetWidth() - 0.5f, -m_Height, -0.5f);

        uvs[0] = new Vector2(0.0f, 1.0f);
        uvs[1] = new Vector2(1.0f, 1.0f);
        uvs[2] = new Vector2(0.0f, 0.0f);
        uvs[3] = new Vector2(1.0f, 0.0f);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 3;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 2;

        vertices[4] = new Vector3(grid.GetWidth() - 0.5f, 0.0f, -0.5f);
        vertices[5] = new Vector3(grid.GetWidth() - 0.5f, 0.0f, grid.GetHeight() - 0.5f);
        vertices[6] = new Vector3(grid.GetWidth() - 0.5f, -m_Height, -0.5f);
        vertices[7] = new Vector3(grid.GetWidth() - 0.5f, -m_Height, grid.GetHeight() - 0.5f);

        uvs[4] = new Vector2(0.0f, 1.0f);
        uvs[5] = new Vector2(1.0f, 1.0f);
        uvs[6] = new Vector2(0.0f, 0.0f);
        uvs[7] = new Vector2(1.0f, 0.0f);

        triangles[6] = 4;
        triangles[7] = 5;
        triangles[8] = 7;
        triangles[9] = 4;
        triangles[10] = 7;
        triangles[11] = 6;

        vertices[8] = new Vector3(grid.GetWidth() - 0.5f, 0.0f, grid.GetHeight() - 0.5f);
        vertices[9] = new Vector3(-0.5f, 0.0f, grid.GetHeight() - 0.5f);
        vertices[10] = new Vector3(grid.GetWidth() - 0.5f, -m_Height, grid.GetHeight() - 0.5f);
        vertices[11] = new Vector3(-0.5f, -m_Height, grid.GetHeight() - 0.5f);

        uvs[8]  = new Vector2(0.0f, 1.0f);
        uvs[9]  = new Vector2(1.0f, 1.0f);
        uvs[10] = new Vector2(0.0f, 0.0f);
        uvs[11] = new Vector2(1.0f, 0.0f);

        triangles[12] = 8;
        triangles[13] = 9;
        triangles[14] = 11;
        triangles[15] = 8;
        triangles[16] = 11;
        triangles[17] = 10;

        vertices[12] = new Vector3(- 0.5f, 0.0f, grid.GetHeight() - 0.5f);
        vertices[13] = new Vector3(- 0.5f, 0.0f, -0.5f);
        vertices[14] = new Vector3(- 0.5f, -m_Height, grid.GetHeight() - 0.5f);
        vertices[15] = new Vector3(- 0.5f, -m_Height, -0.5f);

        uvs[12] = new Vector2(0.0f, 1.0f);
        uvs[13] = new Vector2(1.0f, 1.0f);
        uvs[14] = new Vector2(0.0f, 0.0f);
        uvs[15] = new Vector2(1.0f, 0.0f);

        triangles[18] = 12;
        triangles[19] = 13;
        triangles[20] = 15;
        triangles[21] = 12;
        triangles[22] = 15;
        triangles[23] = 14;

        #endregion

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        m_MeshFilter.mesh = mesh;

        m_MeshRenderer.sharedMaterial = m_Material;
    }
}
