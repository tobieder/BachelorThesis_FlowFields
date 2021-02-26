using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private List<Cell> openList;
    private List<Cell> closedList;

    public AStarPathfinding()
    {

    }

    public List<Cell> FindPath(Grid _grid, Cell _start, Cell _end)
    {
        openList = new List<Cell> { _start };
        closedList = new List<Cell>();

        ResetCells(_grid);

        _start.SetGCost(0);
        _start.SetHCost(CalculateDistanceCost(_start, _end));
        _start.CalculateFCost();

        while(openList.Count > 0)
        {
            Cell currCell = GetLowestFCostCell(openList);
            if(currCell == _end)
            {
                return CalculatePath(_end);
            }

            openList.Remove(currCell);
            closedList.Add(currCell);

            List<Cell> neighbors = currCell.GetNeighbors();

            foreach (Cell neighbourCell in neighbors)
            {
                if (closedList.Contains(neighbourCell))
                {
                    continue;
                }

                if(neighbourCell.GetCost() == byte.MaxValue)
                {
                    closedList.Add(neighbourCell);
                    continue;
                }

                int tentativeGCost = currCell.GetGCost() + CalculateDistanceCost(currCell, neighbourCell);
                if(tentativeGCost < neighbourCell.GetGCost())
                {
                    neighbourCell.SetPrevCell(currCell);
                    neighbourCell.SetGCost(tentativeGCost);
                    neighbourCell.SetHCost(CalculateDistanceCost(neighbourCell, _end));
                    neighbourCell.CalculateFCost();

                    if(!openList.Contains(neighbourCell))
                    {
                        openList.Add(neighbourCell);
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
        float xDist = Mathf.Abs(b.xPos - a.xPos);
        float zDist = Mathf.Abs(b.zPos - a.zPos);

        float remaining = Mathf.Abs(xDist - zDist);

        return (int)(MOVE_DIAGONAL_COST * Mathf.Min(xDist, zDist) + MOVE_STRAIGHT_COST * remaining);
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
