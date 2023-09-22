using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField]
    private float yScale;
    [SerializeField]
    private float viewRadius;
    [SerializeField]
    private float totalScale;
    [SerializeField]
    private GameObject indicator;

    public float GetYScale() => yScale;
    public float GetViewRadius() => viewRadius;
    public float GetTotalScale() => totalScale;


    void Start()
    {
        InvokeRepeating("UpdateIndicators", 0.5f, 1.0f);
    }

    void UpdateIndicators()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        RadarLocatable[] loc = FindObjectsOfType<RadarLocatable>();
        foreach (RadarLocatable r in loc)
        {
            GameObject i = Instantiate(indicator, transform);
            i.GetComponent<RadarIndicator>().Initialise(r, this);
        }
    }
}
