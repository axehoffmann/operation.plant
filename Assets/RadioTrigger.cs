using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTrigger : MonoBehaviour
{

    [SerializeField] private RadioEvent radioEvent;
    [SerializeField] private bool played = false;

    private Radio radio;

    void Start()
    {
        radio = FindObjectOfType<Radio>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger");
        if (played)
            return;
        
        radio.PlayEvent(radioEvent);
        played = true;
    }
}
