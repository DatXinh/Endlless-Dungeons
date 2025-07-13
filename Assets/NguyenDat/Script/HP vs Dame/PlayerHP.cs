using System;
using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100; // Maximum health points
    public int currentHP; // Current health points
    public int InvincibilityTime = 1; // Time in seconds for invincibility after taking damage
    public GameObject damagePopupPrefab; // Prefab for the damage popup

    private bool isInvincible = false; // Trạng thái bất tử

    void Start()
    {
        currentHP = maxHP; // Initialize current health to maximum health
    }
    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (currentHP > 0) // Check if the player is alive
        {
            if (!isInvincible)
            {
                currentHP -= damage; // Reduce current health by damage amount
                if (currentHP < 0) // Ensure current health does not go below zero
                {
                    currentHP = 0;
                }
                // Display damage popup
                if (damagePopupPrefab != null)
                {
                    GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                    FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                    if (floatingDamage != null)
                    {
                        floatingDamage.SetDamageValue(damage, Color.red); // Set the damage value and color
                    }
                }
                Debug.Log("Player took damage: " + damage + ", Current HP: " + currentHP);
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }

    // Coroutine để xử lý bất tử
    private System.Collections.IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }

    // Hàm hồi máu
    public void Heal(int amount)
    {
        if (currentHP > 0)
        {
            currentHP += amount;
            // Display damage popup
            if (damagePopupPrefab != null)
            {
                GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                if (floatingDamage != null)
                {
                    floatingDamage.SetDamageValue(amount, Color.green);
                }
            }
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
        }
    }
}
