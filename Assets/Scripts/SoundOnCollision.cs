using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundOnCollision : MonoBehaviour
{
    [SerializeField] private AudioClip[] lowVelocity;
    [SerializeField] private AudioClip[] highVelocity;

    [SerializeField] private Transform measurePoint;
    private Vector3 previousPos;

    [SerializeField] private float velocityThreshold;
    [SerializeField] private float cooldown = 0.2f;
    private float currentCooldown = 0f;

    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (currentCooldown > 0f)
            currentCooldown -= Time.fixedDeltaTime;

        previousPos = measurePoint.position;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (currentCooldown > 0f)
            return;

        float vel = GetDetectorVelocity();
        currentCooldown = cooldown;
        if (vel > velocityThreshold)
            PlayRandom(highVelocity);
        else
            PlayRandom(lowVelocity);
    }

    private void PlayRandom(AudioClip[] arr)
    {
        AudioClip clip = arr[Random.Range(0, arr.Length)];
        source.PlayOneShot(clip);
    }    

    private float GetDetectorVelocity()
    {
        Vector3 detectorPosition = measurePoint.position;
        Vector3 globalVelocity = (detectorPosition - previousPos) / Time.fixedDeltaTime;
        return globalVelocity.magnitude;
    }
}
