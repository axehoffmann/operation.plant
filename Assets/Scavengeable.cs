using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavengeable : MonoBehaviour
{
    [SerializeField]    
    private float baseValue;

    public float value;

    private void FixedUpdate()
    {
        if (value < baseValue)
            value += (baseValue * 0.05f) * Time.fixedDeltaTime;
    }
}
