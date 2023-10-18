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

    [SerializeField] private float maxSpeed = 5.0f;
    [SerializeField] private float decelerationSpeed = 2.0f;

    [SerializeField] private float closeAggroRadius = 3f;
    [SerializeField] private float eyesightRadius = 10f;
    [SerializeField] private LayerMask aggroable;
    [SerializeField] [Range(0f, 1f)] private float fieldOfView = 0.8f;

    [SerializeField] private LineRenderer lazer;

    [SerializeField] private float rotateDegPerSecond = 100f;

    [SerializeField] private float alertDuration = 3f;

    [SerializeField] private Animator anim;

    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private AudioSource attackSound;
    private float currentAttackCooldown = 0f;

    private float currentAlertDur = 0f;

    private IEnumerator DamageTargetDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        print(Vector3.Distance(transform.position, currentAggroTarget.transform.position));
        if (currentAggroTarget 
            && Vector3.Distance(transform.position, currentAggroTarget.transform.position) < 1.5f)
        {
            currentAggroTarget.GetComponentInParent<PlayerHealth>().Hurt(attackDamage);
        }
    }

    private void Awake()
    {
        scavengeTargets = FindObjectsOfType<Scavengeable>().ToList();

        InvokeRepeating(nameof(UpdateAggro), 0.5f, 0.5f);

        rb = GetComponent<Rigidbody>();
    }

    private void SelectScavengable()
    {
        // Select a scrap randomly, preferring nearby and high-value scrap
        currentScrap = scavengeTargets.OrderByDescending(
            x => x.value - (Vector3.Distance(transform.position, x.transform.position) * 0.3f))
            .ElementAt(Random.Range(0, Mathf.Min(scavengeTargets.Count - 1, 3)));
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
                                .Where(x => x != null)
                                .OrderByDescending(x =>
                                    Vector3.Distance(transform.position, x.transform.position) * x.AggroMultiplier); 

        foreach (var target in allAggroables)
        {
            if (Vector3.Distance(target.transform.position, transform.position) > eyesightRadius)
                continue;
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
            // Decelerate if above our max speed
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.AddForce(rb.velocity.normalized * decelerationSpeed);
            }
            else
            {
                rb.AddForce(
                    (dist > range.y ? 1 : -2) * passiveThrust * (pos - transform.position)
                );
            }
        }
        return moving;
    }

    private bool IsInFront(Vector3 target)
    {
        Vector3 delta = target - transform.position;
        float facing = Vector3.Dot(delta, transform.forward);
        return facing > 0.3;            
    }

    private void OnScavenging()
    {
        if (currentAggroTarget)
        {
            state = State.Aggro;
            currentAttackCooldown = attackCooldown;
            return;
        }   
        
        if (currentScrap == null || currentScrap.value < 0.1f)
            SelectScavengable();

        Quaternion targetDir = Quaternion.LookRotation(currentScrap.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, rotateDegPerSecond * Time.fixedDeltaTime);

        // Move towards target scrap if we are too far, or away if we are too close.
        if (!MoveTowardsPosition(currentScrap.transform.position, scavengeRange))
        {
            // Only laze if we are looking in the general direction of the scrap
            if (!IsInFront(currentScrap.transform.position))
                return;

            if (currentAttackCooldown > 0)
                currentAttackCooldown -= Time.fixedDeltaTime;

            if (currentAttackCooldown <= 0)
            {
                anim.SetTrigger("Attack");
                attackSound.Play();
                currentAttackCooldown = attackCooldown;
            }

            /*
            // Laze our target
            lazer.enabled = true;
            lazer.SetPosition(1, currentScrap.transform.position);
            lazer.SetPosition(0, lazer.transform.position);
            */

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
        currentAlertDur -= Time.fixedDeltaTime;

        if (currentAggroTarget)
        {
            state = State.Aggro;
            currentAttackCooldown = attackCooldown;
            return;
        }

        if (currentAlertDur <= 0f)
            state = State.Scavenging;
    }

    private void OnAggro()
    {
        if (!currentAggroTarget)
        {
            state = State.Alert;
            currentAlertDur = alertDuration;
            return;
        }

        Quaternion targetDir = Quaternion.LookRotation(currentAggroTarget.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, rotateDegPerSecond * Time.fixedDeltaTime);

        if(!MoveTowardsPosition(currentAggroTarget.transform.position + Vector3.up, scavengeRange))
        {
            if (currentAttackCooldown > 0)
                currentAttackCooldown -= Time.fixedDeltaTime;

            if (currentAttackCooldown <= 0)
            {
                anim.SetTrigger("Attack");
                StartCoroutine(DamageTargetDelayed(0.3f));
                attackSound.Play();
                currentAttackCooldown = attackCooldown;
            }
        }
    }

    private void OnAttacking()
    {

    }


    private void FixedUpdate()
    {
        lazer.enabled = false;
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
