using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum Benchmark { None, FlowFields, AStarDistance, AStarUnits };

public class PathfindingBenchmark : MonoBehaviour
{
    [SerializeField]
    private bool overwriteFile = true;

    [SerializeField]
    private Benchmark benchmarkType;

    bool benchmarkRunning = false;

    [SerializeField]
    private Window_Graph flowFieldGraph;
    [SerializeField]
    private Window_Graph aStarDistanceGraph;
    [SerializeField]
    private Window_Graph aStarUnitsGraph;

    [SerializeField]
    private RectTransform informationPanel;
    [SerializeField]
    private TMP_Dropdown dropdown;
    [SerializeField]
    Button startButton;
    [SerializeField]
    TMP_InputField inputField;
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

    int mapSize = 100;
    Grid flowFieldGrid;
    Grid aStarUnitsGrid;
    Grid aStarDistanceGrid;

    private List<float>[] distanceAStarExecutionTimes = new List<float>[401];

    bool distanceAStarFinished = false;

    private int maxFlowFieldMapSize = 1;
    private int maxXValue = 1;
    private int maxUnits = 1;
    // -----

    // Headers 
    private string[] ma_DistanceHeaders = new string[2] { "Distance", "ExecutionTime" };
    private string[] ma_UnitsHeaders = new string[2] { "Units", "ExecutionTime" };
    private string[] ma_FlowFieldsHeaders = new string[2] { "Map Size", "ExecutionTime" };
    // -----

    int concurrentCoroutines;

    List<float> flowFieldValues = new List<float>();
    List<float> astarDistanceValues = new List<float>();
    List<float> astarUnitsValues = new List<float>();

    void Start()
    {
        astar = new AStarPathfinding();
        flowfield = new FlowField();
        concurrentCoroutines = SystemInfo.processorCount - 1;

        for(int i = 0; i < distanceAStarExecutionTimes.Length; i++)
        {
            distanceAStarExecutionTimes[i] = new List<float>();
        }

        HandleInputData(0);
    }

    private void Update()
    {
        if (benchmarkRunning)
        {
            switch (benchmarkType)
            {
                case Benchmark.None: break;
                case Benchmark.AStarDistance: AStarDistance(); break;
                case Benchmark.AStarUnits: AStarUnits(); break;
                case Benchmark.FlowFields: FlowFields(); break;
                default: Debug.LogError("Unknown Benchmark Type."); break;
            }
        }
    }

    public void HandleInputData (int val)
    {
        benchmarkType = (Benchmark)val;

        switch(benchmarkType)
        {
            case Benchmark.None:
                {
                    informationPanel.sizeDelta = new Vector2(0.0f, 100);


                    startButton.gameObject.SetActive(false);
                    inputField.gameObject.SetActive(false);

                    destination.transform.parent.gameObject.SetActive(false);
                    time.transform.parent.gameObject.SetActive(false);
                    unitsUI.transform.parent.gameObject.SetActive(false);
                    mapSizeUI.transform.parent.gameObject.SetActive(false);
                    break;
                }
            case Benchmark.FlowFields:
                {
                    flowFieldGraph.gameObject.SetActive(true);
                    aStarDistanceGraph.gameObject.SetActive(false);
                    aStarUnitsGraph.gameObject.SetActive(false);

                    informationPanel.sizeDelta = new Vector2(0.0f, 300);

                    startButton.gameObject.SetActive(true);
                    inputField.gameObject.SetActive(true);
                    inputField.text = "";
                    inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter max. Grid-Size ...";

                    destination.transform.parent.gameObject.SetActive(false);
                    time.transform.parent.gameObject.SetActive(true);
                    //time.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -150.0f);
                    unitsUI.transform.parent.gameObject.SetActive(false);
                    mapSizeUI.transform.parent.gameObject.SetActive(true);
                    //mapSizeUI.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -200.0f);
                    break;
                }
            case Benchmark.AStarDistance:
                {
                    flowFieldGraph.gameObject.SetActive(false);
                    aStarDistanceGraph.gameObject.SetActive(true);
                    aStarUnitsGraph.gameObject.SetActive(false);

                    informationPanel.sizeDelta = new Vector2(0.0f, 300);

                    startButton.gameObject.SetActive(true);
                    inputField.gameObject.SetActive(true);
                    inputField.text = "";
                    inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter max. X-Value ...";

                    destination.transform.parent.gameObject.SetActive(true);
                    //destination.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -200.0f);
                    time.transform.parent.gameObject.SetActive(true);
                    //time.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -150.0f);
                    unitsUI.transform.parent.gameObject.SetActive(false);
                    mapSizeUI.transform.parent.gameObject.SetActive(false);
                    break;
                }
            case Benchmark.AStarUnits:
                {
                    flowFieldGraph.gameObject.SetActive(false);
                    aStarDistanceGraph.gameObject.SetActive(false);
                    aStarUnitsGraph.gameObject.SetActive(true);

                    informationPanel.sizeDelta = new Vector2(0.0f, 300);

                    startButton.gameObject.SetActive(true);
                    inputField.gameObject.SetActive(true);
                    inputField.text = "";
                    inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "Enter max. Unit-Count ...";

                    destination.transform.parent.gameObject.SetActive(false);
                    time.transform.parent.gameObject.SetActive(true);
                    //time.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -150.0f);
                    unitsUI.transform.parent.gameObject.SetActive(true);
                    //unitsUI.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, -200.0f);
                    mapSizeUI.transform.parent.gameObject.SetActive(false);
                    break;
                }
        }
    }

