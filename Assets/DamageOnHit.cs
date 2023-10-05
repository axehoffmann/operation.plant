using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOnHit : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float velocityThreshold = 2f;

    private float velocity;

    private void FixedUpdate()
    {
        // TODO calculate velocity at point
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer & enemyLayer.value) != 0)
        {
            collision.gameObject.GetComponent<Breakable>().Hit();
            collision.gameObject.GetComponentInParent<Rigidbody>().AddForceAtPosition(collision.GetContact(0).point)
        }
    }
}
