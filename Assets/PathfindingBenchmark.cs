using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum Benchmark { AStarDistance, AStarUnits, FlowFields, None};

public class PathfindingBenchmark : MonoBehaviour
{
    [SerializeField]
    private bool overwriteFile = true;

    [SerializeField]
    private Benchmark benchmarkType;

    [SerializeField]
    private TextMeshProUGUI destination;
    [SerializeField]
    private TextMeshProUGUI time;
    [SerializeField]
    private TextMeshProUGUI unitsUI;
    [SerializeField]
    private TextMeshProUGUI mapSizeUI;

    AStarPathfinding astar;
    FlowField flowfield;
    Task pathfinding;

    // Persistant Values -----
    int x = 0;
    int z = 0;

    int units = 1;

    int mapSize = 8192/2;
    Grid grid;

    private const int distanceArrayMaxWidth = 4;
    private List<float>[] distanceAStarExecutionTimes = new List<float>[401];

    bool distanceAStarFinished = false;
    // -----

    // Headers 
    private string[] ma_DistanceHeaders = new string[2] { "Distance", "ExecutionTime" };
    private string[] ma_UnitsHeaders = new string[2] { "Units", "ExecutionTime" };
    private string[] ma_FlowFieldsHeaders = new string[2] { "Map Size", "ExecutionTime" };
    // -----

    int concurrentCoroutines;

    // TEMP
    int oldx = 400;
    int oldz = 400;

    void Start()
    {
        if(benchmarkType == Benchmark.None)
        {
            Destroy(this.gameObject);
        }
        astar = new AStarPathfinding();
        flowfield = new FlowField();
        concurrentCoroutines = SystemInfo.processorCount - 1;

        for(int i = 0; i < distanceAStarExecutionTimes.Length; i++)
        {
            distanceAStarExecutionTimes[i] = new List<float>();
        }

        if(overwriteFile)
        {
            switch (benchmarkType)
            {
                case Benchmark.AStarDistance: CSVManager.CreateData("AStarBenchmarkDistance.csv", ma_DistanceHeaders); ; break;
                case Benchmark.AStarUnits: CSVManager.CreateData("AStarBenchmarkUnits.csv", ma_UnitsHeaders); break;
                case Benchmark.FlowFields: CSVManager.CreateData("FlowFieldsBenchmark.csv", ma_FlowFieldsHeaders); break;
                case Benchmark.None: break;
                default: Debug.LogError("Unknown Benchmark Type."); break;
            }
        }
    }

    private void Update()
    {
        switch(benchmarkType)
        {
            case Benchmark.AStarDistance: AStarDistance(); break;
            case Benchmark.AStarUnits: AStarUnits(); break;
            case Benchmark.FlowFields: FlowFields(); break;
            default: Debug.LogError("Unknown Benchmark Type."); break;
        }
    }

    #region FlowFields

    private void FlowFields()
    {
        if (mapSize <= 8192)
        {
            grid = new Grid(mapSize, mapSize, 1.0f);
            grid.InitializeNeighbors();

            Debug.Log(mapSize);
            float averageExecutionTime = 0.0f;

            for (int i = 0; i < byte.MaxValue; i++)
            {
                float startTime = Time.realtimeSinceStartup;
                flowfield.FlowFieldPathfinding((byte)i, grid.getCell(0, 0));
                float endTime = Time.realtimeSinceStartup;

                averageExecutionTime += (endTime - startTime);
            }

            averageExecutionTime /= byte.MaxValue;

            string[] data = new string[2]
            {
                grid.GetWidth().ToString() + "x" + grid.GetHeight().ToString(),
                averageExecutionTime.ToString()
            };
            CSVManager.AppendToData("FlowFieldsBenchmark.csv", ma_FlowFieldsHeaders, data);


            destination.text = "0, 0";
            time.text = averageExecutionTime.ToString("F4") + "s";
            mapSizeUI.text = mapSize + ", " + mapSize;

            mapSize *= 2;
        }
    }

    #endregion

    #region AStarUnits

    private void AStarUnits()
    {
        float startTime = Time.realtimeSinceStartup;
        for (int i = 0; i < units; i++)
        {
            List<Cell> path = astar.FindPath(GridCreator.grid.getCellFromPosition(0, 0), GridCreator.grid.getCell(x, z));
        }
        float endTime = Time.realtimeSinceStartup;

        string[] data = new string[2]
        {
                        units.ToString(),
                        (endTime - startTime).ToString()
        };
        CSVManager.AppendToData("AStarBenchmarkUnits.csv", ma_UnitsHeaders, data);
        destination.text = 200 + ", " + 200;
        time.text = (endTime - startTime).ToString("F4") + "s";
        unitsUI.text = units.ToString();

        units++;
    }

    #endregion

    #region AStarDistance
    private void AStarDistance()
    {
        if (!distanceAStarFinished)
        {
            for (int i = 0; i < concurrentCoroutines; i++)
            {
                if (x < distanceArrayMaxWidth)
                {
                    pathfinding = new Task(AStarPathfindingDistance(x, z));
                    if (oldx == x && oldz == z)
                    {
                        Debug.Break();
                    }
                    oldx = x;
                    oldz = z;

                    if (z < GridCreator.grid.GetHeight() - 1)
                    {
                        z++;
                    }
                    else
                    {
                        z = 0;
                        x++;
                    }
                }
                else
                {
                    distanceAStarFinished = true;
                }
            }

            if(distanceAStarFinished)
            {
                new Task(CalculateAverageAndSaveToFile());
            }
        }
    }

    private IEnumerator AStarPathfindingDistance(int _x, int _z)
    {
        float startTime = Time.realtimeSinceStartup;
        List<Cell> path = astar.FindPath(GridCreator.grid.getCellFromPosition(0, 0), GridCreator.grid.getCell(_x, _z));
        float endTime = Time.realtimeSinceStartup;

        /*
        if (path != null)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                //Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos) * 10f + Vector3.one * 5f, Color.green, 1000.0f);
                Debug.DrawLine(new Vector3(path[i].xPos, 0.0f, path[i].zPos), new Vector3(path[i + 1].xPos, 0.0f, path[i + 1].zPos), Color.green, 10.0f);
            }
        }
        */

        distanceAStarExecutionTimes[path.Count].Add(endTime - startTime);

        destination.text = _x.ToString() + ", " + _z.ToString();
        time.text = (endTime - startTime).ToString("F4") + "s";

        yield return null;
    }

    private IEnumerator CalculateAverageAndSaveToFile()
    {
        Debug.Log("CalculateAverageAndSaveToFile");

        for (int i = 1; i < distanceAStarExecutionTimes.Length; i++)
        {
            float averageExecutionTime = 0.0f;
            for(int j = 0; j < distanceAStarExecutionTimes[i].Count; j++)
            {
                averageExecutionTime += distanceAStarExecutionTimes[i][j];
            }
            averageExecutionTime /= distanceAStarExecutionTimes[i].Count;

            Debug.Log("Average Execution Time for distance " + i + " is:" + averageExecutionTime);

            string[] data = new string[2]
            {
                        i.ToString(),
                        averageExecutionTime.ToString()
            };
            CSVManager.AppendToData("AStarBenchmarkDistance.csv", ma_DistanceHeaders, data);

            yield return null;
        }

        yield return null;
    }
    #endregion
}
