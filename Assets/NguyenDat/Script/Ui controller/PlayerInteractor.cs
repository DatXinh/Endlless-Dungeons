using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractor : MonoBehaviour
{
    public IInteractable currentInteractable;
    public Transform weaponParent;

    public Image WeaponIcon;

    public TestWeaponAtk testWeaponAtk;
    public ScaleWeapon scaleWeapon;

    private GameObject[] weaponSlots = new GameObject[2]; // 0: chính, 1: phụ
    private int activeWeaponIndex = 0; // slot hiện tại đang dùng

    private void Awake()
    {
        testWeaponAtk = weaponParent.GetComponent<TestWeaponAtk>();
        scaleWeapon = weaponParent.GetComponent<ScaleWeapon>();
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

            // Trường hợp: chưa có vũ khí nào
            if (weaponSlots[0] == null)
            {
                weaponSlots[0] = newWeapon;
                activeWeaponIndex = 0;
                EquipWeapon(0);
            }
            // Trường hợp: có 1 vũ khí → gán vào slot phụ
            else if (weaponSlots[1] == null)
            {
                weaponSlots[1] = newWeapon;
                activeWeaponIndex = 1;
                EquipWeapon(1);
            }
            // Trường hợp: đã có đủ 2 vũ khí → tráo vũ khí đang dùng
            else
            {
                int current = activeWeaponIndex;
                GameObject droppedWeapon = weaponSlots[current];

                // Thả vũ khí cũ ra vị trí của vũ khí mới
                droppedWeapon.transform.SetParent(null);
                droppedWeapon.transform.position = droppedPos;
                droppedWeapon.transform.rotation = droppedRot;
                droppedWeapon.SetActive(true);

                Collider2D col = droppedWeapon.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                // Gán vũ khí mới vào slot hiện tại
                weaponSlots[current] = newWeapon;
                EquipWeapon(current);
            }

            weaponInteractable.Interact();
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
                    weaponInteractable.weaponParent = this.weaponParent; // đảm bảo gán lại
                    weaponInteractable.updateWeapon(); // hoặc hàm bạn muốn gọi
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

                // ✅ Cập nhật hình ảnh UI
                WeaponInteractable weaponInteractable = weapon.GetComponent<WeaponInteractable>();
                if (weaponInteractable != null && weaponInteractable.weaponIcon != null && WeaponIcon != null)
                {
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

}
