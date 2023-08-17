using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerLocomotion : MonoBehaviour
{
    private bool canWalk;


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
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        input = joystick.axis;

        input = new Vector2(joystick.axis.x * sensitivity * strafeMultiplier, joystick.axis.y * sensitivity);
        if (input.y < 0)
            input.y *= strafeMultiplier;
    }

    void FixedUpdate()
    {
        if (!canWalk)
            return;

        if (input == Vector2.zero)
            return;

        Vector2 velocity = input.normalized * Mathf.Min(input.magnitude, moveSpeed);

        Transform dir = Player.instance.hmdTransform;
        Vector3 delta = (dir.forward * velocity.y + dir.right * velocity.x) * Time.deltaTime;
        delta.y = 0.0f;
        rb.MovePosition(rb.position + delta);
    }
}
