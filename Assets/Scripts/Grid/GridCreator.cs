using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GridCreator : MonoBehaviour
{
    public bool useFile;

    public int width = 400;
    public int height = 400;
    public float cellSize = 1.0f;

    public float3 defaultFlowFieldDirection;

    public static Grid grid;
    private FlowField flowField;

    private void Start()
    {
        InitialzeGrid();

        flowField = new FlowField();
    }

    private void Update()
    {
        grid.Update();

        if(Input.GetMouseButtonDown(0))
        {
            Cell clickedCell = getClickedCell();
            if(clickedCell != null)
                flowField.CreateIntegrationField(clickedCell);
        }
    }

    public void InitialzeGrid()
    {

        HandlePreCalculatedFlowFieldFile directions = GetComponent<HandlePreCalculatedFlowFieldFile>();
        if(directions.input != null && useFile)
        {
            grid = new Grid(directions.GetWidth(), directions.GetHeight(), cellSize);
            grid.InitializeFFVectors(directions.GetFlowFieldVectors());
        }
        else
        { 
            grid = new Grid(width, height, cellSize);
            grid.InitializeFFVectors(defaultFlowFieldDirection);
            //grid.InitializeFFVectorsRandom();

            // TEMP: Setup some walls to test avoidance
            /*
            grid.getCell(3, 3).SetCost(255);
            grid.getCell(3, 4).SetCost(255);
            grid.getCell(3, 5).SetCost(255);
            grid.getCell(3, 6).SetCost(255);

            grid.getCell(4, 3).SetCost(255);
            grid.getCell(4, 4).SetCost(255);
            grid.getCell(4, 5).SetCost(255);
            grid.getCell(4, 6).SetCost(255);

            grid.getCell(5, 3).SetCost(255);
            grid.getCell(5, 4).SetCost(255);
            grid.getCell(5, 5).SetCost(255);
            grid.getCell(5, 6).SetCost(255);

            grid.getCell(6, 3).SetCost(255);
            grid.getCell(6, 4).SetCost(255);
            grid.getCell(6, 5).SetCost(255);
            grid.getCell(6, 6).SetCost(255);
            */
        }
    }


    // TEMP
    private Cell getClickedCell()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // TEMP: limit spawn area to grid size
            if (hit.point.x < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.z < -(0.5 * GridCreator.grid.GetCellSize()) ||
                hit.point.x > ((GridCreator.grid.GetWidth() * GridCreator.grid.GetCellSize()) - (GridCreator.grid.GetCellSize() / 2.0f)) ||
                hit.point.z > ((GridCreator.grid.GetHeight() * GridCreator.grid.GetCellSize() - (GridCreator.grid.GetCellSize() / 2.0f))))
            {
                Debug.Log("Unable to get the clicked on cell. Out of bounds!");
            }
            else
            {
                return grid.getCell((int)hit.point.x, (int)hit.point.z);
            }
        }

        return null;
    }
}
