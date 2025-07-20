using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponTooltipDisplay : MonoBehaviour
{
    public GameObject tooltipPanel;
    public Image weaponIconImage;
    public TextMeshProUGUI weaponName;
    public TextMeshProUGUI weaponDama;
    public TextMeshProUGUI weaponCrit;
    public TextMeshProUGUI weaponMana;
    public TextMeshProUGUI weaponPrice;
    private WeaponData weaponData;
    public int weaponLevel;

    private void Awake()
    {
        weaponData = GetComponent<WeaponData>();
        weaponLevel = weaponData.weaponLevel;
        HideTooltip();
    }

    public void ShowTooltip()
    {
        if (weaponData == null) return;
        weaponIconImage.sprite = weaponData.weaponIcon;
        weaponName.text = weaponData.weaponName;
        if (weaponLevel == 1)
        {
            weaponName.color = Color.white;
        }
        else if( weaponLevel == 2)
        {
            weaponName.color = Color.green;
        }else
        {
            weaponName.color = Color.red;
        }
        weaponDama.text = weaponData.weaponDamage.ToString() + " Damage";
        weaponCrit.text = weaponData.weaponCriticalChange.ToString() + " % Critical change";
        weaponMana.text = "Mana Cost: " + weaponData.weaponManaCost.ToString();
        weaponPrice.text = "Price: " + weaponData.WeaponPrice.ToString() + " Gold";
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
