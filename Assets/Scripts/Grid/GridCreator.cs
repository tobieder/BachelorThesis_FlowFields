using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public int width = 400;
    public int height = 400;
    public float cellSize = 1.0f;

    public static Grid grid;

    private void Start()
    {
        InitialzeGrid();
    }

    private void Update()
    {
        grid.Update();
    }

    public void InitialzeGrid()
    {

        HandlePreCalculatedFlowFieldFile directions = GetComponent<HandlePreCalculatedFlowFieldFile>();
        if(directions.input != null)
        {
            grid = new Grid(directions.GetWidth(), directions.GetHeight(), cellSize);
            grid.InitializeFFVectors(directions.GetFlowFieldVectors());
        }
        else
        { 
            grid = new Grid(width, height, cellSize);
            grid.InitializeFFVectorsRandom();
        }
    }
}
