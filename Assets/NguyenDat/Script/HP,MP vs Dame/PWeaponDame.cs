using System.Collections;
using UnityEngine;

public class PWeaponDame : MonoBehaviour
{
    private WeaponData weaponData;

    public int weaponDamage;
    public int weaponCriticalChange;

    void Start()
    {
        // Lấy WeaponData từ object cha
        weaponData = GetComponentInParent<WeaponData>();
        if (weaponData != null)
        {
            weaponDamage = weaponData.weaponDamage;
            weaponCriticalChange = weaponData.weaponCriticalChange;
        }
    }

    // Gây sát thương khi va chạm với Enemy (dùng Collider 2D)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            int baseDamage = weaponDamage;
            int critRate = weaponCriticalChange;

            bool isCritical = Random.Range(0, 100) < critRate;
            int finalDamage = baseDamage;

            if (isCritical)
            {
                float critMultiplier = Random.Range(1.5f, 3.0f);
                finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
            }
            enemyHP.TakeDamage(finalDamage, isCritical);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            int baseDamage = weaponDamage;
            int critRate = weaponCriticalChange;

            bool isCritical = Random.Range(0, 100) < critRate;
            int finalDamage = baseDamage;

            if (isCritical)
            {
                float critMultiplier = Random.Range(1.5f, 3.0f);
                finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
            }
            enemyHP.TakeDamage(finalDamage, isCritical);
        }
    }
}
