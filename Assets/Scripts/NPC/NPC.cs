using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPC : MonoBehaviour
{
    public float maxSpeed;

    Rigidbody rb;
    Vector3 velocity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        this.transform.rotation = Quaternion.Euler(0.0f, 1.0f, 0.0f);
    }

    private void FixedUpdate()
    {
        velocity = GridCreator.grid.getCellFromPosition(transform.position.x, transform.position.z).flowFieldDirection;

        rb.AddForce(velocity);
        if (rb.velocity != Vector3.zero)
        {
            this.transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
        }

        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }
}
