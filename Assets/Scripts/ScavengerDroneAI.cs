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
    [SerializeField] private State state = State.Scavenging;
    private Rigidbody rb;

    private List<Scavengeable> scavengeTargets = new();
    private Scavengeable currentScrap = null;

    public EnemyAggroable currentAggroTarget = null;

    [SerializeField] private float scrappingRate = 0.2f;

    [SerializeField] private Vector2 scavengeRange = new(0.4f, 1.0f);
    [SerializeField] private float passiveThrust = 0.2f;
    [SerializeField] private Vector2 orbitDelay = new(3.0f, 7.0f);
    private float currentOrbitDelay = 7.0f;
    [SerializeField] private Vector2 orbitForce = new(4.0f, 8.0f);

    [SerializeField] private float closeAggroRadius = 3f;
    [SerializeField] private float eyesightRadius = 10f;
    [SerializeField] private LayerMask aggroable;
    [SerializeField] [Range(0f, 1f)] private float fieldOfView = 0.8f;

    [SerializeField] private float rotateDegPerSecond = 100f;

    private void Awake()
    {
        scavengeTargets = FindObjectsOfType<Scavengeable>().ToList();

        InvokeRepeating(nameof(UpdateAggro), 0.5f, 0.5f);

        rb = GetComponent<Rigidbody>();
    }

    private void SelectScavengable()
    {
        currentScrap = scavengeTargets.OrderByDescending(x => x.value - (Vector3.Distance(transform.position, x.transform.position) * 0.3f)).First();
    }

    private void UpdateAggro()
    {
        // Aggroables close nearby, even behind
        var closeAggroables = Physics.OverlapSphere(transform.position, closeAggroRadius, aggroable.value);

        // Filter eyesight-visible aggroables by a vision cone
        var eyeAggroables = Physics.OverlapSphere(transform.position, eyesightRadius, aggroable.value)
                                .Where(x => 
                                    Vector3.Dot(
                                        transform.forward.normalized, 
                                        (x.transform.position - transform.position).normalized) > (1f - fieldOfView)
                                )
                                .ToArray();

        // Take the closest aggroable with direct line of sight
        var allAggroables = closeAggroables.Concat(eyeAggroables)
                                .Select(x => x.GetComponentInParent<EnemyAggroable>())
                                .OrderBy(x =>
                                    Vector3.Distance(transform.position, x.transform.position) * x.AggroMultiplier);

        foreach (var target in allAggroables)
        {
            if (target.VisibleRatio(transform.position) > 0.4f)
            {
                currentAggroTarget = target;
                return;
            }
        }
        currentAggroTarget = null;
    }

    private bool MoveTowardsPosition(Vector3 pos, Vector2 range)
    {
        float dist = Vector3.Distance(pos, transform.position);
        bool moving = dist > range.y || dist < range.x;
        if (moving)
        {
            rb.AddForce(
                (dist > range.y ? 1 : -2) * passiveThrust * (pos - transform.position)
            );
        }
        return moving;
    }

    private void OnScavenging()
    {
        if (currentAggroTarget)
        {
            state = State.Aggro;
            return;
        }   
        
        if (currentScrap == null || currentScrap.value < 0.1f)
            SelectScavengable();

        Quaternion targetDir = Quaternion.LookRotation(currentScrap.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, rotateDegPerSecond * Time.fixedDeltaTime);

        // Move towards target scrap if we are too far, or away if we are too close.
        if (!MoveTowardsPosition(currentScrap.transform.position, scavengeRange))
        {
            // Perform some orbiting around the scrap pile every now and then
            if (currentOrbitDelay > 0f)
                currentOrbitDelay -= Time.fixedDeltaTime;
            else
            {
                rb.AddForce(
                    Random.Range(orbitForce.x, orbitForce.y)
                    * ((Random.value * 2) - 1)
                    * transform.right,
                    ForceMode.Impulse
                );
                currentOrbitDelay = Random.Range(orbitDelay.x, orbitDelay.y);
            }

            currentScrap.value -= scrappingRate * Time.fixedDeltaTime;
        }
    }

    private void OnAlert()
    {
        if (currentAggroTarget)
        {
            state = State.Aggro;
            return;
        }
    }

    private void OnAggro()
    {
        if (!currentAggroTarget)
        {
            state = State.Alert;
            return;
        }

        Quaternion targetDir = Quaternion.LookRotation(currentAggroTarget.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, rotateDegPerSecond * Time.fixedDeltaTime);

        MoveTowardsPosition(currentAggroTarget.transform.position, scavengeRange);
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
