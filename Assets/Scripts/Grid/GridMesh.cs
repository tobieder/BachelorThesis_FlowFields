using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMesh : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float gridPercentage = 0.1f;
    public Material gridMeshMaterial;

    //public bool showVertexIndices;

    private GameObject gridMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        GridCreator gridCreator = GetComponent<GridCreator>();

        MeshUpdateEditor();
    }

    public void MeshUpdateEditor()
    {
        GridCreator gridCreator = GetComponent<GridCreator>();
        if (GridCreator.grid == null)
        {
            gridCreator.InitialzeGrid();
        }

        Debug.Log(GridCreator.grid.getCell(0, 0).flowFieldDirection);

        if (!gridMesh)
        {
            gridMesh = new GameObject("GridMesh");
            gridMesh.transform.parent = this.transform;

            meshRenderer = gridMesh.AddComponent<MeshRenderer>();
        }
        if(!meshFilter)
        {
            meshFilter = gridMesh.GetComponent<MeshFilter>();
            if (!meshFilter)
            {
                meshFilter = gridMesh.AddComponent<MeshFilter>();
            }
        }
        if (!meshRenderer)
        {
            meshRenderer = gridMesh.GetComponent<MeshRenderer>();
            if (!meshRenderer)
            {
                meshRenderer = gridMesh.AddComponent<MeshRenderer>();
            }
        }

        CreateMesh(GridCreator.grid, gridCreator);
    }

    void CreateMesh(Grid grid, GridCreator gridCreator)
    {
        Vector3[] vertices = new Vector3[(gridCreator.width * gridCreator.height * 4) + 4];
        Vector2[] uvs = new Vector2[(gridCreator.width * gridCreator.height * 4) + 4];
        int[] triangles = new int[gridCreator.width * gridCreator.height * 6 * 3 + (8 * 3)];

        float halfCellsize = gridCreator.cellSize / 2.0f;

        // ----- Set Vertices -----
        int vertexCount = 0;
        for (int z = 0; z < gridCreator.height; z++)
        {
            for (int x = 0; x < gridCreator.width; x++)
            {
                vertices[vertexCount + 0] = grid.GetWorldPositon(x, z) + (new Vector3(-halfCellsize, 0.0f, -halfCellsize) * gridPercentage);
                vertices[vertexCount + 1] = grid.GetWorldPositon(x, z) + (new Vector3(-halfCellsize, 0.0f, halfCellsize) * gridPercentage);
                vertices[vertexCount + 2] = grid.GetWorldPositon(x, z) + (new Vector3(halfCellsize, 0.0f, halfCellsize) * gridPercentage);
                vertices[vertexCount + 3] = grid.GetWorldPositon(x, z) + (new Vector3(halfCellsize, 0.0f, -halfCellsize) * gridPercentage);

                /*
                if (showVertexIndices)
                {
                    WorldText.CreateWorldText(this.transform, (vertexCount + 0).ToString(), vertices[vertexCount + 0], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                    WorldText.CreateWorldText(this.transform, (vertexCount + 1).ToString(), vertices[vertexCount + 1], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                    WorldText.CreateWorldText(this.transform, (vertexCount + 2).ToString(), vertices[vertexCount + 2], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                    WorldText.CreateWorldText(this.transform, (vertexCount + 3).ToString(), vertices[vertexCount + 3], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
                }
                */
                vertexCount += 4;
            }
        }

        // ----- Set Triangles -----
        int triangleCount = 0;

        for (int z = 1; z < gridCreator.height; z++)
        {
            triangles[triangleCount + 0] = (z * gridCreator.width) * 4;
            triangles[triangleCount + 1] = ((z * gridCreator.width) - gridCreator.width) * 4 + 2;
            triangles[triangleCount + 2] = ((z * gridCreator.width) - gridCreator.width) * 4 + 1;

            triangles[triangleCount + 3] = (z * gridCreator.width) * 4;
            triangles[triangleCount + 4] = (z * gridCreator.width) * 4 + 3;
            triangles[triangleCount + 5] = ((z * gridCreator.width) - gridCreator.width) * 4 + 2;


            triangleCount += 6;
        }
        for (int x = 1; x < gridCreator.width; x++)
        {
            triangles[triangleCount + 0] = x * 4;
            triangles[triangleCount + 1] = x * 4 - 1;
            triangles[triangleCount + 2] = x * 4 - 2;

            triangles[triangleCount + 3] = x * 4;
            triangles[triangleCount + 4] = x * 4 - 2;
            triangles[triangleCount + 5] = x * 4 + 1;

            triangleCount += 6;
        }

        int counter = gridCreator.width + 1;
        for (int z = 1; z < gridCreator.height; z++)
        {
            for (int x = 1; x < gridCreator.width; x++)
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
                triangles[triangleCount + 2] = currIndex - (gridCreator.width * 4) + 2;         //6

                triangles[triangleCount + 3] = currIndex;                                       //16
                triangles[triangleCount + 4] = currIndex - (gridCreator.width * 4) + 2;         //6
                triangles[triangleCount + 5] = currIndex - (gridCreator.width * 4) + 1;         //5

                triangleCount += 6;

                // Square
                triangles[triangleCount + 0] = currIndex;
                triangles[triangleCount + 1] = currIndex - (gridCreator.width * 4) + 1;
                triangles[triangleCount + 2] = currIndex - (gridCreator.width * 4) - 2;

                triangles[triangleCount + 3] = currIndex;
                triangles[triangleCount + 4] = currIndex - (gridCreator.width * 4) - 2;
                triangles[triangleCount + 5] = currIndex - 1;

                triangleCount += 6;

                counter++;
            }
            counter++;
        }

        // ----- Borders -----
        vertices[vertexCount + 0] = grid.GetWorldPositon(0, 0) + (new Vector3(-halfCellsize, 0.0f, -halfCellsize) * (1.0f + (1.0f - gridPercentage)));
        vertices[vertexCount + 1] = grid.GetWorldPositon(gridCreator.width - 1, 0) + (new Vector3(halfCellsize, 0.0f, -halfCellsize) * (1.0f + (1.0f - gridPercentage)));
        vertices[vertexCount + 2] = grid.GetWorldPositon(gridCreator.width - 1, gridCreator.height - 1) + (new Vector3(halfCellsize, 0.0f, halfCellsize) * (1.0f + (1.0f - gridPercentage)));
        vertices[vertexCount + 3] = grid.GetWorldPositon(0, gridCreator.height - 1) + (new Vector3(-halfCellsize, 0.0f, halfCellsize) * (1.0f + (1.0f - gridPercentage)));

        /*
        if (showVertexIndices)
        {
            WorldText.CreateWorldText(this.transform, (vertexCount + 0).ToString(), vertices[vertexCount + 0], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
            WorldText.CreateWorldText(this.transform, (vertexCount + 1).ToString(), vertices[vertexCount + 1], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
            WorldText.CreateWorldText(this.transform, (vertexCount + 2).ToString(), vertices[vertexCount + 2], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
            WorldText.CreateWorldText(this.transform, (vertexCount + 3).ToString(), vertices[vertexCount + 3], 10, Color.red, TextAnchor.MiddleCenter, TextAlignment.Center, 5000);
        }
        */

        triangles[triangleCount + 0] = 0;
        triangles[triangleCount + 1] = vertexCount + 1;
        triangles[triangleCount + 2] = vertexCount + 0;

        triangles[triangleCount + 3] = 0;
        triangles[triangleCount + 4] = (gridCreator.width - 1) * 4 + 3;
        triangles[triangleCount + 5] = vertexCount + 1;
        triangleCount += 6;

        triangles[triangleCount + 0] = (gridCreator.width - 1) * 4 + 3;
        triangles[triangleCount + 1] = vertexCount + 2;
        triangles[triangleCount + 2] = vertexCount + 1;

        triangles[triangleCount + 3] = (gridCreator.width - 1) * 4 + 3;
        triangles[triangleCount + 4] = ((gridCreator.width * gridCreator.height) - 1) * 4 + 2;
        triangles[triangleCount + 5] = vertexCount + 2;
        triangleCount += 6;

        triangles[triangleCount + 0] = ((gridCreator.width * gridCreator.height) - 1) * 4 + 2;
        triangles[triangleCount + 1] = vertexCount + 3;
        triangles[triangleCount + 2] = vertexCount + 2;

        triangles[triangleCount + 3] = ((gridCreator.width * gridCreator.height) - 1) * 4 + 2;
        triangles[triangleCount + 4] = (((gridCreator.height - 1) * gridCreator.width)) * 4 + 1;
        triangles[triangleCount + 5] = vertexCount + 3;
        triangleCount += 6;

        triangles[triangleCount + 0] = (((gridCreator.height - 1) * gridCreator.width)) * 4 + 1;
        triangles[triangleCount + 1] = vertexCount + 0;
        triangles[triangleCount + 2] = vertexCount + 3;

        triangles[triangleCount + 3] = (((gridCreator.height - 1) * gridCreator.width)) * 4 + 1;
        triangles[triangleCount + 4] = 0;
        triangles[triangleCount + 5] = vertexCount + 0;
        triangleCount += 6;


        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        meshFilter.mesh = mesh;

        meshRenderer.sharedMaterial = gridMeshMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
