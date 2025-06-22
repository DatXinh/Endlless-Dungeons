using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform spriteTransform;
    public Transform weaponParent;

    public float enemyDetectRadius;
    public bool isEnemyNearby;

    public static readonly List<Transform> allEnemies = new List<Transform>(); // Danh sách enemy toàn cục

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private InputSystem_Actions inputActions;
    private Animator animator;
    public bool isFacingRight = true;

    public TestWeaponLockEnemy testWeaponLockEnemy;
    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        testWeaponLockEnemy = GetComponentInChildren<TestWeaponLockEnemy>();
        animator = GetComponent<Animator>();
        if (testWeaponLockEnemy != null)
        {
            testWeaponLockEnemy.playerMove = this;
        }

        // Tìm toàn bộ các Enemy và Boss khi Awake
        allEnemies.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                allEnemies.Add(enemy.transform);
        }
        GameObject[] bosses = GameObject.FindGameObjectsWithTag("Boss");
        foreach (var boss in bosses)
        {
            if (boss != null && !allEnemies.Contains(boss.transform))
                allEnemies.Add(boss.transform);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        // Tìm enemy gần nhất trong bán kính bằng danh sách đã lưu (tối ưu, không FindGameObjectsWithTag)
        Transform nearestEnemy = null;
        float minDist = float.MaxValue;
        foreach (var enemy in allEnemies)
        {
            if (enemy == null) continue;
            float dist = (enemy.position - transform.position).sqrMagnitude;
            if (dist < enemyDetectRadius * enemyDetectRadius && dist < minDist)
            {
                minDist = dist;
                nearestEnemy = enemy;
            }
        }
        isEnemyNearby = nearestEnemy != null;

        // Truyền enemy gần nhất cho TestWeaponLockEnemy (nếu có) chỉ khi isEnemyNearby = true
        if (testWeaponLockEnemy != null)
        {
            if (isEnemyNearby)
            {
                testWeaponLockEnemy.nearestEnemy = nearestEnemy;
            }
            else
            {
                testWeaponLockEnemy.nearestEnemy = null;
            }
        }

        if (spriteTransform == null)
            return;

        float dir = 0f;
        if (isEnemyNearby && nearestEnemy != null)
        {
            // Luôn quay về phía enemy gần nhất, bỏ qua hướng di chuyển
            dir = nearestEnemy.position.x - transform.position.x;
        }
        else if (moveInput.x != 0)
        {
            // Chỉ quay theo hướng di chuyển nếu không có enemy trong tầm
            dir = moveInput.x;
        }

        if (dir != 0)
        {
            isFacingRight = dir > 0;
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectRadius);
    }
}