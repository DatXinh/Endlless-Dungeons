using System.Collections;
using UnityEngine;

public class PWeaponDame : MonoBehaviour
{
    private WeaponData weaponData;

    public int weaponDamage;
    public int weaponCriticalChange;
    private int FinalWeaponDamage;

    void Start()
    {
        // Lấy WeaponData từ object cha
        weaponData = GetComponentInParent<WeaponData>();
        if (weaponData != null)
        {
            weaponDamage = weaponData.weaponDamage;
            weaponCriticalChange = weaponData.weaponCriticalChange;
        }
        // Tính toán sát thương cuối cùng dựa trên vòng lặp hiện tại
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.currentLoop > 0)
            {
                FinalWeaponDamage = (int)(weaponDamage * (LoopManager.Instance.currentLoop * 0.2f));
            }
            else
            {
                FinalWeaponDamage = weaponDamage; // Sử dụng sát thương gốc nếu không có LoopManager
            }
        }
    }

    // Gây sát thương khi va chạm với Enemy (dùng Collider 2D)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHP enemyHP = collision.GetComponent<EnemyHP>();
        if (enemyHP != null)
        {
            int baseDamage = FinalWeaponDamage;
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
            int baseDamage = FinalWeaponDamage;
            int critRate = weaponCriticalChange;

            bool isCritical = Random.Range(0, 100) < critRate;
            int finalDamage = baseDamage;

            if (isCritical)
            {
                float critMultiplier = Random.Range(1.5f, 3.0f);
                finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
            }
            Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
        }
    }
}
