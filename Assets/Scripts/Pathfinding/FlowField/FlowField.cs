using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Cell destination;

    public FlowField()
    {

    }

    public void CreateIntegrationField(byte _flowMapIndex, Cell _destination)
    {
        ResetFlowField();

        destination = _destination;

        byte oldCost = destination.GetCost();

        destination.SetCost(0);
        destination.SetBestCost(0);

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destination);

        while (cellsToCheck.Count > 0)
        {
            Cell currCell = cellsToCheck.Dequeue();
            List<Cell> neighbors = GetNeighbors(currCell.xIndex, currCell.zIndex, false);
            foreach (Cell neighbor in neighbors)
            {
                if (neighbor.GetCost() == byte.MaxValue)
                {
                    continue;
                }
                if (neighbor.GetCost() + currCell.GetBestCost() < neighbor.GetBestCost())
                {
                    neighbor.SetBestCost((ushort)(neighbor.GetCost() + currCell.GetBestCost()));
                    cellsToCheck.Enqueue(neighbor);
                }
            }
        }

        destination.SetCost(oldCost);

        CreateFlowField(_flowMapIndex);
    }

    public void CreateFlowField(byte _flowMapIndex)
    {
        foreach(Cell cell in GridCreator.grid.GetGridArray())
        {
            if (cell.GetCost() != byte.MaxValue)
            {
                List<Cell> neighbors = GetNeighbors(cell.xIndex, cell.zIndex, true);

                ushort bestCost = cell.GetBestCost();

                foreach (Cell neighbor in neighbors)
                {
                    if (neighbor.GetBestCost() < bestCost)
                    {
                        bestCost = neighbor.GetBestCost();
                        cell.SetFlowFieldDirection(_flowMapIndex, new Vector3(neighbor.xPos - cell.xPos, 0.0f, neighbor.zPos - cell.zPos).normalized);
                    }
                }
            }
            else
            {
                cell.SetFlowFieldDirection(_flowMapIndex, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    private List<Cell> GetNeighbors(int _xPos, int _zPos, bool _useOrdinalDirections)
    {
        List<Cell> neighbors = new List<Cell>();

        Cell currCell;

        currCell = GridCreator.grid.getCell(_xPos + 1, _zPos + 0);
        if (currCell != null) neighbors.Add(currCell);
        currCell = GridCreator.grid.getCell(_xPos - 1, _zPos + 0);
        if (currCell != null) neighbors.Add(currCell);
        currCell = GridCreator.grid.getCell(_xPos + 0, _zPos + 1);
        if (currCell != null) neighbors.Add(currCell);
        currCell = GridCreator.grid.getCell(_xPos + 0, _zPos - 1);
        if (currCell != null) neighbors.Add(currCell);

        if(_useOrdinalDirections)
        {
            currCell = GridCreator.grid.getCell(_xPos + 1, _zPos + 1);
            if (currCell != null) neighbors.Add(currCell);
            currCell = GridCreator.grid.getCell(_xPos + 1, _zPos - 1);
            if (currCell != null) neighbors.Add(currCell);
            currCell = GridCreator.grid.getCell(_xPos - 1, _zPos + 1);
            if (currCell != null) neighbors.Add(currCell);
            currCell = GridCreator.grid.getCell(_xPos - 1, _zPos - 1);
            if (currCell != null) neighbors.Add(currCell);
        }

        return neighbors;
    }

    private void ResetFlowField()
    {
        foreach(Cell cell in GridCreator.grid.GetGridArray())
        {
            cell.SetBestCost(ushort.MaxValue);
        }
    }
}
