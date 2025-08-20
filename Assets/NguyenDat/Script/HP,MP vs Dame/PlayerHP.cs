using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;   // 🔥 thêm Firebase

public class PlayerHP : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public int InvincibilityTime = 1;
    public GameObject damagePopupPrefab;
    public Image healthBar;
    public TMP_Text maxHeal;
    public TMP_Text currentHeal;

    public bool isInvincible = false;

    public Joystick joystick;
    public JoystickAttackAndAim joystickAttackAndAim;

    public GameObject rightPanel;
    public GameObject leftPanel;
    public GameObject deadMesseng;
    public GameObject pauseButton;
    public AudioSource[] allAudioSources;
    public List<AudioSource> playAudioSources = new List<AudioSource>();

    void Start()
    {
        currentHP = maxHP;
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInvincible = false;
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

            // 🔥 Clear run khi chết
            FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
            if (user != null)
            {
                FirebaseUserDataManager.Instance.ClearCurrentRun(user);
            }

            SceneManager.LoadScene("Home");
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

    // ✅ thêm hàm này để ApplyCurrentRunToPlayer gọi
    public void SetHP(int value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHP);
        UpdateHealthUI();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
