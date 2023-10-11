using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform measurePoint;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private float maxForce = 5f;
    // Speed required for the movement to count as a swing attack
    [SerializeField] private float swingThreshold;
    // How long the player's swing attack will last
    [SerializeField] private float swingLeniency;
    // Effect to display while swing attack active
    [SerializeField] private ParticleSystem swingEffect;


    private Vector3 previousPos;

    private float swingTime = 0f;

    private void FixedUpdate()
    {
        if (swingTime > 0f)
        {
            swingTime -= Time.fixedDeltaTime;
            swingEffect.enableEmission = true;
        }
        else
            swingEffect.enableEmission = false;

        Vector3 relativeVelocity = Quaternion.Inverse(transform.rotation) * GetDetectorVelocity();

        // Activate the player's swing attack
        if (relativeVelocity.magnitude > swingThreshold)
        {
            swingTime = swingLeniency;
        }

        previousPos = measurePoint.position;
    }

    private void OnCollisionEnter(Collision collision)
    { 
        if (((1 << collision.gameObject.layer) & enemyLayer.value) != 0)
        {
            if (swingTime <= 0f)
                return;

            Breakable b = collision.gameObject.GetComponentInParent<Breakable>();
            if (b == null)
                return;
            b.Hit();
            Instantiate(hitEffect, collision.GetContact(0).point, Quaternion.identity);

            Vector3 hitVelocity = GetDetectorVelocity();
            // Cap our impact velocity so we don't dropkick the enemy to the moon
            float hitForce = hitVelocity.magnitude;
            if (hitForce > maxForce)
            {
                hitVelocity = hitVelocity.normalized * maxForce;
            }
            collision.gameObject.GetComponentInParent<Rigidbody>().AddForce(hitVelocity * 2f, ForceMode.Impulse);
        }
    }

    private Vector3 GetDetectorVelocity()
    {
        Vector3 detectorPosition = measurePoint.position;
        Vector3 globalVelocity = (detectorPosition - previousPos) / Time.fixedDeltaTime;
        return globalVelocity;
    }
}
