using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RadioEvent", menuName = "ScriptableObjects/RadioEvent", order = 1)]
public class RadioEvent : ScriptableObject
{
    public AudioClip audio;
    public bool highPriority = false;
    public float pauseAfterPlaying = 2f;
    private bool requiresHeld = false;
}
