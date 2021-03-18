using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP FOR EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum PathfindingMethod { FlowField, AStar, None}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed;
    private float currMaxSpeed;

    [SerializeField]
    private PathfindingMethod m_PathfingingMethod; 

    Rigidbody rb;

    // ANIMATOR
    private Animator animator;

    // DYNAMIC
    private bool m_DynamicPathfindingSwitch = false;
    private Cell m_Destination;
    private int m_GroupIndex;
    private int m_GroupUnits;

    // FLOWFIELD
    private byte flowmapIndex;

    public float targetRadius;
    public float arriveRadius;

    // ASTAR
    private float m_AStarTargetRadius = 0.5f;
    private float m_ChangeToAStarRadius = 5.0f;
    List<Cell> m_Path = new List<Cell>();


    private void Start()
    {
        NPCManager.Instance.npcs.Add(this);

        currMaxSpeed = maxSpeed;

        flowmapIndex = byte.MaxValue;

        rb = GetComponent<Rigidbody>();
        this.transform.rotation = Quaternion.Euler(0.0f, 1.0f, 0.0f);

        animator = GetComponent<Animator>(); 
        
        this.transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1, 1), 0.0f, Random.Range(-1, 1)), Vector3.up);
    }

    private void OnDestroy()
    {
        _ = NPCManager.Instance.npcs.Remove(this);
    }

    private void FixedUpdate()
    {
        animator.speed = 1.0f;
        if (m_PathfingingMethod == PathfindingMethod.FlowField)
        {
            if (flowmapIndex == byte.MaxValue)
            {
                return;
            }
            else
            {
                Cell currCell = GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z);
                //currMaxSpeed = maxSpeed * ((float)(byte.MaxValue - currCell.GetCost()) / (float)byte.MaxValue);
                currMaxSpeed = maxSpeed / currCell.GetCost();

                if (FlowFieldManager.Instance.getDestinationCell() != null)
                {
                    float dstToDestination = (new Vector3(FlowFieldManager.Instance.getDestinationCell().xPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().zIndex) - transform.position).magnitude;

                    if(m_DynamicPathfindingSwitch && dstToDestination < m_ChangeToAStarRadius)
                    {
                        AStarPathfinding pathfinding = new AStarPathfinding();
                        //Vector3 targetpos = new Vector3(FlowFieldManager.Instance.getDestinationCell().xPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().zIndex);

                        List<Cell> path = pathfinding.FindPath(GridCreator.grid, GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z), m_Destination);

                        SetPathfindingAStar(path);
                        return;
                    }

                    if (dstToDestination <= targetRadius)
                    {
                        rb.velocity = Vector3.zero;
                    }
                    else if (dstToDestination > targetRadius && dstToDestination < arriveRadius)
                    {
                        Vector3 velocity = currCell.GetFlowFieldDirection(flowmapIndex);
                        rb.AddForce(velocity);

                        currMaxSpeed *= ((dstToDestination - targetRadius) / (arriveRadius - targetRadius));
                        animator.speed = ((dstToDestination - targetRadius) / (arriveRadius - targetRadius));

                        if (rb.velocity.magnitude > currMaxSpeed)
                        {
                            rb.velocity = rb.velocity.normalized * currMaxSpeed;
                        }
                    }
                    else
                    {
                        Vector3 velocity = currCell.GetFlowFieldDirection(flowmapIndex);

                        rb.AddForce(velocity);

                        if (rb.velocity.magnitude > currMaxSpeed)
                        {
                            rb.velocity = rb.velocity.normalized * currMaxSpeed;
                        }
                    }


                    if (rb.velocity != Vector3.zero)
                    {
                        this.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                    }
                }
            }
        }
        else if(m_PathfingingMethod == PathfindingMethod.AStar)
        {
            Cell currCell = GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z);
            currMaxSpeed = maxSpeed * ((float)(byte.MaxValue - (currCell.GetCost() * 10)) / (float)byte.MaxValue);
            //currMaxSpeed = maxSpeed / currCell.GetCost();

            Vector3 velocity = (new Vector3(m_Path[0].xPos, 0.0f, m_Path[0].zPos) - transform.position);
            float dstToDestination = velocity.magnitude;

            rb.AddForce(velocity.normalized);

            if (rb.velocity.magnitude > currMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * currMaxSpeed;
            }

            if (rb.velocity != Vector3.zero)
            {
                this.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
            }

            if (dstToDestination < m_AStarTargetRadius)
            {
                m_Path.RemoveAt(0);
                if (m_Path.Count == 0)
                {
                    rb.velocity = Vector3.zero;
                    m_PathfingingMethod = PathfindingMethod.None;
                    return;
                }
            }
        }
        else
        {
            // Wait for new path
        }

        if (rb.velocity.magnitude > 0.2f && animator != null)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        //Handles.Label(transform.position + new Vector3(0.0f, 1.1f, 0.0f), flowmapIndex.ToString());
        Handles.Label(transform.position + new Vector3(0.0f, 1.1f, 0.0f), m_GroupIndex + "/" + m_GroupUnits);
    }
#endif


    public void SetPathfindingFlowField(byte _index)
    {
        m_Path.Clear();
        m_PathfingingMethod = PathfindingMethod.FlowField;
        m_DynamicPathfindingSwitch = false;
        flowmapIndex = _index;
    }

    public void SetPathfindingAStar(List<Cell> _path)
    {
        flowmapIndex = byte.MaxValue;
        m_PathfingingMethod = PathfindingMethod.AStar;
        m_DynamicPathfindingSwitch = false;
        m_Path = _path;
    }

    public void SetPathfindingDynamic(byte _index, Cell _target, int _groupIndex, int _groupUnits)
    {
        flowmapIndex = byte.MaxValue;
        m_PathfingingMethod = PathfindingMethod.FlowField;
        m_DynamicPathfindingSwitch = true;
        flowmapIndex = _index;

        m_GroupIndex = _groupIndex;
        m_GroupUnits = _groupUnits;

        m_Destination = GetDestinationFormationCell(_target, _groupIndex, _groupUnits);
    }

    private Cell GetDestinationFormationCell(Cell _destination, int _groupIndex, int _groupUnits)
    {
        return GridCreator.grid.getCell(_destination.xIndex + _groupIndex, _destination.zIndex);
    }

    public byte GetFlowMapIndex()
    {
        return flowmapIndex;
    }
}
