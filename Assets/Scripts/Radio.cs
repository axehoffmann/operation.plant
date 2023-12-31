using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Radio : MonoBehaviour
{
    private Queue<RadioEvent> radioQueue = new();

    private VideoPlayer video;

    [SerializeField] private VideoClip active;
    [SerializeField] private VideoClip idle;
    [SerializeField] private VideoClip noSignal;

    public void PlayEvent(RadioEvent radioEvent)
    {
        /*
        if (radioEvent.highPriority)
        {
            pauseDuration = 0.5f;
            audioSource.Stop();
            currentEvent = radioEvent;
            currentEventPlayed = false;
            return;
        }
        */
        radioQueue.Enqueue(radioEvent);
    }

    private float pauseDuration;
    private bool held;

    private AudioSource audioSource;

    private RadioEvent currentEvent;
    private bool currentEventPlayed = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        video = GetComponentInChildren<VideoPlayer>();
    }

    private void FixedUpdate()
    {
        if (pauseDuration > 0)
        {
            pauseDuration -= Time.fixedDeltaTime;
            return;
        }

        if (radioQueue.Count == 0)
        {
            video.clip = idle;
            return;
        }

        if (!audioSource.isPlaying)
        {
            // Play next event in queue
            currentEvent = radioQueue.Dequeue();
            PlayCurrentEvent();
            return;
        }

        // Stuff we needed for high priority events
        /*
        if (currentEvent == null)
        {
            return;
        }

        if (!currentEventPlayed)
        {
            // Play high priority event
            PlayCurrentEvent();
            return;
        }
        */
        
    }

    private void PlayCurrentEvent()
    {
        audioSource.PlayOneShot(currentEvent.audio);
        pauseDuration = currentEvent.audio.length + currentEvent.pauseAfterPlaying;
        video.clip = active;
        currentEventPlayed = true;
    }
}
