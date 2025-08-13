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
    private int FinalWeaponDamage;

    private void Awake()
    {
        weaponData = GetComponent<WeaponData>();
        weaponLevel = weaponData.weaponLevel;
        if (LoopManager.Instance != null)
        {
            if (LoopManager.Instance.currentLoop > 0)
            {
                FinalWeaponDamage = (int)(weaponData.weaponDamage * (LoopManager.Instance.currentLoop * 0.2f));
            }
            else
            {
                FinalWeaponDamage = weaponData.weaponDamage; // Use base damage if no LoopManager
            }
        }
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
        weaponDama.text = FinalWeaponDamage.ToString();
        weaponCrit.text = weaponData.weaponCriticalChange.ToString() + "%";
        weaponMana.text = weaponData.weaponManaCost.ToString();
        weaponPrice.text = weaponData.WeaponPrice.ToString();
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
