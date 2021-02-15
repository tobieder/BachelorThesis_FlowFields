using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GridCreator : MonoBehaviour
{
    private static GridCreator _instance;
    public static GridCreator Instance { get { return _instance; } }

    public bool useFile;

    public int width = 400;
    public int height = 400;
    public float cellSize = 1.0f;

    public float3 defaultFlowFieldDirection;

    public static Grid grid;

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

    private void Start()
    {
        InitialzeGrid();
    }

    public void InitialzeGrid()
    {

        HandlePreCalculatedFlowFieldFile directions = GetComponent<HandlePreCalculatedFlowFieldFile>();
        if(directions.input != null && useFile)
        {
            grid = new Grid(directions.GetWidth(), directions.GetHeight(), cellSize);
            grid.InitializeFFVectors(byte.MaxValue, directions.GetFlowFieldVectors());
            grid.InitializeNeighbors();
        }
        else
        { 
            grid = new Grid(width, height, cellSize);
            grid.InitializeFFVectors(byte.MaxValue, defaultFlowFieldDirection);
            grid.InitializeNeighbors();
        }
    }
}
