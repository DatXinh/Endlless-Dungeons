using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform spriteTransform;
    public Transform weaponParent;

    public float enemyDetectRadius ; // Bán kính kiểm tra enemy gần
    public LayerMask enemyLayer;         // Layer của enemy, set trong Inspector
    public bool isEnemyNearby;           // True nếu có enemy ở gần

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;
    private Animator animator;

    private TestWeaponAtk testWeaponAtk;

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

        // Kiểm tra có enemy ở gần không
        Collider2D enemy = Physics2D.OverlapCircle(transform.position, enemyDetectRadius, enemyLayer);
        isEnemyNearby = enemy != null;

        if (spriteTransform == null)
            return;

        if (isEnemyNearby && enemy != null)
        {
            // Quay mặt về phía enemy gần nhất
            float dir = enemy.transform.position.x - transform.position.x;
            if (dir != 0)
            {
                bool facingRight = dir > 0;
                transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
            }
        }
        else if (moveInput.x != 0)
        {
            // Quay mặt theo hướng di chuyển
            bool facingRight = moveInput.x > 0;
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }

    // Vẽ bán kính kiểm tra enemy trong Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRadius);
    }
}
