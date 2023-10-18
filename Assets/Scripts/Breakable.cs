using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private float durability = 1;
    [SerializeField] private float damage = 0;
    [SerializeField] private MonoBehaviour[] disableOnBreak;
    [SerializeField] private GameObject instantiateOnBreak;
    [SerializeField] private bool destroyOnBreak;
    [SerializeField] private AudioClip playOnBreak;
    [SerializeField] private ParticleSystem particleToStop;
    [SerializeField] private Animator anim;

    private Rigidbody rb;
    private AudioSource source;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        source = GetComponent<AudioSource>();
    }

    public void Hit()
    {
        if (damage >= durability)
            return;

        damage += 1;
        if (damage >= durability)
        {
            if (destroyOnBreak)
                Destroy(gameObject);

            if (instantiateOnBreak)
                Instantiate(instantiateOnBreak, transform.position, Quaternion.identity);

            if (source)
            {
                source.loop = false;
                source.Stop();
            }

            if (source && playOnBreak)
                source.PlayOneShot(playOnBreak);

            foreach (MonoBehaviour c in disableOnBreak)
            {
                c.enabled = false;
            }

            if (rb)
            {
                rb.useGravity = true;
            }

            if (particleToStop)
                particleToStop.Stop();

            
            anim.SetTrigger("Break");
        }
    }
}
