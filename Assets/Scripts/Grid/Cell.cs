using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    Grass,
    DarkGrass,
    Sand,
    Rocky
}

public class Cell
{
    // General
    Grid grid;

    public float xPos;
    public float zPos;

    public int xIndex;
    public int zIndex;

    private List<Cell> m_Neighbors;

    // Ground
    private GroundType groundType;
    private int[] uvs;

    // Flow Field
    private byte cost; // 1 - 254 (255 == wall/unwakable) (0 == goal)++
    private byte originalCost;
    private ushort integration;
    private Vector3[] flowFieldDirections;

    // A*
    private int gCost;
    private int hCost;
    private int fCost;

    private Cell prevCell;

    public Cell(Grid _grid, float _xPos, float _zPos, int _xIndex, int _zIndex, byte _cost)
    {
        // General
        grid = _grid;

        xPos = _xPos;
        zPos = _zPos;
        xIndex = _xIndex;
        zIndex = _zIndex;

        uvs = new int[4];

        // FLowField
        cost = _cost;
        originalCost = cost;

        integration = ushort.MaxValue;

        flowFieldDirections = new Vector3[byte.MaxValue + 1];

        // A*
        gCost = int.MaxValue;
        prevCell = null;
    }

    #region General

    public override string ToString()
    {
        return xPos + ", " + zPos;
    }

    public void SetGroundType(GroundType _groundType)
    {
        groundType = _groundType;
    }

    public GroundType GetGroundType()
    {
        return groundType;
    }

    public void SetUVs(int[] _uvs)
    {
        uvs = _uvs;
    }

    public int[] GetUVs()
    {
        return uvs;
    }

    public List<Cell> GetNeighbors()
    {
        return m_Neighbors;
    }

    // Call once after all cells are initialized.
    public void SetNeighbors()
    {
        List<Cell> neighbors = new List<Cell>();

        if (this.xIndex - 1 >= 0)
        {
            neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex));

            if (this.zIndex - 1 >= 0)
            {
                neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex - 1));
            }
            if (this.zIndex + 1 < grid.GetHeight())
            {
                neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex + 1));
            }
        }
        if (this.xIndex + 1 < grid.GetWidth())
        {
            neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex));

            if (this.zIndex - 1 >= 0)
            {
                neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex - 1));
            }
            if (this.zIndex + 1 < grid.GetHeight())
            {
                neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex + 1));
            }
        }

        if (this.zIndex - 1 >= 0)
        {
            neighbors.Add(grid.getCell(this.xIndex, this.zIndex - 1));
        }
        if (this.zIndex + 1 < grid.GetHeight())
        {
            neighbors.Add(grid.getCell(this.xIndex, this.zIndex + 1));
        }

        m_Neighbors = neighbors;
    }

    public IEnumerator SetNeighborsMultithreaded()
    {
        List<Cell> neighbors = new List<Cell>();

        if (this.xIndex - 1 >= 0)
        {
            neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex));

            if (this.zIndex - 1 >= 0)
            {
                neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex - 1));
            }
            if (this.zIndex + 1 < grid.GetHeight())
            {
                neighbors.Add(grid.getCell(this.xIndex - 1, this.zIndex + 1));
            }
        }
        if (this.xIndex + 1 < grid.GetWidth())
        {
            neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex));

            if (this.zIndex - 1 >= 0)
            {
                neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex - 1));
            }
            if (this.zIndex + 1 < grid.GetHeight())
            {
                neighbors.Add(grid.getCell(this.xIndex + 1, this.zIndex + 1));
            }
        }

        if (this.zIndex - 1 >= 0)
        {
            neighbors.Add(grid.getCell(this.xIndex, this.zIndex - 1));
        }
        if (this.zIndex + 1 < grid.GetHeight())
        {
            neighbors.Add(grid.getCell(this.xIndex, this.zIndex + 1));
        }

        m_Neighbors = neighbors;
        yield return null;
    }

    #endregion

    #region FlowFields
    public void SetCost(byte _newCost)
    {
        cost = _newCost;
    }

    public byte GetCost()
    {
        return cost;
    }

    public void SetOriginalCost(byte _newOriginalCost)
    {
        originalCost = _newOriginalCost;
    }

    public byte GetOriginalCost()
    {
        return originalCost;
    }

    public void SetIntegration(ushort _newIntegration)
    {
        integration = _newIntegration;
    }

    public ushort GetIntegration()
    {
        return integration;
    }

    public void SetFlowFieldDirection(byte _index, Vector3 _newFlowFieldDirection)
    {
        flowFieldDirections[_index] = _newFlowFieldDirection;
    }

    public Vector3 GetFlowFieldDirection(byte _index)
    {
        return flowFieldDirections[_index];
    }

    #endregion

    #region A*

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetGCost(int newGCost)
    {
        gCost = newGCost;
    }
    
    public int GetGCost()
    {
        return gCost;
    }
    public void SetHCost(int newHCost)
    {
        hCost = newHCost;
    }

    public int GetHCost()
    {
        return hCost;
    }
    public void SetFCost(int newFCost)
    {
        fCost = newFCost;
    }

    public int GetFCost()
    {
        return fCost;
    }

    public void SetPrevCell(Cell newPrevCell)
    {
        prevCell = newPrevCell;
    }

    public Cell GetPrevCell()
    {
        return prevCell;
    }

    #endregion

}
