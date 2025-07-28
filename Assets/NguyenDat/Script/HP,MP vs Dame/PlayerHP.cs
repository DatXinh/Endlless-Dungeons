using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100; // Maximum health points
    public int currentHP; // Current health points
    public int InvincibilityTime = 1; // Time in seconds for invincibility after taking damage
    public GameObject damagePopupPrefab; // Prefab for the damage popup
    public Image healthBar; // Reference to the health bar UI element
    public TMP_Text maxHeal; // Reference to the health text UI element
    public TMP_Text currentHeal; // Reference to the current health text UI element

    private bool isInvincible = false; // Trạng thái bất tử

    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;
    public AudioSource[] allAudioSources;
    private AudioSource[] pausedAudioSources;

    void Start()
    {
        currentHP = maxHP; // Initialize current health to maximum health
        if (healthBar != null)
        {
            healthBar.fillAmount = 1f; // Set health bar to full at the start
        }
        if (maxHeal != null)
        {
            maxHeal.text = maxHP.ToString(); // Set the maximum health text
        }
        UpdateHealthUI(); // Update the health UI at the start
        deadMesseng.SetActive(false);
        allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
    }
    // Method to take damage
    public void TakeDamage(int damage)
    {
        if (currentHP > 0) // Check if the player is alive
        {
            if (!isInvincible)
            {
                currentHP -= damage; // Reduce current health by damage amount
                UpdateHealthUI(); // Update the health UI
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
                        floatingDamage.SetDamageValue(damage, Color.red);
                    }
                }
                StartCoroutine(InvincibilityCoroutine());
            }
        }
        if (currentHP <= 0)
        {
            Time.timeScale = 0;
            deadMesseng.SetActive(true);
            rightPanel.SetActive(false);
            leftPanel.SetActive(false);
            PauseGame();
        }
    }

    // Coroutine để xử lý bất tử
    private IEnumerator InvincibilityCoroutine()
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
            if (currentHP > maxHP)
            {
                currentHP = maxHP;
            }
            UpdateHealthUI();
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
        }
    }
    public void UpdateHealthUI()
    {
        float healthPercent = (float)currentHP / maxHP;

        if (healthBar != null)
        {
            healthBar.fillAmount = healthPercent;

            Color healthColor;

            if (healthPercent > 0.8f)
            {
                healthColor = new Color(0f, 1f, 0f); // Xanh lá
            }
            else if (healthPercent > 0.6f)
            {
                healthColor = new Color(0.5f, 1f, 0f); // Vàng xanh
            }
            else if (healthPercent > 0.4f)
            {
                healthColor = new Color(1f, 1f, 0f); // Vàng
            }
            else if (healthPercent > 0.2f)
            {
                healthColor = new Color(1f, 0.5f, 0f); // Cam
            }
            else
            {
                healthColor = new Color(1f, 0f, 0f); // Đỏ
            }

            healthBar.color = healthColor;
        }

        if (currentHeal != null)
        {
            currentHeal.text = currentHP.ToString();
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;

        // Pause các audio đang chạy
        AudioSource[] allAudio = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        pausedAudioSources = System.Array.FindAll(allAudio, a => a.isPlaying);
        foreach (AudioSource audio in pausedAudioSources)
        {
            audio.Pause();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;

        // Resume các audio đã bị pause
        if (pausedAudioSources != null)
        {
            foreach (AudioSource audio in pausedAudioSources)
            {
                if (audio != null) audio.UnPause();
            }
            pausedAudioSources = null;
        }
    }
    public void resetHP()
    {
        currentHP = maxHP;
        UpdateHealthUI();
    }
}
