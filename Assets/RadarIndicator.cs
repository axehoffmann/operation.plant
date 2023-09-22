using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarIndicator : MonoBehaviour
{
    private Radar radar;
    private RadarLocatable pointTo;

    public void Initialise(RadarLocatable target, Radar r) =>
        (pointTo, radar, GetComponent<Renderer>().material.color) = (target, r, target.GetIndicatorColour());

    void LateUpdate()
    {
        float dist = Vector3.Distance(pointTo.transform.position, transform.position);

        // Delete indicator if the tracked object is out of range
        if (dist > pointTo.GetMaxDistance())
        {
            Destroy(gameObject);
        }


        Vector3 dir = pointTo.transform.position - transform.position;
        float ratio = dir.magnitude / radar.GetViewRadius();
        // Position the indicator on the radar
        transform.position = 
            ((ratio < 1.0f ? 
                      dir.normalized * ratio 
                    : dir.normalized
                ) * radar.GetTotalScale()
            ) + radar.transform.position;
    }
}
