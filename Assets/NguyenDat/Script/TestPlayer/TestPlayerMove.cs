using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform spriteTransform;
    public Transform weaponParent;

    public float enemyDetectRadius;
    public LayerMask enemyLayer;
    public bool isEnemyNearby;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;
    private Animator animator;

    private TestWeaponAtk testWeaponAtk;
    public TestWeaponLockEnemy testWeaponLockEnemy;

    private bool isUsingMagicWeapon = false;
    private Transform weaponTransform;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        testWeaponAtk = GetComponentInChildren<TestWeaponAtk>();

        if (spriteTransform == null)
        {
            Debug.LogError("spriteTransform is not assigned in the inspector.");
        }
        else
        {
            animator = spriteTransform.GetComponent<Animator>();
        }

        // Kiểm tra weapon hiện tại có phải là gậy phép hoặc sách phép
        if (weaponParent != null && weaponParent.childCount > 0)
        {
            weaponTransform = weaponParent.GetChild(0); // Giả sử luôn có 1 vũ khí
            WeaponData weaponData = weaponTransform.GetComponent<WeaponData>();

            if (weaponData != null &&
                (weaponData.weaponType == WeaponType.MagicStaff || weaponData.weaponType == WeaponType.SpellBook))
            {
                isUsingMagicWeapon = true;
            }
        }

        if (testWeaponLockEnemy == null)
            testWeaponLockEnemy = GetComponentInChildren<TestWeaponLockEnemy>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Attack.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Attack.canceled -= OnAttackCanceled;

        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        testWeaponAtk.Attack();
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        testWeaponAtk.Attack();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        // Tìm enemy gần nhất trong bán kính
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, enemyDetectRadius, enemyLayer);
        Transform nearestEnemy = null;
        float minDist = float.MaxValue;
        foreach (var hit in hits)
        {
            float dist = (hit.transform.position - transform.position).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                nearestEnemy = hit.transform;
            }
        }
        isEnemyNearby = nearestEnemy != null;

        // Truyền enemy gần nhất cho TestWeaponLockEnemy (nếu có)
        if (testWeaponLockEnemy != null)
            testWeaponLockEnemy.nearestEnemy = nearestEnemy;

        if (spriteTransform == null)
            return;

        float dir = 0f;
        if (isEnemyNearby && nearestEnemy != null)
        {
            dir = nearestEnemy.position.x - transform.position.x;
        }
        else if (moveInput.x != 0)
        {
            dir = moveInput.x;
        }

        if (dir != 0)
        {
            bool facingRight = dir > 0;
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);

            if (isUsingMagicWeapon && weaponTransform != null)
            {
                Vector3 scale = weaponTransform.localScale;
                if (facingRight)
                {
                    scale.x = Mathf.Abs(scale.x);
                    scale.y = Mathf.Abs(scale.y);
                }
                else
                {
                    scale.x = -Mathf.Abs(scale.x);
                    scale.y = -Mathf.Abs(scale.y);
                }
                weaponTransform.localScale = scale;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRadius);
    }
}
