using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int m_Width;
    private int m_Height;

    private float m_CellSize;

    private Cell[,] m_GridArray;

    public Grid(int _width, int _height, float _cellSize)
    {
        m_Width = _width;
        m_Height = _height;
        m_CellSize = _cellSize;

        m_GridArray = new Cell[m_Width, m_Height];

        for (int x = 0; x < m_Width; x++)
        {
            for (int z = 0; z < m_Height; z++)
            {
                m_GridArray[x, z] = new Cell(this, x * m_CellSize, z * m_CellSize, x, z, 1);
                // Cost will be set from ground texture.

                m_GridArray[x, z].SetFlowFieldDirection(byte.MaxValue, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    public void InitializeNeighbors()
    {
        for (int x = 0; x < m_GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < m_GridArray.GetLength(1); z++)
            {
                m_GridArray[x, z].SetNeighbors();
            }
        }
    }

    public void InitializeFFVectorsRandom(byte _index)
    {
        for (int x = 0; x < m_GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < m_GridArray.GetLength(1); z++)
            {
                m_GridArray[x, z].SetFlowFieldDirection(_index, new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized * 0.5f);
            }
        }
    }

    public void InitializeFFVectors(byte _index, Vector3 direction)
    {
        for (int x = 0; x < m_GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < m_GridArray.GetLength(1); z++)
            {
                m_GridArray[x, z].SetFlowFieldDirection(_index, direction);
            }
        }
    }

    public void InitializeFFVectors(byte _index, Vector3[,] directions)
    {
        for (int x = 0; x < m_GridArray.GetLength(0); x++)
        {
            for (int z = 0; z < m_GridArray.GetLength(1); z++)
            {
                m_GridArray[x, z].SetFlowFieldDirection(_index, directions[x, z]);
            }
        }
    }

    public Cell getCell(int x, int z)
    {
        if(x >= 0 && x < m_Width && z >= 0 && z < m_Height)
        {
            return m_GridArray[x, z];
        }

        return null;
    }

    public Cell getCellFromPosition(float x, float z)
    {
        int _x = Mathf.RoundToInt(x / m_CellSize);
        int _z = Mathf.RoundToInt(z / m_CellSize);

        return getCell(_x, _z);
    }

    public void SetValue(byte _index, int _x, int _z, Vector3 value)
    {
        if (_x >= 0 && _x < m_Width && _z >= 0 && _z < m_Height)
        {
            m_GridArray[_x, _z].SetFlowFieldDirection(_index, value.normalized);
        }
    }

    public Vector3 GetWorldPositon(int x, int z)
    {
        return new Vector3(x, 0.0f, z) * m_CellSize;
    }

    public int GetWidth()
    {
        return m_Width;
    }

    public int GetHeight()
    {
        return m_Height;
    }

    public float GetCellSize()
    {
        return m_CellSize;
    }

    public Cell[,] GetGridArray()
    {
        if (m_GridArray != null)
            return m_GridArray;
        else
            return null;
    }
}
