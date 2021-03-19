using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Cell> m_OpenList;
    private List<Cell> m_ClosedList;

    public AStarPathfinding()
    {

    }

    public List<Cell> FindPath(Grid _grid, Cell _start, Cell _end)
    {
        m_OpenList = new List<Cell> { _start };
        m_ClosedList = new List<Cell>();

        ResetCells(_grid);

        _start.SetGCost(0);
        _start.SetHCost(CalculateDistanceCost(_start, _end));
        _start.CalculateFCost();

        while(m_OpenList.Count > 0)
        {
            Cell currCell = GetLowestFCostCell(m_OpenList);
            if(currCell == _end)
            {
                return CalculatePath(_end);
            }

            m_OpenList.Remove(currCell);
            m_ClosedList.Add(currCell);

            List<Cell> neighbors = currCell.GetNeighbors();

            foreach (Cell neighbourCell in neighbors)
            {
                if (m_ClosedList.Contains(neighbourCell))
                {
                    continue;
                }

                if(neighbourCell.GetCost() == byte.MaxValue)
                {
                    m_ClosedList.Add(neighbourCell);
                    continue;
                }

                int tentativeGCost = currCell.GetGCost() + CalculateNeighborCost(currCell, neighbourCell);
                if (tentativeGCost < neighbourCell.GetGCost())
                {
                    neighbourCell.SetPrevCell(currCell);
                    neighbourCell.SetGCost(tentativeGCost);
                    neighbourCell.SetHCost(CalculateDistanceCost(neighbourCell, _end));
                    neighbourCell.CalculateFCost();

                    if(!m_OpenList.Contains(neighbourCell))
                    {
                        m_OpenList.Add(neighbourCell);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    private List<Cell> CalculatePath(Cell goal)
    {
        List<Cell> path = new List<Cell>();

        path.Add(goal);
        Cell currCell = goal;

        while(currCell.GetPrevCell() != null)
        {
            path.Add(currCell.GetPrevCell());
            currCell = currCell.GetPrevCell();
        }

        path.Reverse();

        return path;
    }

    private int CalculateDistanceCost(Cell a, Cell b)
    {
        float xDist = Mathf.Abs(b.m_XPos - a.m_XPos);
        float zDist = Mathf.Abs(b.m_ZPos - a.m_ZPos);

        float remaining = Mathf.Abs(xDist - zDist);

        return (int)((MOVE_DIAGONAL_COST * Mathf.Min(xDist, zDist)) + (MOVE_STRAIGHT_COST * remaining));
    }

    private int CalculateNeighborCost(Cell a, Cell b)
    {
        float xDist = Mathf.Abs(b.m_XPos - a.m_XPos);
        float zDist = Mathf.Abs(b.m_ZPos - a.m_ZPos);

        return (int)((2 * Mathf.Min(xDist, zDist) + (a.GetCost() * 10)));
    }

    private Cell GetLowestFCostCell(List<Cell> cellList)
    {
        Cell lowestFCostCell = cellList[0];

        for(int i = 1; i < cellList.Count; i++)
        {
            if(cellList[i].GetFCost() < lowestFCostCell.GetFCost())
            {
                lowestFCostCell = cellList[i];
            }
        }

        return lowestFCostCell;
    }

    private void ResetCells(Grid _grid)
    {
        for (int x = 0; x < _grid.GetWidth(); x++)
        {
            for (int z = 0; z < _grid.GetHeight(); z++)
            {
                Cell currCell = _grid.getCell(x, z);
                currCell.SetGCost(int.MaxValue);
                currCell.CalculateFCost();
                currCell.SetPrevCell(null);
            }
        }
    }
}
