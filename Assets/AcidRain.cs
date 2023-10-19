using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidRain : MonoBehaviour
{
    [SerializeField] private float damagePerSecond;
    private void OnTriggerEnter(Collider other)
    {
        UpdateAcidRain(other, damagePerSecond);

    }

    private void OnTriggerExit(Collider other)
    {
        UpdateAcidRain(other, 0f);
    }

    private void UpdateAcidRain(Collider other, float dps)
    {
        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        Debug.Log(player);
        if (player != null)
        {
            player.DamageOverTime(dps);
        }
    }
}
