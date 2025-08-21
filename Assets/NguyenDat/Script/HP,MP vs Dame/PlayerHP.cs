using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Auth;

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
    public GameObject pauseButton;

    public AudioSource[] allAudioSources;
    public List<AudioSource> playAudioSources = new List<AudioSource>();

    [Header("Player References")]
    public PLayerMP playerMP;
    public PlayerInteractor playerInteractor;

    void Start()
    {
        currentHP = maxHP;
        if (healthBar != null) healthBar.fillAmount = 1f;
        if (maxHeal != null) maxHeal.text = maxHP.ToString();
        UpdateHealthUI();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isInvincible = false;

        // 👉 Nếu quay về Home thì reset HP và UI
        if (scene.name == "Home")
        {
            resetHP();
            RestoreUI();
            Time.timeScale = 1f;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHP > 0 && !isInvincible)
        {
            currentHP -= damage;
            UpdateHealthUI();

            if (damagePopupPrefab != null)
            {
                GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                if (floatingDamage != null) floatingDamage.SetDamageValue(damage, Color.red);
            }

            StartCoroutine(InvincibilityCoroutine());
        }

        if (currentHP <= 0)
        {
            OnPlayerDie();
        }
    }

    private void OnPlayerDie()
    {
        // 🔹 Tắt UI điều khiển
        HideUI();
        PauseGame();

        // 🔹 Lưu dữ liệu vào Firebase + PlayerPrefs
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null && playerInteractor != null && playerMP != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            int playTime = PlayerDontDestroyOnLoad.instance.GetTimePlayed();

            string[] weaponNames = new string[2];
            for (int i = 0; i < 2; i++)
            {
                var weapon = playerInteractor.GetWeapon(i);
                weaponNames[i] = weapon != null ? weapon.name.Replace("(Clone)", "").Trim() : "";
            }

            CurrentRunData runData = new CurrentRunData(
                0, // HP chết = 0
                playerMP.currentMP,
                playerInteractor.Coins,
                playTime,
                weaponNames,
                sceneName
            );

            FirebaseUserDataManager.Instance.SaveEndGameLog(user, runData, "Dead");

            string json = JsonUtility.ToJson(runData);
            PlayerPrefs.SetString("LastEndRun", json);
            PlayerPrefs.Save();
            Debug.Log("💾 Saved LastEndRun = " + json);
        }

        SceneManager.LoadScene("EndScene");
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
            if (currentHP > maxHP) currentHP = maxHP;

            UpdateHealthUI();

            if (damagePopupPrefab != null)
            {
                GameObject damagePopup = Instantiate(damagePopupPrefab, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = damagePopup.GetComponent<FloatingDamage>();
                if (floatingDamage != null) floatingDamage.SetDamageValue(amount, Color.green);
            }
        }
    }

    public void UpdateHealthUI()
    {
        float healthPercent = (float)currentHP / maxHP;
        if (healthBar != null) healthBar.fillAmount = healthPercent;
        if (currentHeal != null) currentHeal.text = currentHP.ToString();
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
        foreach (AudioSource audio in playAudioSources) audio.Play();
        playAudioSources.Clear();
    }

    public void resetHP()
    {
        currentHP = maxHP;
        UpdateHealthUI();
    }

    public void SetHP(int value)
    {
        currentHP = Mathf.Clamp(value, 0, maxHP);
        UpdateHealthUI();
    }

    private void HideUI()
    {
        if (leftPanel != null) leftPanel.SetActive(false);
        if (rightPanel != null) rightPanel.SetActive(false);
        if (pauseButton != null) pauseButton.SetActive(false);
    }

    private void RestoreUI()
    {
        if (leftPanel != null) leftPanel.SetActive(true);
        if (rightPanel != null) rightPanel.SetActive(true);
        if (pauseButton != null) pauseButton.SetActive(true);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
