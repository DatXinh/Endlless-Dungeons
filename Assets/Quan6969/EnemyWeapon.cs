using UnityEngine;
using System.Collections.Generic;

public class EnemyWeapon : MonoBehaviour
{
    public float baseDamage = 8f;
    public float bonusDamage = 3f;
    public float critRate = 0.1f;

    private HashSet<GameObject> damagedPlayers = new HashSet<GameObject>();

    public void ResetDamage()
    {
        damagedPlayers.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !damagedPlayers.Contains(other.gameObject))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                bool isCrit = Random.value < critRate;
                DamageInfo damage = new DamageInfo(baseDamage, bonusDamage, isCrit);
                player.TakeDamage(damage);
                damagedPlayers.Add(other.gameObject);
            }
        }
    }
}
