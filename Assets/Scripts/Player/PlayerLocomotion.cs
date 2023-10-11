using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerLocomotion : GameComponent
{
    private bool canWalk = true;


    // Ziplining state
    private bool ziplining = false;
    private LineRenderer line = null;
    private int ziplineIndex = 0;
    private float ziplineDelta = 0;
    private float ziplineDeltaSpeed = -1;

    [SerializeField]
    private float ziplineSpeed = 2f;

    [SerializeField]
    private SteamVR_Action_Vector2 joystick;

    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float strafeMultiplier;
    [SerializeField]
    private float sensitivity;

    private Rigidbody rb;

    private Vector2 input;

    [SerializeField] private EnemyAggroable aggro;
    

    public void BeginZipline(LineRenderer zipline)
    {
        canWalk = false;
        ziplining = true;
        line = zipline;
        ziplineIndex = 0;
        ziplineDelta = 0;
        ziplineDeltaSpeed = -1;
    }

    public void EndZipline(LineRenderer zipline)
    {
        canWalk = true;
        ziplining = false;
        line = null;
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input = new Vector2(joystick.axis.x * sensitivity * strafeMultiplier, joystick.axis.y * sensitivity);
        if (input.y < 0)
            input.y *= strafeMultiplier;
    }

    void FixedUpdate()
    {
        aggro.UpdateMultiplier(ID, 1.0f + ((input.magnitude / moveSpeed) * 0.5f));

        if (ziplining)
            UpdateZipline();

        if (!canWalk)
            return;

        if (input == Vector2.zero)
            return;

        Vector2 velocity = input.normalized * Mathf.Min(input.magnitude, moveSpeed);

        Transform dir = Player.instance.hmdTransform;
        Vector3 delta = (dir.forward * velocity.y + dir.right * velocity.x) * Time.fixedDeltaTime;
        delta.y = 0.0f;
        rb.MovePosition(rb.position + delta);
    }

    private void UpdateZipline()
    {
        if (line == null
            || line.positionCount <= ziplineIndex + 1)
        {
            EndZipline(null);
            return;
        }

        Vector3 previousPoint = line.GetPosition(ziplineIndex);
        Vector3 nextPoint = line.GetPosition(ziplineIndex + 1);

        if (ziplineDeltaSpeed < 0)
            RecalculateZiplineDelta(previousPoint, nextPoint);

        ziplineDelta += ziplineDeltaSpeed * Time.fixedDeltaTime;

        if (ziplineDelta > 1)
        {
            ziplineDelta = 0;
            ziplineIndex++;
            ziplineDeltaSpeed = -1;
            return;
        }

        rb.MovePosition(Vector3.Lerp(previousPoint, nextPoint, ziplineDelta));
    }

    private void RecalculateZiplineDelta(Vector3 a, Vector3 b)
    {
        float dist = Vector3.Distance(a, a);
        ziplineDeltaSpeed = dist / ziplineSpeed;
    }
}
