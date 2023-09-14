using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour
{
    private Queue<RadioEvent> radioQueue = new Queue<RadioEvent>();

    public void PlayEvent(RadioEvent radioEvent)
    {
        if (radioEvent.highPriority)
        {
            pauseDuration = 0.5f;
            audioSource.Stop();
            currentEvent = radioEvent;
            currentEventPlayed = false;
            return;
        }

        radioQueue.Enqueue(radioEvent);
    }

    private float pauseDuration;
    private bool held;

    private AudioSource audioSource;

    private RadioEvent currentEvent;
    private bool currentEventPlayed = false;

    void OnAttachedToHand() => held = true;
    void OnDetachedFromHand() => held = false;
    private void Start() => audioSource = GetComponent<AudioSource>();

    private void FixedUpdate()
    {
        if (pauseDuration > 0)
        {
            pauseDuration -= Time.fixedDeltaTime;
            return;
        }

        if (!currentEventPlayed)
        {
            // Play high priority event
            PlayCurrentEvent();
            return;
        }

        if (!audioSource.isPlaying)
        {
            // Play next event in queue
            currentEvent = radioQueue.Dequeue();
            PlayCurrentEvent();
            return;
        }
    }

    private void PlayCurrentEvent()
    {
        audioSource.PlayOneShot(currentEvent.audio);
        pauseDuration = currentEvent.audio.length + currentEvent.pauseAfterPlaying;
        currentEventPlayed = true;
    }
}
