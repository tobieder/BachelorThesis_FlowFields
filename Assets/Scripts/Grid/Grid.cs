using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;

    private float cellSize;

    private Cell[,] gridArray;
    //private TextMesh[,] debugTextArray;

    public Grid(int _width, int _height, float _cellSize)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;

        gridArray = new Cell[width, height];
        //debugTextArray = new TextMesh[width, height];

        Vector3 _gridOffset = new Vector3(cellSize, 0.0f, cellSize) * 0.5f;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                gridArray[x, z] = new Cell(this, x * cellSize, z * cellSize, x, z, 1); // Get cost from ground texture

                gridArray[x, z].SetFlowFieldDirection(byte.MaxValue, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    public void InitializeNeighbors()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z].SetNeighbors();
            }
        }
    }

    public void InitializeFFVectorsRandom(byte _index)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z].SetFlowFieldDirection(_index, new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized * 0.5f);
            }
        }
    }

    public void InitializeFFVectors(byte _index, Vector3 direction)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z].SetFlowFieldDirection(_index, direction);
            }
        }
    }

    public void InitializeFFVectors(byte _index, Vector3[,] directions)
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z].SetFlowFieldDirection(_index, directions[x, z]);
            }
        }
    }

    public Cell getCell(int x, int z)
    {
        if(x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridArray[x, z];
        }

        //Debug.LogError("Grid.GetCell(x, z): x or z value out of Range.");
        return null;
    }

    public Cell getCellFromPosition(float x, float z)
    {
        int _x = Mathf.RoundToInt(x / cellSize);
        int _z = Mathf.RoundToInt(z / cellSize);

        return getCell(_x, _z);
    }

    public void SetValue(byte _index, int _x, int _z, Vector3 value)
    {
        if (_x >= 0 && _x < width && _z >= 0 && _z < height)
        {
            gridArray[_x, _z].SetFlowFieldDirection(_index, value.normalized);
            //debugTextArray[_x, _z].text = gridArray[_x, _z].flowFieldDirection.ToString();
        }
    }

    public Vector3 GetWorldPositon(int x, int z)
    {
        return new Vector3(x, 0.0f, z) * cellSize;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Cell[,] GetGridArray()
    {
        if (gridArray != null)
            return gridArray;
        else
            return null;
    }
}
