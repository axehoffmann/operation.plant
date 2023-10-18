using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLANTAI : MonoBehaviour
{

    private Transform following;
    private SpringJoint spring;
    private Rigidbody rb;

    [SerializeField] private float heightLeniency;
    [SerializeField] private float followDist;
    [SerializeField] private float followForce;
    [SerializeField] private float hoverHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private float maxDistance = 20f;


    void Start()
    {
        following = FindObjectOfType<PlayerLocomotion>().transform;
        spring = GetComponent<SpringJoint>();
        rb = GetComponent<Rigidbody>();
    }

    private Vector2 To2D(Vector3 pos)
    {
        return new(pos.x, pos.z);
    }

    private void FixedUpdate()
    {
        // Move towards player
        Vector2 pPos = To2D(following.position);
        Vector2 pos = To2D(transform.position);
        if (Vector2.Distance(pPos, pos) > followDist)
        {
            Vector2 dir = (pPos - pos).normalized;
            Vector3 dir3 = new(dir.x, 0f, dir.y);

            rb.AddForce(dir3 * followForce);
        }

        // Update hover physics spring
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, 10f, groundMask))
        {
            // Move vertically up to player if player is higher than PLANT
            float vertDiff = (following.position.y) - hit.point.y;
            vertDiff = Mathf.Clamp(vertDiff, 0f, heightLeniency);

            spring.connectedAnchor = hit.point + ((hoverHeight + vertDiff) * Vector3.up);
        }

        if (Vector2.Distance(pPos, pos) > maxDistance)
        {
            transform.position = following.position + following.right;
        }
    }
}