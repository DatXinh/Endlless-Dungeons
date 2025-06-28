using UnityEngine;
using System.Collections.Generic;

public class PlayerWeapon : MonoBehaviour
{
    public float baseDamage = 10f;
    public float bonusDamage = 5f;
    public float critRate = 0.2f;
    public float damageCooldown = 1.5f; // thời gian giữa 2 lần đánh cùng 1 enemy

    private Dictionary<GameObject, float> lastDamageTime = new Dictionary<GameObject, float>();

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            float currentTime = Time.time;

            // Nếu enemy chưa từng bị đánh → gán thời gian cũ
            if (!lastDamageTime.ContainsKey(other.gameObject))
                lastDamageTime[other.gameObject] = -999f;

            // Kiểm tra đã qua cooldown chưa
            if (currentTime - lastDamageTime[other.gameObject] >= damageCooldown)
            {
                EnemyHealth enemy = other.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    bool isCrit = Random.value < critRate;
                    DamageInfo damage = new DamageInfo(baseDamage, bonusDamage, isCrit);

                    enemy.TakeDamage(damage);
                    lastDamageTime[other.gameObject] = currentTime;
                }
            }
        }
    }
}
