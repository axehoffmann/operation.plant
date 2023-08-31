using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAggroable : MonoBehaviour
{
    public float AggroMultiplier { get { return stale ? recalculateAggro() : aggroMultiplier; } }
    [SerializeField] private float aggroMultiplier = 1f;
    private bool stale = false;

    private Dictionary<GUID, float> multipliers = new();

    [SerializeField]
    private List<Collider> visibleParts = new();

    [SerializeField]
    private LayerMask aggroableLayer;

    private float recalculateAggro()
    {
        aggroMultiplier = 1f;
        foreach (var kv in multipliers)
        {
            aggroMultiplier *= kv.Value;
        }
        stale = false;
        return aggroMultiplier;
    }

    public void UpdateMultiplier(GUID guid, float multiplier)
    {
        stale = true;
        multipliers[guid] = multiplier;
    }

    // How visible is this aggroable from an eye?
    public float VisibleRatio(Vector3 eye)
    {
        int total = 0;
        foreach (Collider col in visibleParts)
        {
            if (Physics.Raycast(eye, col.bounds.center - eye, out RaycastHit hit))
            {
                total += hit.collider == col ? 1 : 0;
            }
        }
        return (float)total / (float)visibleParts.Count;   
    }
}
