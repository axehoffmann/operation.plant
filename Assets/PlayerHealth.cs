using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float regeneration;

    public SpawnPoint spawnPoint;

    private float damageOverTime = 0f;
    private bool dead = false;
    private void FixedUpdate()
    {

        currentHealth += (regeneration - damageOverTime) * Time.fixedDeltaTime;
        
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        if (currentHealth < 0 && !dead)
        {
            StartCoroutine(Die());
        }
    }

    public void DamageOverTime(float dps)
    {
        damageOverTime = dps;
    }

    public void Hurt(float damage)
    {
        currentHealth -= damage;
    }

    private IEnumerator Die()
    {
        dead = true;
        SteamVR_Fade.View(Color.black, 4f);
        yield return new WaitForSeconds(4.5f);
        transform.position = spawnPoint.transform.position;
        currentHealth = maxHealth;
        SteamVR_Fade.View(new Color(0, 0, 0, 0), 1f);
        dead = false;
    }
}
