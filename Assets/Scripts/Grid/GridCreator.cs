using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCreator : MonoBehaviour
{
    public int width = 400;
    public int height = 400;
    public float cellSize = 1.0f;

    Grid grid;

    private void Start()
    {
        grid = new Grid(width, height, cellSize);
    }

    private void Update()
    {
        grid.Update();
    }

    public Grid GetGrid()
    {
        return grid;
    }
}
