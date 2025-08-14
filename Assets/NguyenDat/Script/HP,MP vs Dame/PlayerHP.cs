using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100; // Maximum health points
    public int currentHP; // Current health points
    public int InvincibilityTime = 1; // Time in seconds for invincibility after taking damage
    public GameObject damagePopupPrefab; // Prefab for the damage popup
    public Image healthBar; // Reference to the health bar UI element
    public TMP_Text maxHeal; // Reference to the health text UI element
    public TMP_Text currentHeal; // Reference to the current health text UI element

    public bool isInvincible = false; // Trạng thái bất tử

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
            healthBar.fillAmount = 1f;
        }
        if (maxHeal != null)
        {
            maxHeal.text = maxHP.ToString();
        }
        UpdateHealthUI();
        deadMesseng.SetActive(false);

        // Lắng nghe sự kiện load scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInvincible = false; // Reset bất tử khi vào scene mới
    }

    public void TakeDamage(int damage)
    {
        if (currentHP > 0)
        {
            if (!isInvincible)
            {
                currentHP -= damage;
                UpdateHealthUI();

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

    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(InvincibilityTime);
        isInvincible = false;
    }

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
        allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
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

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
