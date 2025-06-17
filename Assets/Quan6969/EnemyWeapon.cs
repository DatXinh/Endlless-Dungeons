using UnityEngine;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour
{
    public float baseDamage = 10f;
    public float bonusDamage = 5f;
    public float critRate = 0.2f;
    public float damageCooldown = 2f; 

    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            float currentTime = Time.time;

            if (!lastDamageTime.ContainsKey(other.gameObject))
                lastDamageTime[other.gameObject] = -999f;

            if (currentTime - lastDamageTime[other.gameObject] >= damageCooldown)
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    bool isCrit = Random.value < critRate;
                    DamageInfo damage = new DamageInfo(baseDamage, bonusDamage, isCrit);

                    player.TakeDamage(damage);

                    lastDamageTime[other.gameObject] = currentTime; 
                }
            }
        }
    }
}
