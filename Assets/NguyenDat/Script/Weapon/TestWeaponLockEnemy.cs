using UnityEngine;

public class TestWeaponLockEnemy : MonoBehaviour
{
    [HideInInspector]
    public Transform nearestEnemy;
    [HideInInspector]
    public LaunchProjectile launchProjectile;

    public bool isLockingTarget { get; private set; }
    public TestPlayerMove playerMove;

    [Header("Laser Weapon")]
    public LaserWeapon laserWeapon; // Assign in inspector or via GetComponent

    private bool lastFacingRight = true;
    private float lastAngle = float.MinValue;

    void Awake()
    {
        playerMove = GetComponentInParent<TestPlayerMove>();
        launchProjectile = GetComponent<LaunchProjectile>();
        lastFacingRight = playerMove != null ? playerMove.isFacingRight : true;
        if (launchProjectile == null)
        {

        }

        // Optional: auto-assign LaserWeapon if not set in inspector
        if (laserWeapon == null)
            laserWeapon = GetComponent<LaserWeapon>();
    }

    void Update()
    {
        if (playerMove == null)
            return;

        bool isFacingRight = playerMove.isFacingRight;

        // Chỉ thực hiện code khi isFacingRight thay đổi hoặc khi không có nearestEnemy
        bool needUpdateScaleOrRotation = isFacingRight != lastFacingRight || nearestEnemy == null;

        if (needUpdateScaleOrRotation)
        {
            // Nếu không có enemy: scale.x = 1, scale.y = 1, rotation.z = 40 hoặc -40
            if (nearestEnemy == null)
            {
                isLockingTarget = false;
                Vector3 scale = transform.localScale;
                scale.x = 1;
                scale.y = 1;
                transform.localScale = scale;
                float angle = isFacingRight ? 40f : -40f;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                // Nếu có enemy, chỉ cập nhật scale khi hướng đổi
                Vector3 scale = transform.localScale;
                scale.x = isFacingRight ? 1 : -1;
                scale.y = isFacingRight ? 1 : -1;
                transform.localScale = scale;
            }
            lastFacingRight = isFacingRight;
        }

        // Nếu có enemy gần nhất thì xoay về hướng enemy đó (chỉ khi góc thay đổi đáng kể)
        if (nearestEnemy != null)
        {
            isLockingTarget = true;
            if (launchProjectile != null)
            {
                launchProjectile.nearestEnemy = nearestEnemy;
            }
            if(laserWeapon != null)
            {
                laserWeapon.nearestEnemy = nearestEnemy;
            }
            Vector3 direction = nearestEnemy.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (!Mathf.Approximately(angle, lastAngle))
            {
                transform.rotation = Quaternion.Euler(0, 0, angle);
                lastAngle = angle;
            }

        }
        else
        {
            if (launchProjectile != null)
            {
                launchProjectile.nearestEnemy = null;
            }
            if (laserWeapon != null)
            {
                laserWeapon.nearestEnemy = null;
            }
        }
    }
}
