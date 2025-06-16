using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    public float baseDamage = 10f;
    public float bonusDamage = 5f;
    public float critRate = 0.2f;

    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

    public void ResetDamage()
    {
        damagedEnemies.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                bool isCrit = Random.value < critRate;
                DamageInfo damage = new DamageInfo(baseDamage, bonusDamage, isCrit);
                enemy.TakeDamage(damage);
                damagedEnemies.Add(other.gameObject);
            }
        }
    }
}
