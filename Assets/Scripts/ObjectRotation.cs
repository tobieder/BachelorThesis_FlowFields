using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotation : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private void Update()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.y += (speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