    public void StartBenchmark()
    {

        if (inputField.text != "")
        {
            switch (benchmarkType)
            {
                case Benchmark.None: break;
                case Benchmark.FlowFields: maxFlowFieldMapSize = int.Parse(inputField.text); break;
                case Benchmark.AStarDistance: distanceAStarFinished = false; maxXValue = int.Parse(inputField.text); break;
                case Benchmark.AStarUnits: maxUnits = int.Parse(inputField.text); break;
                default: Debug.LogError("Unknown Benchmark Type."); break;
            }
        }
        else
        {
            inputField.placeholder.GetComponent<TextMeshProUGUI>().text = "<color=red>Enter a value!</color>";
            return;
        }

        dropdown.interactable = false;
        startButton.interactable = false;
        inputField.interactable = false;
        if (overwriteFile)
        {
            switch (benchmarkType)
            {
                case Benchmark.AStarDistance:
                    {
                        aStarDistanceGrid = new Grid(400, 400, 1.0f);
                        InitializeNeighborsMultithreaded(aStarDistanceGrid.GetGridArray());
                        CSVManager.CreateData("AStarBenchmarkDistance.csv", ma_DistanceHeaders); 
                        break;
                    }
                case Benchmark.AStarUnits:
                    {
                        aStarUnitsGrid = new Grid(400, 400, 1.0f);
                        InitializeNeighborsMultithreaded(aStarUnitsGrid.GetGridArray());
                        CSVManager.CreateData("AStarBenchmarkUnits.csv", ma_UnitsHeaders); 
                        break;
                    }
                case Benchmark.FlowFields:
                    {
                        CSVManager.CreateData("FlowFieldsBenchmark.csv", ma_FlowFieldsHeaders); 
                        break;
                    }
                case Benchmark.None: break;
                default: Debug.LogError("Unknown Benchmark Type."); break;
            }
        }

        benchmarkRunning = true;
    }

    private void FinishedBenchmark()
    {
        dropdown.interactable = true;
        startButton.interactable = true;
        inputField.interactable = true;

        benchmarkRunning = false;
    }

    #region FlowFields

    private void FlowFields()
    {
        if (mapSize <= maxFlowFieldMapSize)
        {
            flowFieldGrid = new Grid(mapSize, mapSize, 1.0f);
            InitializeNeighborsMultithreaded(flowFieldGrid.GetGridArray());

            Debug.Log(mapSize);
            float averageExecutionTime = 0.0f;

            for (int i = 0; i < byte.MaxValue; i++)
            {
                float startTime = Time.realtimeSinceStartup;
                flowfield.FlowFieldPathfinding(flowFieldGrid, (byte)i, flowFieldGrid.getCell(0, 0));
                float endTime = Time.realtimeSinceStartup;

                averageExecutionTime += (endTime - startTime);
            }

            averageExecutionTime /= byte.MaxValue;

            string[] data = new string[2]
            {
                flowFieldGrid.GetWidth().ToString() + "x" + flowFieldGrid.GetHeight().ToString(),
                averageExecutionTime.ToString()
            };
            CSVManager.AppendToData("FlowFieldsBenchmark.csv", ma_FlowFieldsHeaders, data);

            flowFieldValues.Add(averageExecutionTime);


            destination.text = "0, 0";
            time.text = averageExecutionTime.ToString("F4") + "s";
            mapSizeUI.text = mapSize + ", " + mapSize;

            mapSize += 100;
        }
        else
        {
            FinishedBenchmark();

            flowFieldGraph.gameObject.SetActive(true);
            aStarDistanceGraph.gameObject.SetActive(false);
            aStarUnitsGraph.gameObject.SetActive(false);

            flowFieldGraph.ShowGraph(flowFieldValues, -1, (int _i) => "" + ((_i * 100) + 100), (float _f) => _f.ToString("F4") + "s");

            mapSize = 100;
            flowFieldValues.Clear();
        }
    }

