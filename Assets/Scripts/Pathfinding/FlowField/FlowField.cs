using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    private Cell m_Destination;
    private Grid m_Grid;

    private float m_VectorIntensity = 10.0f;

    public FlowField()
    {
    }

    /// <summary>
    /// Create a Flow Field to the given Destination cell.
    /// </summary>
    /// <param name="_grid">The grid the algorithm runs on.</param>
    /// <param name="_flowMapIndex">Flow Field Layer to attach the result to.</param>
    /// <param name="_destination">Destination of the pathfinding algorithm.</param>
    public void FlowFieldPathfinding(Grid _grid, byte _flowMapIndex, Cell _destination)
    {
        m_Grid = _grid;

        CreateIntegrationField(_destination);
        CreateFlowField(_flowMapIndex);
    }

    private void CreateIntegrationField(Cell _destination)
    {
        ResetIntegrationField();

        m_Destination = _destination;

        // Save the old cost to later reapply it.
        byte originalCost = m_Destination.GetCost();

        // Mark the destination cell.
        m_Destination.SetCost(0);
        m_Destination.SetIntegration(0);

        Queue<Cell> cellsToCheck = new Queue<Cell>();

        cellsToCheck.Enqueue(m_Destination);

        while (cellsToCheck.Count > 0)
        {
            Cell currCell = cellsToCheck.Dequeue();

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

        // Reset to the saved cost value.
        m_Destination.SetCost(originalCost);
    }

    private void CreateFlowField(byte _flowMapIndex)
    {
        foreach(Cell cell in m_Grid.GetGridArray())
        {
            if (cell.GetCost() != byte.MaxValue)
            {
                List<Cell> neighbors = cell.GetNeighbors();

                ushort bestCost = cell.GetIntegration();

                foreach (Cell neighbor in neighbors)
                {
                    if (neighbor.GetIntegration() < bestCost)
                    {
                        bestCost = neighbor.GetIntegration();
                        cell.SetFlowFieldDirection(_flowMapIndex, new Vector3(neighbor.m_XPos - cell.m_XPos, 0.0f, neighbor.m_ZPos - cell.m_ZPos).normalized * m_VectorIntensity);
                    }
                }
            }
            else
            {
                cell.SetFlowFieldDirection(_flowMapIndex, new Vector3(0.0f, 0.0f, 0.0f));
            }
        }
    }

    private void ResetIntegrationField()
    {
        foreach(Cell cell in m_Grid.GetGridArray())
        {
            cell.SetIntegration(ushort.MaxValue);
        }
    }
}