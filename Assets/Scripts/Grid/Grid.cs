using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;

    private float cellSize;

    private Cell[,] gridArray;
    private TextMesh[,] debugTextArray;

    public struct Cell
    {
        public int xPos;
        public int zPos;

        public bool walkable;

        public Vector3 flowFieldDirection;
    }

    public Grid(int _width, int _height, float _cellSize)
    {
        width = _width;
        height = _height;
        cellSize = _cellSize;

        gridArray = new Cell[width, height];
        //debugTextArray = new TextMesh[width, height];

        Vector3 _gridOffset = new Vector3(cellSize, 0.0f, cellSize) * 0.5f;

        Debug.Log(cellSize);

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z].xPos = x;
                gridArray[x, z].zPos = z;
                gridArray[x, z].walkable = true;
                gridArray[x, z].flowFieldDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f));

                //debugTextArray[x, z] = TestingScript.CreateWorldText(gridArray[x, z].flowFieldDirection.ToString() , null, GetWorldPositon(x, z), 50, Color.white, TextAnchor.MiddleCenter);
                Debug.DrawLine(GetWorldPositon(x, z) - _gridOffset, GetWorldPositon(x, z + 1) - _gridOffset, Color.white, 100.0f);
                Debug.DrawLine(GetWorldPositon(x, z) - _gridOffset, GetWorldPositon(x + 1, z) - _gridOffset, Color.white, 100.0f);

            }
        }
        Debug.DrawLine(GetWorldPositon(0, height) - _gridOffset, GetWorldPositon(width, height) - _gridOffset, Color.white, 100f);
        Debug.DrawLine(GetWorldPositon(width, 0) - _gridOffset, GetWorldPositon(width, height) - _gridOffset, Color.white, 100f);
    }

    public void Update()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                DrawArrow.ForDebug(GetWorldPositon(x, z), gridArray[x, z].flowFieldDirection, Color.red);
            }
        }
    }

    public Cell[,] GetCellArray()
    {
        return gridArray;
    }

    public void SetValue(int _x, int _z, Vector3 value)
    {
        if (_x >= 0 && _x < width && _z >= 0 && _z < height)
        {
            gridArray[_x, _z].flowFieldDirection = value;
            //debugTextArray[_x, _z].text = gridArray[_x, _z].flowFieldDirection.ToString();
        }
    }

    private Vector3 GetWorldPositon(int x, int z)
    {
        return new Vector3(x, 0.0f, z) * cellSize;
    }
}
