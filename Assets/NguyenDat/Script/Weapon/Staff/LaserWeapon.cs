using UnityEngine;

public class LaserWeapon : MonoBehaviour
{
    [Header("Weapon data")]
    public WeaponData weaponData;
    [Header("References")]
    public LineRenderer lineRenderer;
    public Transform startPoint;

    private PLayerMP playerMP;

    [Header("Laser Settings")]
    public float maxLaserLength = 25f;
    public int damage;
    public int manaCost;
    public int CriticalChange;
    public LayerMask hitMask;

    private bool isFiring = false;
    private Coroutine manaDrainCoroutine;

    public AudioSource audioSource;

    private void Awake()
    {
        weaponData = GetComponentInParent<WeaponData>();
        damage = weaponData.weaponDamage;
        manaCost = weaponData.weaponManaCost;
        CriticalChange = weaponData.weaponCriticalChange;
        manaCost = weaponData.weaponManaCost;
    }

    void Update()
    {
        if (isFiring)
        {
            UpdateLaser();
        }
    }
    public void StartFiring()
    {
        if (isFiring) return;
        isFiring = true;
        lineRenderer.enabled = true;
        UpdateLaser();
        audioSource.Play();
        manaDrainCoroutine = StartCoroutine(DrainManaRoutine());

    }
    public void StopFiring()
    {
        if (!isFiring) return;

        isFiring = false;
        lineRenderer.enabled = false;
        if (manaDrainCoroutine != null)
        {
            StopCoroutine(manaDrainCoroutine);
            manaDrainCoroutine = null;
        }
    }

    private void UpdateLaser()
    {
        if (playerMP == null)
        {
            playerMP = GetComponentInParent<PLayerMP>();
        }
        if(manaCost > playerMP.currentMP)
        {
            StopFiring();
            return;
        }
        if (startPoint == null) return;

        Vector3 origin = startPoint.position;
        Vector3 direction = startPoint.right;
        Vector3 endPos = origin + direction * maxLaserLength;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, maxLaserLength, hitMask);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    endPos = hit.point;
                }
                else if (hit.collider.CompareTag("Enemy")|| hit.collider.CompareTag("Boss"))
                {
                    EnemyHP enemyHP = hit.collider.GetComponent<EnemyHP>();
                    int baseDamage = damage;
                    int critRate = CriticalChange;

                    bool isCritical = Random.Range(0, 100) < critRate;
                    int finalDamage = baseDamage;

                    if (isCritical)
                    {
                        float critMultiplier = Random.Range(1.5f, 3.0f);
                        finalDamage = Mathf.RoundToInt(baseDamage * critMultiplier);
                    }
                    enemyHP.TakeDamage(finalDamage, isCritical);
                }
            }
        }
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, endPos);
    }

    // Coroutine trừ mana mỗi 0.25 giây
    private System.Collections.IEnumerator DrainManaRoutine()
    {
        while (isFiring)
        {
            if (playerMP.UseMP(manaCost))
            {
                yield return new WaitForSeconds(0.25f);
            }
            else
            {
                StopFiring();
                yield break;
            }
        }
    }
}
