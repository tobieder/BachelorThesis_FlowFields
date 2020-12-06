using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP FOR EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour
{
    public float maxSpeed;
    private float currMaxSpeed;

    Rigidbody rb;
    Vector3 velocity;

    private byte flowmapIndex;

    // TEMP
    public float targetRadius;
    public float slowRadius;

    private void Start()
    {
        NPCManager.Instance.npcs.Add(this);

        currMaxSpeed = maxSpeed;

        flowmapIndex = byte.MaxValue;

        rb = GetComponent<Rigidbody>();
        this.transform.rotation = Quaternion.Euler(0.0f, 1.0f, 0.0f);
    }

    private void OnDestroy()
    {
        NPCManager.Instance.npcs.Remove(this);
    }

    private void FixedUpdate()
    {
        if (flowmapIndex == byte.MaxValue)
        {
            return;
        }
        else
        {
            Cell currCell = GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z);
            currMaxSpeed = maxSpeed / currCell.GetCost();

            if (FlowFieldManager.Instance.getDestinationCell() != null)
            {
                float dstToDestination = (new Vector3(FlowFieldManager.Instance.getDestinationCell().xPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().zIndex) - transform.position).magnitude;

                if (dstToDestination < targetRadius)
                {
                    rb.velocity = Vector3.zero;
                }
                else if (dstToDestination > slowRadius)
                {
                    velocity = currCell.GetFlowFieldDirection(flowmapIndex);

                    rb.AddForce(velocity);

                    if (rb.velocity.magnitude > currMaxSpeed)
                    {
                        rb.velocity = rb.velocity.normalized * currMaxSpeed;
                    }
                }
                else
                {
                    float targetSpeed = currMaxSpeed * dstToDestination / slowRadius;
                    rb.velocity = currCell.GetFlowFieldDirection(flowmapIndex) * targetSpeed;
                }


                if (rb.velocity != Vector3.zero)
                {
                    this.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
                }
            }
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        Handles.Label(transform.position + new Vector3(0.0f, 1.1f, 0.0f), flowmapIndex.ToString());
    }
#endif

    public void SetFlowMapIndex(byte _index)
    {
        flowmapIndex = _index;
    }

    public byte GetFlowMapIndex()
    {
        return flowmapIndex;
    }
}
