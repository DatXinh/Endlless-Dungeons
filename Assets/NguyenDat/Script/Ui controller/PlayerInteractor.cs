using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public IInteractable currentInteractable;
    public Transform weaponParent;

    public Image WeaponIcon;
    public int Coins = 100;

    public TestWeaponAtk testWeaponAtk;
    public ScaleWeapon scaleWeapon;

    public PlayerHP playerHP;
    public PLayerMP playerMP;

    private GameObject[] weaponSlots = new GameObject[2]; // 0: chính, 1: phụ
    private int activeWeaponIndex = 0;

    public TextMeshProUGUI CointsText;
    private AudioSource audioSource;

    private void Awake()
    {
        testWeaponAtk = weaponParent.GetComponent<TestWeaponAtk>();
        scaleWeapon = weaponParent.GetComponent<ScaleWeapon>();
        audioSource = GetComponent<AudioSource>();
        if (CointsText != null)
        {
            CointsText.text = Coins.ToString();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentInteractable == null)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                if (interactable is WeaponInteractable weapon)
                {
                    weapon.weaponParent = this.weaponParent;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentInteractable != null && other.gameObject == ((MonoBehaviour)currentInteractable).gameObject)
        {
            currentInteractable = null;
        }
    }

    // Gọi từ nút UI
    public void Interact()
    {
        if (currentInteractable is WeaponInteractable weaponInteractable)
        {
            GameObject newWeapon = ((MonoBehaviour)weaponInteractable).gameObject;

            // Lưu lại vị trí vũ khí mới trên mặt đất
            Vector3 droppedPos = newWeapon.transform.position;
            Quaternion droppedRot = newWeapon.transform.rotation;

            weaponInteractable.weaponParent = weaponParent;

            // Kiểm tra nếu là vũ khí bán
            if (weaponInteractable.isSale)
            {
                if (Coins >= weaponInteractable.weaponPrice)
                {
                    Coins -= weaponInteractable.weaponPrice;
                    setCoinNumber();
                }
                else
                {
                    return;
                }
            }

            // Slot vũ khí
            if (weaponSlots[0] == null)
            {
                weaponSlots[0] = newWeapon;
                activeWeaponIndex = 0;
                EquipWeapon(0);
            }
            else if (weaponSlots[1] == null)
            {
                weaponSlots[1] = newWeapon;
                activeWeaponIndex = 1;
                EquipWeapon(1);
            }
            else
            {
                int current = activeWeaponIndex;
                GameObject droppedWeapon = weaponSlots[current];

                droppedWeapon.transform.SetParent(null);
                droppedWeapon.transform.position = droppedPos;
                droppedWeapon.transform.rotation = droppedRot;
                droppedWeapon.transform.localScale = Vector3.one;
                droppedWeapon.SetActive(true);
                SceneManager.MoveGameObjectToScene(droppedWeapon, SceneManager.GetActiveScene());
                WeaponInteractable droppedWeaponInteractable = droppedWeapon.GetComponent<WeaponInteractable>();
                if (droppedWeaponInteractable != null)
                {
                    droppedWeaponInteractable.isEquip = false;
                }

                Collider2D col = droppedWeapon.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                weaponSlots[current] = newWeapon;
                EquipWeapon(current);
            }
            weaponInteractable.Interact();
        }
        else if (currentInteractable is HPInteracable hPInteracable)
        {
            if (Coins > 30)
            {
                playerHP.Heal(hPInteracable.healAmount);
                hPInteracable.Interact();
                if (hPInteracable.isSale)
                {
                    Coins -= hPInteracable.CoinCost;
                }
                setCoinNumber();
            }
        }
        else if (currentInteractable is MPInteractable mPInteracable)
        {
            if (Coins > 30)
            {
                playerMP.RecoverMP(mPInteracable.ManaAmount);
                mPInteracable.Interact();
                if (mPInteracable.isSale)
                {
                    Coins -= mPInteracable.CoinCost;
                }
                setCoinNumber();
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    public void SwapWeapon()
    {
        if (weaponSlots[0] != null && weaponSlots[1] != null)
        {
            int newIndex = 1 - activeWeaponIndex;
            EquipWeapon(newIndex);
            GameObject newWeapon = weaponSlots[activeWeaponIndex];
            if (newWeapon != null)
            {
                WeaponInteractable weaponInteractable = newWeapon.GetComponent<WeaponInteractable>();
                if (weaponInteractable != null)
                {
                    weaponInteractable.weaponParent = this.weaponParent;
                    weaponInteractable.updateWeapon();
                }
            }
        }
    }

    private void EquipWeapon(int index)
    {
        if (weaponSlots[index] == null) return;

        activeWeaponIndex = index;

        for (int i = 0; i < 2; i++)
        {
            GameObject weapon = weaponSlots[i];
            if (weapon == null) continue;

            if (i == index)
            {
                weapon.transform.SetParent(weaponParent);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.SetActive(true);

                Collider2D col = weapon.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                testWeaponAtk.resetWeapon();
                scaleWeapon.resetWeapon();

                // UI icon
                WeaponInteractable weaponInteractable = weapon.GetComponent<WeaponInteractable>();
                if (weaponInteractable != null && weaponInteractable.weaponIcon != null && WeaponIcon != null)
                {
                    weaponInteractable.isEquip = true;
                    WeaponIcon.sprite = weaponInteractable.weaponIcon;
                    WeaponIcon.enabled = true;
                }
            }
            else
            {
                weapon.transform.SetParent(null);
                weapon.SetActive(false);
            }
        }
    }

    public void earnCoins(int amount)
    {
        Debug.Log("Earned Coins: " + amount);
        Coins += amount;
        setCoinNumber();
        if (audioSource != null) audioSource.Play();
    }

    public void setCoinNumber()
    {
        if (CointsText != null)
        {
            CointsText.text = Coins.ToString();
        }
    }

    public void RemoveCurrentWeapon()
    {
        if (weaponSlots[0] == null && weaponSlots[1] == null)
            return;

        if (weaponSlots[0] != null && weaponSlots[1] != null)
        {
            GameObject weaponToRemove = weaponSlots[activeWeaponIndex];
            Destroy(weaponToRemove);

            int otherIndex = 1 - activeWeaponIndex;
            weaponSlots[activeWeaponIndex] = null;
            weaponSlots[0] = weaponSlots[otherIndex];
            weaponSlots[1] = null;
            activeWeaponIndex = 0;
            EquipWeapon(0);
        }
        else
        {
            if (weaponSlots[0] != null)
            {
                Destroy(weaponSlots[0]);
                weaponSlots[0] = null;
            }
            else if (weaponSlots[1] != null)
            {
                Destroy(weaponSlots[1]);
                weaponSlots[1] = null;
            }
            activeWeaponIndex = 0;
            if (WeaponIcon != null)
                WeaponIcon.enabled = false;
        }
    }

    public void RemoveAllWeapons()
    {
        playerHP.resetHP();
        playerMP.resetMp();
        Coins = 100;
        setCoinNumber();
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i] != null)
            {
                Destroy(weaponSlots[i]);
                weaponSlots[i] = null;
            }
        }
        activeWeaponIndex = 0;
        if (WeaponIcon != null)
            WeaponIcon.enabled = false;
    }

    // ✅ thêm hàm cho Firebase
    public GameObject GetWeapon(int index)
    {
        if (index >= 0 && index < 2)
        {
            return weaponSlots[index];
        }
        return null;
    }

    // ✅ load dữ liệu từ Firebase
    public void SetPlayerData(UserProfileData data)
    {
        if (data == null) return;

        Coins = data.coin;
        setCoinNumber();

        if (playerHP != null)
        {
            playerHP.currentHP = Mathf.Clamp(data.hp, 0, playerHP.maxHP);
            playerHP.UpdateHealthUI();
        }
        if (playerMP != null)
        {
            playerMP.currentMP = Mathf.Clamp(data.mp, 0, playerMP.maxMP);
            playerMP.UpdateManaUI();
        }

        // Weapons: ở đây chỉ log tên, vì bạn cần cơ chế spawn prefab từ tên vũ khí
        Debug.Log("⚔ Firebase weapons: " + string.Join(", ", data.weapons));
    }
}
