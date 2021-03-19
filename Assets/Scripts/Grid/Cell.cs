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
    Grid m_Grid;

    public float m_XPos;
    public float m_ZPos;

    public int m_XIndex;
    public int m_ZIndex;

    private List<Cell> m_Neighbors;

    // Ground
    private GroundType m_GroundType;
    private int[] m_UVs;

    // Flow Field
    private byte m_Cost; // 1 - 254 (255 == wall/unwakable)
    private byte m_OriginalCost;
    private ushort m_Integration;
    private Vector3[] m_FlowFieldDirections;

    // A*
    private int m_GCost;
    private int m_HCost;
    private int m_FCost;

    private Cell m_PrevCell;

    public Cell(Grid _grid, float _xPos, float _zPos, int _xIndex, int _zIndex, byte _cost)
    {
        // General
        m_Grid = _grid;

        m_XPos = _xPos;
        m_ZPos = _zPos;
        m_XIndex = _xIndex;
        m_ZIndex = _zIndex;

        m_UVs = new int[4];

        // FlowField
        m_Cost = _cost;
        m_OriginalCost = m_Cost;

        m_Integration = ushort.MaxValue;

        m_FlowFieldDirections = new Vector3[byte.MaxValue + 1];

        // A*
        m_GCost = int.MaxValue;
        m_PrevCell = null;
    }

    #region General

    public override string ToString()
    {
        return m_XPos + ", " + m_ZPos;
    }

    public void SetGroundType(GroundType _groundType)
    {
        m_GroundType = _groundType;
    }

    public GroundType GetGroundType()
    {
        return m_GroundType;
    }

    public void SetUVs(int[] _uvs)
    {
        m_UVs = _uvs;
    }

    public int[] GetUVs()
    {
        return m_UVs;
    }

    public List<Cell> GetNeighbors()
    {
        return m_Neighbors;
    }

    // Call once after all cells are initialized.
    public void SetNeighbors()
    {
        List<Cell> neighbors = new List<Cell>();

        if (this.m_XIndex - 1 >= 0)
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex));

            if (this.m_ZIndex - 1 >= 0)
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex - 1));
            }
            if (this.m_ZIndex + 1 < m_Grid.GetHeight())
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex + 1));
            }
        }
        if (this.m_XIndex + 1 < m_Grid.GetWidth())
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex));

            if (this.m_ZIndex - 1 >= 0)
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex - 1));
            }
            if (this.m_ZIndex + 1 < m_Grid.GetHeight())
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex + 1));
            }
        }

        if (this.m_ZIndex - 1 >= 0)
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex, this.m_ZIndex - 1));
        }
        if (this.m_ZIndex + 1 < m_Grid.GetHeight())
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex, this.m_ZIndex + 1));
        }

        m_Neighbors = neighbors;
    }

    public IEnumerator SetNeighborsMultithreaded()
    {
        List<Cell> neighbors = new List<Cell>();

        if (this.m_XIndex - 1 >= 0)
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex));

            if (this.m_ZIndex - 1 >= 0)
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex - 1));
            }
            if (this.m_ZIndex + 1 < m_Grid.GetHeight())
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex - 1, this.m_ZIndex + 1));
            }
        }
        if (this.m_XIndex + 1 < m_Grid.GetWidth())
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex));

            if (this.m_ZIndex - 1 >= 0)
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex - 1));
            }
            if (this.m_ZIndex + 1 < m_Grid.GetHeight())
            {
                neighbors.Add(m_Grid.getCell(this.m_XIndex + 1, this.m_ZIndex + 1));
            }
        }

        if (this.m_ZIndex - 1 >= 0)
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex, this.m_ZIndex - 1));
        }
        if (this.m_ZIndex + 1 < m_Grid.GetHeight())
        {
            neighbors.Add(m_Grid.getCell(this.m_XIndex, this.m_ZIndex + 1));
        }

        m_Neighbors = neighbors;
        yield return null;
    }

    #endregion

    #region FlowFields
    public void SetCost(byte _newCost)
    {
        m_Cost = _newCost;
    }

    public byte GetCost()
    {
        return m_Cost;
    }

    public void SetOriginalCost(byte _newOriginalCost)
    {
        m_OriginalCost = _newOriginalCost;
    }

    public byte GetOriginalCost()
    {
        return m_OriginalCost;
    }

    public void SetIntegration(ushort _newIntegration)
    {
        m_Integration = _newIntegration;
    }

    public ushort GetIntegration()
    {
        return m_Integration;
    }

    public void SetFlowFieldDirection(byte _index, Vector3 _newFlowFieldDirection)
    {
        m_FlowFieldDirections[_index] = _newFlowFieldDirection;
    }

    public Vector3 GetFlowFieldDirection(byte _index)
    {
        return m_FlowFieldDirections[_index];
    }

    #endregion

    #region A*

    public void CalculateFCost()
    {
        m_FCost = m_GCost + m_HCost;
    }

    public void SetGCost(int newGCost)
    {
        m_GCost = newGCost;
    }
    
    public int GetGCost()
    {
        return m_GCost;
    }
    public void SetHCost(int newHCost)
    {
        m_HCost = newHCost;
    }

    public int GetHCost()
    {
        return m_HCost;
    }
    public void SetFCost(int newFCost)
    {
        m_FCost = newFCost;
    }

    public int GetFCost()
    {
        return m_FCost;
    }

    public void SetPrevCell(Cell newPrevCell)
    {
        m_PrevCell = newPrevCell;
    }

    public Cell GetPrevCell()
    {
        return m_PrevCell;
    }

    #endregion

}
