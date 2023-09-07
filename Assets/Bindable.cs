using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bindable : MonoBehaviour
{
    public enum BindLocation
    {
        Radio,
        Pipe
    }

    private Rigidbody rb;

    public Transform bindLocation = null;
    public bool softbind = false;
    public bool toughbind = false;
    [SerializeField] private BindLocation type;
    [SerializeField] private float bindForce;
    [SerializeField] private float closeDist = 0.5f;

    private float unboundDuration = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnAttachedToHand()
    {
        softbind = false;
        toughbind = false;

        if (!bindLocation)
        {
            bindLocation = FindObjectOfType<PlayerInventory>().RequestBind(type);
        }
    }

    void OnDetachedFromHand()
    {
        softbind = true;
        unboundDuration = 1.0f;
    }

    private void FixedUpdate()
    {
        if (!bindLocation)
            return;

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

            if (transform.position.y < bindLocation.position.y)
                rb.AddForce((transform.position.y - bindLocation.position.y) * Vector3.up * bindForce);
        }

        // If we are very close to bind location, lock to it
        if (Vector3.Distance(transform.position, bindLocation.position) < 0.1f && softbind)
        {
            transform.position = bindLocation.position;
        }
    }
}
