using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    public IInteractable currentInteractable;
    private GameObject currentWeapon;
    public Transform weaponParent;    // Gán trong inspector
    public TestWeaponAtk testWeaponAtk; // Tham chiếu đến TestWeaponAtk
    public ScaleWeapon scaleWeapon; // Tham chiếu đến ScaleWeapon

    // ✅ Thêm để lưu lại vị trí vũ khí mới trước khi đổi
    private Vector3 lastWeaponDroppedPosition;
    private Quaternion lastWeaponDroppedRotation;

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
        if (currentInteractable != null)
        {
            if (currentInteractable is WeaponInteractable weaponInteractable)
            {
                // Nếu là vũ khí, gán cha cho vũ khí
                weaponInteractable.weaponParent = this.weaponParent;

                // ✅ Lưu vị trí hiện tại của vũ khí mới
                GameObject newWeaponGO = ((MonoBehaviour)weaponInteractable).gameObject;
                lastWeaponDroppedPosition = newWeaponGO.transform.position;
                lastWeaponDroppedRotation = newWeaponGO.transform.rotation;

                // ✅ Nếu đang cầm vũ khí → đưa nó về chỗ cũ của vũ khí mới
                if (currentWeapon != null)
                {
                    currentWeapon.transform.SetParent(null);
                    currentWeapon.transform.position = lastWeaponDroppedPosition;
                    currentWeapon.transform.rotation = lastWeaponDroppedRotation;
                    currentWeapon.SetActive(true);

                    Collider2D oldCol = currentWeapon.GetComponent<Collider2D>();
                    if (oldCol != null)
                    {
                        oldCol.enabled = true;
                    }
                }

                // ✅ Gán vũ khí mới thành vũ khí hiện tại
                currentWeapon = newWeaponGO;
                currentWeapon.SetActive(true);

                // Gọi Interact() như cũ
                weaponInteractable.Interact();
            }
        }
    }
}
