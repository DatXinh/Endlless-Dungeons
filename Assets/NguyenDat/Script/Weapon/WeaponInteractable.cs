using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponInteractable : MonoBehaviour, IInteractable
{
    public WeaponData weaponData;
    public bool isSale = false;
    public bool isEquip = false;
    [Header("Thông tin vũ khí")]
    public WeaponTooltipDisplay weaponTooltipDisplay; // Hiển thị thông tin vũ khí
    public string weaponName = "Tên vũ khí";
    public int weaponPrice;
    public Sprite weaponIcon;
    public Collider2D interacCollider;
    public Transform weaponParent;

    private void Start()
    {
        weaponData = GetComponent<WeaponData>();
        weaponName = weaponData.weaponName;
        weaponIcon = weaponData.weaponIcon;
        weaponPrice = weaponData.WeaponPrice;
        interacCollider = GetComponent<Collider2D>();
        weaponTooltipDisplay = GetComponent<WeaponTooltipDisplay>();
    }
    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (isEquip == false)
        {
            Destroy(gameObject);
        }
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
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            weaponTooltipDisplay.ShowTooltip();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            weaponTooltipDisplay.HideTooltip();
        }
    }
}
