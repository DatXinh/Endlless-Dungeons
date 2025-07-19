using UnityEngine;
using UnityEngine.UI;

public class WeaponInteractable : MonoBehaviour, IInteractable
{
    public WeaponData weaponData; // Dữ liệu vũ khí
    public string weaponName = "Tên vũ khí";
    public Sprite weaponIcon;
    public Collider2D interacCollider;
    public Transform weaponParent;

    private void Start()
    {
        weaponData = GetComponent<WeaponData>();
        weaponName = weaponData.weaponName;
        weaponIcon = weaponData.weaponIcon;
        interacCollider = GetComponent<Collider2D>();
    }
    public void Interact()
    {
        if (weaponParent != null)
        {
            transform.SetParent(weaponParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            interacCollider.enabled = false;
            updateWeapon();
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
    public void updateWeapon()
    {
        TestWeaponAtk testWeaponAtk = weaponParent.GetComponent<TestWeaponAtk>();
        ScaleWeapon scaleWeapon = weaponParent.GetComponent<ScaleWeapon>();
        testWeaponAtk.resetWeapon();
        scaleWeapon.resetWeapon();
        LaunchProjectile launchProjectile = GetComponentInChildren<LaunchProjectile>();
        if (launchProjectile != null)
        {
            launchProjectile.resetPlayerMP();
        }
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        weaponIcon = weaponData.weaponIcon;
    }
}
