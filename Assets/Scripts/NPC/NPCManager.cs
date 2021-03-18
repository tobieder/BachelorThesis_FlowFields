using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private static NPCManager _instance;
    public static NPCManager Instance { get { return _instance; } }

    public List<NPC> npcs;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        npcs = new List<NPC>();
    }

    public void ApplyPhysics(bool _val)
    {
        Physics.IgnoreLayerCollision(12, 12, !_val);
    }
}
