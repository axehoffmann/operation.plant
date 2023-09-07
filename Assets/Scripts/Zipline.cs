using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Zipline : MonoBehaviour
{
    [SerializeField]
    LineRenderer line;

    [SerializeField] private SteamVR_Action_Boolean grabbed;
    private bool prevFrameGrabButton;
    private Hand hoveringHand;

    public void OnHandHoverBegin(Hand hand)
    {
        hoveringHand = hand;
    }

    public void OnHandHoverEnd(Hand hand)
    {
        hoveringHand = null;

    }

    void Update()
    {
        if (hoveringHand && !prevFrameGrabButton && grabbed.state)
            hoveringHand.SendMessageUpwards("BeginZipline", line, SendMessageOptions.DontRequireReceiver);

        prevFrameGrabButton = grabbed.state;
    }
}
