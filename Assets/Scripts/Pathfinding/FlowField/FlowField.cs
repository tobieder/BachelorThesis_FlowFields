using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Cell destination;

    public FlowField()
    {

    }

    public void FlowFieldPathfinding(byte _flowMapIndex, Cell _destination)
    {
        CreateIntegrationField(_destination);
        CreateFlowField(_flowMapIndex);
    }

    private void CreateIntegrationField(Cell _destination)
    {
        ResetFlowField();

        destination = _destination;

        byte oldCost = destination.GetCost();

        destination.SetCost(0);
        destination.SetIntegration(0);

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(destination);

        while (cellsToCheck.Count > 0)
        {
            Cell currCell = cellsToCheck.Dequeue();

            //List<Cell> neighbors = GetNeighbors(currCell.xIndex, currCell.zIndex, false);
            List<Cell> neighbors = currCell.GetNeighbors();

            foreach (Cell neighbor in neighbors)
            {
                if (neighbor.GetCost() == byte.MaxValue)
                {
                    continue;
                }
                if (neighbor.GetCost() + currCell.GetIntegration() < neighbor.GetIntegration())
                {
                    neighbor.SetIntegration((ushort)(neighbor.GetCost() + currCell.GetIntegration()));
                    cellsToCheck.Enqueue(neighbor);
                }
            }
        }

        destination.SetCost(oldCost);
    }

    private void CreateFlowField(byte _flowMapIndex)
    {
        foreach(Cell cell in GridCreator.grid.GetGridArray())
        {
            if (cell.GetCost() != byte.MaxValue)
            {
                //List<Cell> neighbors = GetNeighbors(cell.xIndex, cell.zIndex, true);
                List<Cell> neighbors = cell.GetNeighbors();

                ushort bestCost = cell.GetIntegration();

                foreach (Cell neighbor in neighbors)
                {
                    if (neighbor.GetIntegration() < bestCost)
                    {
                        bestCost = neighbor.GetIntegration();
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

    private void ResetFlowField()
    {
        foreach(Cell cell in GridCreator.grid.GetGridArray())
        {
            cell.SetIntegration(ushort.MaxValue);
        }
    }
}