using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PLayerMP : MonoBehaviour
{
    public int maxMP; // Maximum mana points
    public int currentMP;   // Current mana points

    public Image manaBar; // Reference to the mana bar UI element
    public GameObject ManaPopupText;
    public TMP_Text maxMPText;
    public TMP_Text currentMPText;

    void Start()
    {
        currentMP = maxMP;
        if (maxMPText != null)
        {
            maxMPText.text = maxMP.ToString();
        }
        if (currentMPText != null)
        {
            currentMPText.text = currentMP.ToString();
        }
        UpdateManaUI();
    }

    // Method to use mana
    public bool UseMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            UpdateManaUI();
            return true;
        }
        else
        {
            return false;
        }
    }

    // Method to recover mana
    public void RecoverMP(int amount)
    {
        if (currentMP >= 0)
        {
            currentMP += amount;
            if (currentMP > maxMP)
            {
                currentMP = maxMP;
            }
            UpdateManaUI();

            if (ManaPopupText != null)
            {
                GameObject popup = Instantiate(ManaPopupText, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = popup.GetComponent<FloatingDamage>();
                if (floatingDamage != null)
                {
                    floatingDamage.SetDamageValue(amount, Color.blue);
                }
            }
        }
    }

    public void UpdateManaUI()
    {
        if (manaBar != null)
        {
            manaBar.fillAmount = (float)currentMP / maxMP;
        }
        if (currentMPText != null)
        {
            currentMPText.text = currentMP.ToString();
        }
    }

    public void resetMp()
    {
        currentMP = maxMP;
        UpdateManaUI();
    }

    // ✅ Hàm mới để áp MP khi load dữ liệu từ Firebase
    public void SetMP(int value)
    {
        currentMP = Mathf.Clamp(value, 0, maxMP);
        UpdateManaUI();
    }
}
