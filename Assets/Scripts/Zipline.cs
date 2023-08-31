using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class Zipline : MonoBehaviour
{
    [SerializeField]
    LineRenderer line;

    public void OnAttachedToHand(Hand hand)
    {
        hand.SendMessageUpwards("BeginZipline", line, SendMessageOptions.DontRequireReceiver);
        Debug.Log("GrabZipline");
    }

    public void OnDetachedFromHand(Hand hand)
    {
        hand.SendMessageUpwards("EndZipline", line, SendMessageOptions.DontRequireReceiver);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
