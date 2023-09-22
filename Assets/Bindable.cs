using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Bindable : MonoBehaviour
{
    public enum BindLocation
    {
        Radio,
        Pipe
    }

    private Rigidbody rb;
    private Throwable throwable;

    public Transform bindLocation = null;
    public bool softbind = false;
    public bool toughbind = false;
    [SerializeField] private BindLocation type;
    [SerializeField] private float bindForce;
    [SerializeField] private float closeDist = 0.5f;
    [SerializeField] private float rebindDelay = 3f;
    private float unboundDuration = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        throwable = GetComponent<Throwable>();
    }

    void OnAttachedToHand()
    {
        softbind = false;
        toughbind = false;
        rb.isKinematic = true;
        rb.useGravity = false;


        if (!bindLocation)
        {
            bindLocation = FindObjectOfType<PlayerInventory>().RequestBind(type);
        }
    }

    void OnDetachedFromHand()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
        softbind = true;
        unboundDuration = rebindDelay;

        rb.velocity        = throwable.releaseVelocity;
        rb.angularVelocity = throwable.releaseAngularVelocity;
    }

    private void FixedUpdate()
    {
        if (!bindLocation)
            return;


        // If we are very close to bind location, lock to it
        if (Vector3.Distance(transform.position, bindLocation.position) < 0.2f && softbind
            || Vector3.Distance(transform.position, bindLocation.position) > 15f)
        {
            transform.parent = bindLocation;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb.isKinematic = true;
            return;
        }


        // Move towards bind location if we are close
        if (Vector3.Distance(transform.position, bindLocation.position) < closeDist && softbind)
        {
            rb.AddForce((bindLocation.position - transform.position).normalized * bindForce);
        }

        if (unboundDuration > 0.0f)
            unboundDuration -= Time.fixedDeltaTime;
        else
            toughbind = softbind;

        if (toughbind)
        {
            rb.AddForce((bindLocation.position - transform.position).normalized * bindForce);
            rb.useGravity = false;
            if (transform.position.y < bindLocation.position.y)
                rb.AddForce((bindLocation.position.y - transform.position.y) * Vector3.up * bindForce * 0.25f);
        }
    }
}
