using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponListManager : MonoBehaviour
{
    public GameObject weaponItemPrefab;
    public Transform contentParent; // Content trong ScrollView
    public List<WeaponData> allWeapons; // Drag vào trong Inspector

    void Start()
    {
        PopulateWeaponList();
    }

    void PopulateWeaponList()
    {
        foreach (WeaponData weapon in allWeapons)
        {
            GameObject item = Instantiate(weaponItemPrefab, contentParent);

            // Gán các thành phần UI
            var icon = item.transform.Find("Icon").GetComponent<Image>();
            var nameText = item.transform.Find("Name").GetComponent<TMP_Text>();
            var damageText = item.transform.Find("Damage").GetComponent<TMP_Text>();
            var critText = item.transform.Find("Critical").GetComponent<TMP_Text>();
            var manaText = item.transform.Find("Mana").GetComponent<TMP_Text>();
            var descText = item.transform.Find("Description").GetComponent<TMP_Text>();

            // Gán dữ liệu
            icon.sprite = weapon.weaponIcon;
            nameText.text = weapon.weaponName;
            damageText.text = $"Sát thương: {weapon.weaponDamage}";
            critText.text = $"Tỉ lệ chí mạng: {weapon.weaponCriticalChange}%";
            manaText.text = $"Năng lượng tiêu hao: {weapon.weaponManaCost}";
            descText.text = weapon.weaponDescription;

            // Gán màu theo cấp độ
            nameText.color = weapon.weaponLevel switch
            {
                1 => Color.white,
                2 => Color.yellow,
                3 => Color.red,
                _ => Color.gray
            };
        }

    }
}