    public void InitializeNeighborsMultithreaded(Cell[,] _gridArray)
    {
        for (int x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < _gridArray.GetLength(1); z++)
            {
                StartCoroutine(_gridArray[x, z].SetNeighborsMultithreaded());
            }
        }
    }

    #endregion

    #region AStarUnits

    private void AStarUnits()
    {
        if (units <= maxUnits)
        {
            float averageExecutionTime = 0.0f;
            for (int i = 0; i < 10; i++)
            {
                float startTime = Time.realtimeSinceStartup;
                for (int j = 0; j < units; j++)
                {
                    List<Cell> path = astar.FindPath(aStarUnitsGrid, aStarUnitsGrid.getCell(0, 0), aStarUnitsGrid.getCell(200, 200));
                }
                float endTime = Time.realtimeSinceStartup;

                averageExecutionTime += (endTime - startTime);
            }

            averageExecutionTime /= 10.0f;

            string[] data = new string[2]
            {
                        units.ToString(),
                        averageExecutionTime.ToString()
            };
            CSVManager.AppendToData("AStarBenchmarkUnits.csv", ma_UnitsHeaders, data);
            destination.text = 200 + ", " + 200;
            time.text = averageExecutionTime.ToString("F4") + "s";
            unitsUI.text = units.ToString();

            astarUnitsValues.Add(averageExecutionTime);

            units++;
        }
        else
        {
            FinishedBenchmark();

            aStarUnitsGraph.gameObject.SetActive(true);
            flowFieldGraph.gameObject.SetActive(false);
            aStarDistanceGraph.gameObject.SetActive(false);

            aStarUnitsGraph.ShowGraph(astarUnitsValues, -1, (int _i) => "" + (_i + 1), (float _f) => _f.ToString("F4") + "s");

            units = 1;
            astarUnitsValues.Clear();
        }
    }

    #endregion

    #region AStarDistance
    private void AStarDistance()
    {
        if (!distanceAStarFinished)
        {
            for (int i = 0; i < concurrentCoroutines; i++)
            {
                if (x < maxXValue)
                {
                    pathfinding = new Task(AStarPathfindingDistance(x, z));

                    if (z < aStarDistanceGrid.GetHeight() - 1)
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

                x = 0;
                z = 0; 

                FinishedBenchmark();
            }
        }
    }

    private IEnumerator AStarPathfindingDistance(int _x, int _z)
    {
        float startTime = Time.realtimeSinceStartup;
        List<Cell> path = astar.FindPath(aStarDistanceGrid, aStarDistanceGrid.getCellFromPosition(0, 0), aStarDistanceGrid.getCell(_x, _z));
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

            string[] data = new string[2]
            {
                        i.ToString(),
                        averageExecutionTime.ToString()
            };
            CSVManager.AppendToData("AStarBenchmarkDistance.csv", ma_DistanceHeaders, data);

            astarDistanceValues.Add(averageExecutionTime);

            yield return null;
        }

        aStarDistanceGraph.gameObject.SetActive(true);
        flowFieldGraph.gameObject.SetActive(false);
        aStarUnitsGraph.gameObject.SetActive(false);

        aStarDistanceGraph.ShowGraph(astarDistanceValues, -1, (int _i) => "" + (_i + 1), (float _f) => _f.ToString("F4") + "s");

        x = 0;
        z = 0;
        astarDistanceValues.Clear();

        yield return null;
    }
    #endregion
}
