﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TEMP FOR EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class NPC : MonoBehaviour
{
    public float maxSpeed;
    private float currMaxSpeed;

    Rigidbody rb;
    Vector3 velocity;

    private byte flowmapIndex;

    // ANIMATOR
    private Animator animator;

    // TEMP
    public float targetRadius;

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
        if (flowmapIndex == byte.MaxValue)
        {
            return;
        }
        else
        {
            Cell currCell = GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z);
            currMaxSpeed = maxSpeed * ((float)(byte.MaxValue - currCell.GetCost()) / (float)byte.MaxValue);

            if (FlowFieldManager.Instance.getDestinationCell() != null)
            {
                float dstToDestination = (new Vector3(FlowFieldManager.Instance.getDestinationCell().xPos, 0.0f, FlowFieldManager.Instance.getDestinationCell().zIndex) - transform.position).magnitude;

                if (dstToDestination < targetRadius)
                {
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    velocity = currCell.GetFlowFieldDirection(flowmapIndex);

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
