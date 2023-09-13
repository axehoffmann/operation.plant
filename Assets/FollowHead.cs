using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    public Transform head;
    [SerializeField]
    private Vector3 offset;


    private Quaternion rotOffset;

    private void Start()
    {
        rotOffset = Quaternion.Inverse(head.localRotation * transform.localRotation);
    }

    void Update()
    {
        Quaternion rot = Quaternion.Euler(0f, head.localRotation.eulerAngles.y, 0f);

        transform.localPosition = (rot * (offset)) + head.localPosition;
        transform.localRotation = rot;
    }
}
