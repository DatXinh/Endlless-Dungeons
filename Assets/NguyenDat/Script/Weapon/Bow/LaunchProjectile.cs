using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    [HideInInspector] public GameObject projectilePrefab;

    public Transform firePoint;
    public Transform weaponTransform;
    public float launchForce = 10f;
    public AudioSource audioSource;

    private WeaponData weaponData;
    private int projectileDamage;

    private void Awake()
    {
        weaponData = GetComponentInParent<WeaponData>();
        projectilePrefab = weaponData.weaponProjectile;
        projectileDamage = weaponData.weaponDamage;
    }

    public void LaunchSingle()
    {
        if (!IsValid()) return;

        Vector2 direction = weaponTransform.right.normalized;
        SpawnProjectile(firePoint.position, direction);
        PlayAudio();
    }

    public void LaunchDoubleSpread(float spreadAngle = 15f)
    {
        if (!IsValid()) return;

        Vector2 baseDir = weaponTransform.right.normalized;
        Vector2[] directions = {
            RotateDirection(baseDir, -spreadAngle),
            RotateDirection(baseDir, spreadAngle)
        };

        foreach (var dir in directions)
        {
            SpawnProjectile(firePoint.position, dir);
        }

        PlayAudio();
    }

    public void LaunchTripleCone(float spreadAngle = 15f)
    {
        if (!IsValid()) return;

        Vector2 baseDir = weaponTransform.right.normalized;
        Vector2[] directions = {
            baseDir,
            RotateDirection(baseDir, -spreadAngle),
            RotateDirection(baseDir, spreadAngle)
        };

        foreach (var dir in directions)
        {
            SpawnProjectile(firePoint.position, dir);
        }

        PlayAudio();
    }
    public void LaunchFiveSpread(float totalSpread = 30f)
    {
        if (!IsValid()) return;

        Vector2 baseDir = weaponTransform.right.normalized;
        float startAngle = -totalSpread / 2;
        float angleStep = totalSpread / 4f;

        for (int i = 0; i < 5; i++)
        {
            float angle = startAngle + angleStep * i;
            Vector2 dir = RotateDirection(baseDir, angle);
            SpawnProjectile(firePoint.position, dir);
        }

        PlayAudio();
    }

    public void LaunchCircularBurst(int bulletCount = 12)
    {
        if (!IsValid()) return;

        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            SpawnProjectile(firePoint.position, dir.normalized);
        }

        PlayAudio();
    }
    public void LaunchFanShot()
    {
        LaunchFanShot_Internal(5, 45f);
    }
    private void LaunchFanShot_Internal(int bulletCount = 7, float totalSpread = 45f)
    {
        if (!IsValid()) return;

        Vector2 baseDir = weaponTransform.right.normalized;
        float startAngle = -totalSpread / 2f;
        float angleStep = totalSpread / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 dir = RotateDirection(baseDir, angle);
            SpawnProjectile(firePoint.position, dir);
        }

        PlayAudio();
    }
    public void LaunchRandomSpread()
    {
        LaunchRandomSpread_Internal(5, 45f);
    }
    private void LaunchRandomSpread_Internal(int bulletCount = 6, float maxSpread = 20f)
    {
        if (!IsValid()) return;

        Vector2 baseDir = weaponTransform.right.normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = Random.Range(-maxSpread, maxSpread);
            Vector2 dir = RotateDirection(baseDir, angle);
            SpawnProjectile(firePoint.position, dir);
        }

        PlayAudio();
    }

    private GameObject SpawnProjectile(Vector2 position, Vector2 direction)
    {
        GameObject projectile = Instantiate(projectilePrefab, position, Quaternion.Euler(0f, 0f, GetAngle(direction)));

        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * launchForce;
        }

        return projectile;
    }

    private float GetAngle(Vector2 dir) => Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    private Vector2 RotateDirection(Vector2 direction, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * direction;
    }

    private bool IsValid()
    {
        return projectilePrefab != null && firePoint != null && weaponTransform != null;
    }

    private void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
