using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SparkBotAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackCooldown;
    private float currentCooldown = 0;

    [SerializeField] private float aggroRadius;
    [SerializeField] private LayerMask aggroable;

    [SerializeField] private EnemyAggroable currentAggroTarget;

    [SerializeField] private float attackRange = 2f;

    private Rigidbody rb;
    [SerializeField] private Animator anim;

    [SerializeField] private ParticleSystem attackEffect;

    [SerializeField] private float rotationSpeed = 90f;

    private IEnumerator Attack()
    {
        anim.SetTrigger("Attack");
        currentCooldown = attackCooldown;
        yield return new WaitForSeconds(1f);
        attackEffect.Play();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(UpdateAggro), 0.5f, 0.5f);
    }

    private void FixedUpdate()
    {
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.fixedDeltaTime;
        }
        if (!currentAggroTarget)
        {
            anim.SetBool("Walking", false);
            return;
        }

        Vector3 delta = currentAggroTarget.transform.position - transform.position;
        float distToTarget = delta.magnitude;

        if (distToTarget < attackRange)
        {
            if (currentCooldown > 0)
            {
                anim.SetBool("Walking", false);
                return;
            }
            StartCoroutine(Attack());
        }
        else if (distToTarget > attackRange)
        {
            anim.SetBool("Walking", true);

            Quaternion targetDir = Quaternion.LookRotation(delta, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, rotationSpeed * Time.fixedDeltaTime);

            float facing = Vector3.Dot(delta, transform.forward);
            if (facing > 0)
                rb.velocity = delta.normalized * moveSpeed;
        }
    }

    private void UpdateAggro()
    {
        var closeAggroables = Physics.OverlapSphere(transform.position, aggroRadius, aggroable.value);

        // Take the closest aggroable
        currentAggroTarget = closeAggroables
                                .Select(x => x.GetComponentInParent<EnemyAggroable>())
                                .Where(x => x != null)
                                .OrderByDescending(x =>
                                    Vector3.Distance(transform.position, x.transform.position) * x.AggroMultiplier)
                                .FirstOrDefault();
    }
}
