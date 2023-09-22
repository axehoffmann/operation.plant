using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarLocatable : MonoBehaviour
{
    [SerializeField]
    private float visibleDistance = 10f;
    [SerializeField]
    private Color indicatorColour = Color.yellow;

    public float GetMaxDistance() => visibleDistance;
    public Color GetIndicatorColour() => indicatorColour;
}
