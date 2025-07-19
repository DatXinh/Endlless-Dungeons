using UnityEngine;

public class WeaponInteractable : MonoBehaviour, IInteractable
{
    public WeaponData weaponData; // Dữ liệu vũ khí
    public string weaponName = "Tên vũ khí";
    public Collider2D interacCollider;
    public Transform weaponParent;

    private void Start()
    {
        weaponData = GetComponent<WeaponData>();
        weaponName = weaponData.weaponName;
        interacCollider = GetComponent<Collider2D>();
    }
    public void Interact()
    {
        if (weaponParent != null)
        {
            Debug.Log("Gán weaponParent: " + weaponParent.name);
            transform.SetParent(weaponParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            interacCollider.enabled = false;
            Debug.Log("Đã gắn vào: " + transform.parent.name);
            // Gọi hàm để cập nhật vũ khí trong TestWeaponAtk
            TestWeaponAtk testWeaponAtk = weaponParent.GetComponent<TestWeaponAtk>();
            ScaleWeapon scaleWeapon = weaponParent.GetComponent<ScaleWeapon>();
            testWeaponAtk.resetWeapon();
            scaleWeapon.resetWeapon();
            LaunchProjectile launchProjectile = GetComponentInChildren<LaunchProjectile>();
            if (launchProjectile != null)
            {
                launchProjectile.resetPlayerMP();
            }
            // Cập nhật scale của vũ khí
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            Debug.LogWarning("weaponParent bị null khi nhặt vũ khí.");
        }

    }
    public string GetInteractionPrompt()
    {
        return "nhặt vũ khí hahahaha";
    }
}
