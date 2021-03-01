using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP
using UnityEditor;

public class GroundMesh : MonoBehaviour
{
    public Material textureAtlas;

    private GameObject groundMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;

    void Start()
    {
        CreateGroundMesh();
    }

    private void CreateGroundMesh()
    {
        if (!groundMesh)
        {
            groundMesh = new GameObject("GroundMesh");
            groundMesh.transform.parent = this.transform;

            meshRenderer = groundMesh.AddComponent<MeshRenderer>();
        }
        if (!meshFilter)
        {
            meshFilter = groundMesh.GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                meshFilter = groundMesh.AddComponent<MeshFilter>();
            }
        }
        if (!meshRenderer)
        {
            meshRenderer = groundMesh.GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                meshRenderer = groundMesh.AddComponent<MeshRenderer>();
            }
        }
        if(!meshCollider)
        {
            meshCollider = groundMesh.GetComponent<MeshCollider>();
            if (!meshCollider)
            {
                meshCollider = groundMesh.AddComponent<MeshCollider>();
            }
        }

        groundMesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        Grid grid = GridCreator.grid;

        Vector3[] vertices = new Vector3[4 + (((grid.GetWidth() - 1) * 2) * 2) + (((grid.GetHeight() - 1) * 2) * 2) + (((grid.GetWidth() - 1) * (grid.GetHeight() - 1)) * 4)];
        Vector2[] uvs = new Vector2[4 + (((grid.GetWidth() - 1) * 2) * 2) + (((grid.GetHeight() - 1) * 2) * 2) + (((grid.GetWidth() - 1) * (grid.GetHeight() - 1)) * 4)];
        int[] triangles = new int[(grid.GetWidth() * grid.GetHeight()) * 2 * 3];

        // Set Vertices
        int vertexCount = 0;
        for(int z = 0; z < grid.GetHeight(); z++)
        {
            for(int x = 0; x < grid.GetWidth(); x++)
            {
                Cell currCell = grid.getCell(x, z);
                vertices[vertexCount + 0] = new Vector3(currCell.xPos - (grid.GetCellSize() * 0.5f), 0.0f, currCell.zPos - (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 1] = new Vector3(currCell.xPos - (grid.GetCellSize() * 0.5f), 0.0f, currCell.zPos + (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 2] = new Vector3(currCell.xPos + (grid.GetCellSize() * 0.5f), 0.0f, currCell.zPos + (grid.GetCellSize() * 0.5f));
                vertices[vertexCount + 3] = new Vector3(currCell.xPos + (grid.GetCellSize() * 0.5f), 0.0f, currCell.zPos - (grid.GetCellSize() * 0.5f));

                vertexCount += 4;
            }
        }

        // Set UVs
        Vector2[] curruvs;
        float scale = 10.0f;
        int uvIterator = 0;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                Cell currCell = GridCreator.grid.getCell(x, z);
                float perlin = Mathf.PerlinNoise((float)x / (float)GridCreator.grid.GetWidth() * scale, (float)z / (float)GridCreator.grid.GetHeight() * scale);
                if (perlin < 0.4f)
                {
                    curruvs = GetUVs(GroundType.Grass);
                    currCell.SetCost(1);
                    currCell.SetOriginalCost(1);
                }
                else if(perlin < 0.6f)
                {
                    curruvs = GetUVs(GroundType.DarkGrass);
                    currCell.SetCost(2);
                    currCell.SetOriginalCost(2);
                }
                else
                {
                    curruvs = GetUVs(GroundType.Sand);
                    currCell.SetCost(4);
                    currCell.SetOriginalCost(4);
                }

                uvs[uvIterator + 0] = curruvs[0];
                uvs[uvIterator + 1] = curruvs[1];
                uvs[uvIterator + 2] = curruvs[2];
                uvs[uvIterator + 3] = curruvs[3];

                uvIterator += 4;
            }
        }

        // Set Triangles
        int triangleCount = 0;
        int triIterator = 0;
        for(int z = 0; z < grid.GetWidth(); z++)
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

        meshFilter.mesh = mesh;

        meshRenderer.sharedMaterial = textureAtlas;

        meshCollider.sharedMesh = mesh;

        Vector3[] normals = new Vector3[vertices.Length];
        for(int i = 0; i < normals.Length; i++)
        {
            normals[i] = new Vector3(0.0f, 1.0f, 0.0f);
        }
        mesh.normals = normals;

        mesh.RecalculateNormals();
    }

    public Vector2[] GetUVs(GroundType _groundType)
    {
        float imgPerSite = 4.0f;

        Vector2[] ret = new Vector2[4];

        switch(_groundType)
        {
            case GroundType.Grass:
                ret[0] = new Vector2(0.0f / imgPerSite, 3.0f / imgPerSite);    //links unten
                ret[1] = new Vector2(1.0f / imgPerSite, 3.0f / imgPerSite);    //rechts unten
                ret[2] = new Vector2(1.0f / imgPerSite, 4.0f / imgPerSite);    //rechts oben
                ret[3] = new Vector2(0.0f / imgPerSite, 4.0f / imgPerSite);    //links oben
                break;
            case GroundType.Sand:
                ret[0] = new Vector2(3.0f / imgPerSite, 1.0f / imgPerSite);
                ret[1] = new Vector2(4.0f / imgPerSite, 1.0f / imgPerSite);
                ret[2] = new Vector2(4.0f / imgPerSite, 2.0f / imgPerSite);
                ret[3] = new Vector2(3.0f / imgPerSite, 2.0f / imgPerSite);
                break;
            case GroundType.DarkGrass:
                ret[0] = new Vector2(3.0f / imgPerSite, 3.0f / imgPerSite);
                ret[1] = new Vector2(4.0f / imgPerSite, 3.0f / imgPerSite);
                ret[2] = new Vector2(4.0f / imgPerSite, 4.0f / imgPerSite);
                ret[3] = new Vector2(3.0f / imgPerSite, 4.0f / imgPerSite);
                break;
            default:
                Debug.LogError("Ground Type not available.");
                break;
        }

        return ret;
    }
}
