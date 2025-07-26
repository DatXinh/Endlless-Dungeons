using UnityEngine;

public class SpawnAtDestroy : MonoBehaviour
{
    public GameObject spawnPrefab;
    public PWeaponDame weaponDame;

    public int weaponDamage;
    public int weaponCriticalChange;

    private bool shouldSpawn = false;

    private void Awake()
    {
        weaponDame = GetComponent<PWeaponDame>();
        if (weaponDame != null)
        {
            weaponDamage = weaponDame.weaponDamage;
            weaponCriticalChange = weaponDame.weaponCriticalChange;
        }
    }

    public void TriggerDestroy()
    {
        shouldSpawn = true;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying) return;

        if (shouldSpawn && spawnPrefab != null)
        {
            GameObject spawnedObject = Instantiate(spawnPrefab, transform.position, Quaternion.identity);
            PWeaponDame pWeaponDame = spawnedObject.GetComponent<PWeaponDame>();
            if (pWeaponDame != null)
            {
                pWeaponDame.weaponDamage = weaponDamage;
                pWeaponDame.weaponCriticalChange = weaponCriticalChange;
            }
        }
    }
}
