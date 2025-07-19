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
        currentMP = maxMP; // Initialize current mana to maximum mana
        if (maxMPText != null)
        {
            maxMPText.text = maxMP.ToString(); // Set the maximum mana text
        }
        if (currentMPText != null)
        {
            currentMPText.text = currentMP.ToString(); // Set the current mana text
        }
    }

    void Update()
    {
        
    }

    // Method to use mana
    public bool UseMP(int amount)
    {
        if (currentMP >= amount)
        {
            currentMP -= amount;
            if (manaBar != null)
            {
                manaBar.fillAmount = (float)currentMP / maxMP; // Update mana bar
            }
            if (currentMPText != null)
            {
                currentMPText.text = currentMP.ToString(); // Update current mana text
            }
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
            if (manaBar != null)
            {
                manaBar.fillAmount = (float)currentMP / maxMP; // Update mana bar
            }
            if (currentMPText != null)
            {
                currentMPText.text = currentMP.ToString(); // Update current mana text
            }
            if(ManaPopupText != null)
            {
                GameObject popup = Instantiate(ManaPopupText, transform.position, Quaternion.identity);
                FloatingDamage floatingDamage = popup.GetComponent<FloatingDamage>();
                if (floatingDamage != null)
                {
                    floatingDamage.SetDamageValue(amount, Color.blue); // Set the mana recovery value and color
                }
            }
            if (currentMP > maxMP)
            {
                currentMP = maxMP;
            }
        }
    }
}
