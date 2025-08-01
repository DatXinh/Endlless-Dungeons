using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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

    public Joystick joystick;
    public JoystickAttackAndAim joystickAttackAndAim; // Reference to the joystick for attack and aim

    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;
    public GameObject pauseButton;
    public AudioSource[] allAudioSources;
    public List<AudioSource> playAudioSources = new List<AudioSource>();

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
                // Gây knockback nếu có direction từ attacker
                UpdateHealthUI(); // Update the health UI
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

            joystick.OnPointerUp(null);
            joystickAttackAndAim.OnPointerUp(null);
            leftPanel.SetActive(false);
            rightPanel.SetActive(false);
            deadMesseng.SetActive(true);
            pauseButton.SetActive(false);
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
        }

        if (currentHeal != null)
        {
            currentHeal.text = currentHP.ToString();
        }
    }
    public void PauseGame()
    {
        foreach (AudioSource audio in allAudioSources)
        {
            if (audio.isPlaying)
            {
                playAudioSources.Add(audio);
                audio.Pause();
            }
        }
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        foreach (AudioSource audio in playAudioSources)
        {
            audio.Play();
        }
        playAudioSources.Clear();
    }
    public void resetHP()
    {
        currentHP = maxHP;
        UpdateHealthUI();
    }
}
