using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GridCreator : MonoBehaviour
{
    private static GridCreator _instance;
    public static GridCreator Instance { get { return _instance; } }

    public int m_Width = 400;
    public int m_Height = 400;
    public float m_CellSize = 1.0f;

    public float3 m_DefaultFlowFieldDirection;

    public static Grid s_Grid;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        if(GameManager.instance != null && GameManager.instance.GetSelectedMapSize() != 0)
        {
            m_Width = m_Height = GameManager.instance.GetSelectedMapSize();
        }
        InitialzeGrid();
    }

    public void InitialzeGrid()
    {
        s_Grid = new Grid(m_Width, m_Height, m_CellSize);
        s_Grid.InitializeFFVectors(byte.MaxValue, m_DefaultFlowFieldDirection);
        s_Grid.InitializeNeighbors();
    }
}
