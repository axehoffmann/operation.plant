using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GameComponent : MonoBehaviour
{
    protected GUID ID;
    private void Start()
    {
        ID = GUID.Generate();
    }
}
