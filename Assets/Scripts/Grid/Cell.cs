using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GroundType
{
    Grass,
    Rocky,
    Sand
}

public class Cell
{
    // General
    public float xPos;
    public float zPos;

    public int xIndex;
    public int zIndex;

    // Ground
    private GroundType groundType;
    private int[] uvs;

    // Flow Field
    private byte cost; // 1 - 254 (255 == wall/unwakable) (0 == goal)++
    private ushort bestCost;
    private short integration;
    private Vector3 flowFieldDirection;

    public Cell(float _xPos, float _zPos, int _xIndex, int _zIndex, byte _cost)
    {
        xPos = _xPos;
        zPos = _zPos;
        xIndex = _xIndex;
        zIndex = _zIndex;
        cost = _cost;

        bestCost = ushort.MaxValue;

        uvs = new int[4];
    }

    public void SetCost(byte _newCost)
    {
        cost = _newCost;
    }

    public byte GetCost()
    {
        return cost;
    }

    public void SetIntegration(short _newIntegration)
    {
        integration = _newIntegration;
    }

    public short GetIntegration()
    {
        return integration;
    }

    public void SetFlowFieldDirection(Vector3 _newFlowFieldDirection)
    {
        flowFieldDirection = _newFlowFieldDirection;
    }

    public Vector3 GetFlowFieldDirection()
    {
        return flowFieldDirection;
    }

    public void SetBestCost(ushort _bestCost)
    {
        bestCost = _bestCost;
    }

    public ushort GetBestCost()
    {
        return bestCost;
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
}
