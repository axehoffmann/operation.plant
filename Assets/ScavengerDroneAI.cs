using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScavengerDroneAI : MonoBehaviour
{
    private enum State
    {
        Scavenging,
        Alert,
        Aggro,
        Attacking
    }
    private State state = State.Scavenging;
    private Rigidbody rb;

    private List<Scavengeable> scavengeTargets = new();
    private Scavengeable currentScrap = null;


    [SerializeField] private Vector2 scavengeRange = new(0.4f, 1.0f);
    [SerializeField] private float passiveThrust = 0.2f;
    [SerializeField] private Vector2 orbitDelay = new(3.0f, 7.0f);
    private float currentOrbitDelay = 7.0f;
    [SerializeField] private Vector2 orbitForce = new(4.0f, 8.0f);



    private void Awake()
    {
        scavengeTargets = FindObjectsOfType<Scavengeable>().ToList();
        rb = GetComponent<Rigidbody>();
    }

    private void SelectScavengable()
    {
        scavengeTargets.OrderByDescending(x => x.value);
        currentScrap = scavengeTargets[0];
    }

    private void OnScavenging()
    {
        if (currentScrap == null || currentScrap.value < 0.1f)
            SelectScavengable();

        transform.LookAt(currentScrap.transform.position);
        float distToScrap = Vector3.Distance(transform.position, currentScrap.transform.position);
        // Move towards target scrap if we are too far, or away if we are too close.
        if (distToScrap > scavengeRange.y || distToScrap < scavengeRange.x)
        {
            rb.AddForce(
                passiveThrust
                * (currentScrap.transform.position - transform.position)
                * (distToScrap > scavengeRange.y ? 1 : -2)
            );
        }
        else
        {
            if (currentOrbitDelay > 0f)
                currentOrbitDelay -= Time.fixedDeltaTime;
            else
            {
                rb.AddForce(transform.right 
                    * Random.Range(orbitForce.x, orbitForce.y)
                    * ((Random.value * 2) - 1),
                    ForceMode.Impulse
                );
                currentOrbitDelay = Random.Range(orbitDelay.x, orbitDelay.y);
            }
        }


    }

    private void OnAlert()
    {

    }

    private void OnAggro()
    {

    }

    private void OnAttacking()
    {

    }


    private void FixedUpdate()
    {
        switch(state)
        {
            case State.Scavenging:
                OnScavenging(); break;
            case State.Alert: 
                OnAlert(); break;
            case State.Aggro: 
                OnAggro(); break;
            case State.Attacking: 
                OnAttacking(); break;
        }
    }
}
