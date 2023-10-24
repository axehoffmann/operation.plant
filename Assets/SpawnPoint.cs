using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;

    private AudioSource triggerSound;
    [SerializeField] private ParticleSystem triggerParticle;
    [SerializeField] private GameObject activeObs;

    private void Start()
    {
        triggerSound = GetComponent<AudioSource>();
    }

    private void Disable()
    {
        activeObs.SetActive(false);
    }

    private void Enable()
    {
        triggerSound.Play();
        triggerParticle.Play();
        activeObs.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            if (playerHealth.spawnPoint != spawnPos)
            {
                if (playerHealth.spawnPoint)
                    playerHealth.spawnPoint.Disable();
                playerHealth.spawnPoint = this;
                Enable();
            }
        }    
    }
}
