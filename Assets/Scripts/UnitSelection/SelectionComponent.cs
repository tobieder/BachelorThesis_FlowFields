using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionComponent : MonoBehaviour
{
    private Color originalColor;
    void Start()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        originalColor = rend.material.color;
        rend.material.color = Color.red;
    }

    private void OnDestroy()
    {
        GetComponentInChildren<Renderer>().material.color = originalColor;
    }
}
