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
    private float m_MaxSpeed;
    private float m_CurrMaxSpeed;

    [SerializeField]
    private PathfindingMethod m_PathfingingMethod; 

    Rigidbody m_Rigidbody;

    // ANIMATOR
    private Animator m_Animator;

    // FLOWFIELD
    private byte m_FlowmapIndex;

    public float m_TargetRadius;
    public float m_ArriveRadius;

    // ASTAR
    private float m_AStarTargetRadius = 0.5f;
    private float m_ChangeToAStarRadius = 5.0f;
    List<Cell> m_Path = new List<Cell>();

    // DYNAMIC
    private bool m_DynamicPathfindingSwitch = false;
    private Cell m_Destination;
    private int m_GroupIndex;
    private int m_GroupUnits;


    private void Start()
    {
        NPCManager.Instance.m_NPCs.Add(this);

        m_CurrMaxSpeed = m_MaxSpeed;

        // Set to the default Flowmap Layer with no movement.
        m_FlowmapIndex = byte.MaxValue;

        m_Rigidbody = GetComponent<Rigidbody>();
        //this.transform.rotation = Quaternion.Euler(0.0f, 1.0f, 0.0f);

        m_Animator = GetComponent<Animator>(); 
        
        this.transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1, 1), 0.0f, Random.Range(-1, 1)), Vector3.up);
    }

    private void OnDestroy()
    {
        _ = NPCManager.Instance.m_NPCs.Remove(this);
    }

    private void FixedUpdate()
    {
        m_Animator.speed = 1.0f;

        Cell currCell = GridCreator.s_Grid.getCellFromPosition(transform.position.x, transform.position.z);

        /* Using the current setup the correct currMaxSpeed value would be (maxSpeed / currCell.GetCost())
         * this is way to slow. To compensate it is possible to adjust the speed here.
        */
        m_CurrMaxSpeed = m_MaxSpeed * ((float)(byte.MaxValue - (currCell.GetCost() * 10)) / (float)byte.MaxValue);

        if (m_PathfingingMethod == PathfindingMethod.FlowField)
        {
            if (m_FlowmapIndex == byte.MaxValue)
            {
                return;
            }
            else
            {
                if (FlowFieldManager.Instance.getDestinationCell() != null)
                {
                    float dstToDestination = (new Vector3(FlowFieldManager.Instance.getDestinationCell().m_XPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().m_ZPos) - transform.position).magnitude;

                    if(m_DynamicPathfindingSwitch && dstToDestination < m_ChangeToAStarRadius)
                    {
                        AStarPathfinding pathfinding = new AStarPathfinding();
                        //Vector3 targetpos = new Vector3(FlowFieldManager.Instance.getDestinationCell().xPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().zIndex);

                        List<Cell> path = pathfinding.FindPath(GridCreator.s_Grid, GridCreator.s_Grid.getCellFromPosition(transform.position.x, transform.position.z), m_Destination);

                        SetPathfindingAStar(path);
                        return;
                    }

                    if (dstToDestination <= m_TargetRadius)
                    {
                        m_Rigidbody.velocity = Vector3.zero;
                    }
                    else if (dstToDestination > m_TargetRadius && dstToDestination < m_ArriveRadius)
                    {
                        Vector3 velocity = currCell.GetFlowFieldDirection(m_FlowmapIndex);
                        m_Rigidbody.AddForce(velocity);

                        m_CurrMaxSpeed *= ((dstToDestination - m_TargetRadius) / (m_ArriveRadius - m_TargetRadius));
                        m_Animator.speed = ((dstToDestination - m_TargetRadius) / (m_ArriveRadius - m_TargetRadius));

                        if (m_Rigidbody.velocity.magnitude > m_CurrMaxSpeed)
                        {
                            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_CurrMaxSpeed;
                        }
                    }
                    else
                    {
                        Vector3 velocity = currCell.GetFlowFieldDirection(m_FlowmapIndex);

                        m_Rigidbody.AddForce(velocity);

                        if (m_Rigidbody.velocity.magnitude > m_CurrMaxSpeed)
                        {
                            m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_CurrMaxSpeed;
                        }
                    }


                    if (m_Rigidbody.velocity != Vector3.zero)
                    {
                        this.transform.rotation = Quaternion.LookRotation(m_Rigidbody.velocity, Vector3.up);
                    }
                }
            }
        }
        else if(m_PathfingingMethod == PathfindingMethod.AStar)
        {
            Vector3 velocity = (new Vector3(m_Path[0].m_XPos, 0.0f, m_Path[0].m_ZPos) - transform.position);
            float dstToDestination = velocity.magnitude;

            m_Rigidbody.AddForce(velocity.normalized);

            if (m_Rigidbody.velocity.magnitude > m_CurrMaxSpeed)
            {
                m_Rigidbody.velocity = m_Rigidbody.velocity.normalized * m_CurrMaxSpeed;
            }

            if (m_Rigidbody.velocity != Vector3.zero)
            {
                this.transform.rotation = Quaternion.LookRotation(m_Rigidbody.velocity, Vector3.up);
            }

            if (dstToDestination < m_AStarTargetRadius)
            {
                m_Path.RemoveAt(0);
                if (m_Path.Count == 0)
                {
                    m_Rigidbody.velocity = Vector3.zero;
                    m_PathfingingMethod = PathfindingMethod.None;
                    return;
                }
            }
        }
        else
        {
            // No Path to follow currently
        }

        if (m_Rigidbody.velocity.magnitude > 0.2f && m_Animator != null)
        {
            m_Animator.SetBool("isWalking", true);
        }
        else
        {
            m_Animator.SetBool("isWalking", false);
        }
    }
    
#if UNITY_EDITOR
    // Debugging Tool to show certain NPC information in the editor.
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
        m_FlowmapIndex = _index;
    }

    public void SetPathfindingAStar(List<Cell> _path)
    {
        m_FlowmapIndex = byte.MaxValue;
        m_PathfingingMethod = PathfindingMethod.AStar;
        m_DynamicPathfindingSwitch = false;
        m_Path = _path;
    }

    public void SetPathfindingDynamic(byte _index, Cell _target, int _groupIndex, int _groupUnits)
    {
        m_FlowmapIndex = byte.MaxValue;
        m_PathfingingMethod = PathfindingMethod.FlowField;
        m_DynamicPathfindingSwitch = true;
        m_FlowmapIndex = _index;

        m_GroupIndex = _groupIndex;
        m_GroupUnits = _groupUnits;

        m_Destination = GetDestinationFormationCell(_target, _groupIndex, _groupUnits);
    }

    private Cell GetDestinationFormationCell(Cell _destination, int _groupIndex, int _groupUnits)
    {
        return GridCreator.s_Grid.getCell(_destination.m_XIndex + _groupIndex, _destination.m_ZIndex);
    }

    public byte GetFlowMapIndex()
    {
        return m_FlowmapIndex;
    }
}
