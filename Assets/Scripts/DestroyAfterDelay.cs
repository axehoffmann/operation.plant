using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float delay = 5f;
    void Start()
    {
        Invoke(nameof(Destroy), delay);
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
