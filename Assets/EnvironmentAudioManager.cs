using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;

    [SerializeField] private float switchRate;
    private int switchDirection = 1;
    private int current = 0;

    public void SwapTo(AudioClip clip)
    {
        switchDirection = -switchDirection;
        current = (switchDirection + 1) / 2;

        audioSources[current].clip = clip;
        audioSources[current].volume = 0f;
    }

    private void FixedUpdate()
    {
        int other = (current + 1) % 2;
        audioSources[current].volume += switchRate * Time.fixedDeltaTime;
        audioSources[other].volume -= switchRate * Time.fixedDeltaTime;

        audioSources[current].volume = Mathf.Min(audioSources[current].volume, 1.0f);
        audioSources[other].volume = Mathf.Max(audioSources[other].volume, 0.0f);
    }
}
