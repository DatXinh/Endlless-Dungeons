using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Transform spriteTransform;
    public Transform weaponParent;

    public JoystickAttackAndAim joystickAttackAndAim;

    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    public float Speed;
    public float dashDistane = 7.5f;
    public float dashDuration = 0.2f;
    public bool isFacingRight = true;

    private static TestPlayerMove instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

        // Cập nhật biến Speed khi di chuyển
        Speed = rb.linearVelocity.magnitude;

        // Cập nhật biến Speed trong Animator
        if (animator != null)
        {
            animator.SetFloat("Speed", Speed > 0.01f ? 1f : 0f);
        }

        if (spriteTransform == null)
            return;

        float dir = 0f;

        // Ưu tiên hướng tấn công nếu đang giữ joystick tấn công
        if (joystickAttackAndAim != null && joystickAttackAndAim.isHolding)
        {
            Vector2 aimDir = joystickAttackAndAim.aimDirection;
            if (aimDir.x != 0)
                dir = aimDir.x;
        }
        else if (moveInput.x != 0)
        {
            dir = moveInput.x;
        }

        if (dir != 0)
        {
            isFacingRight = dir > 0;
            transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Đặt lại vị trí mỗi khi load scene mới
        transform.position = Vector3.zero; // (0,0,0)
    }

    private void OnDestroy()
    {
        // Gỡ đăng ký nếu object bị huỷ
        if (instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Dash()
    {
        Debug.Log("Dash called");
        Vector2 dashDirection = moveInput.normalized;
        // Nếu không có hướng di chuyển, dash theo hướng nhìn
        if (dashDirection == Vector2.zero)
        {
            dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        }
        StartCoroutine(DashCoroutine(dashDirection, dashDistane, dashDuration));
    }

    IEnumerator DashCoroutine(Vector2 dashDirection, float dashDistance, float dashDuration)
    {
        float dashSpeed = dashDistance / dashDuration;
        float elapsed = 0f;
        // Lưu lại tốc độ ban đầu
        float originalMoveSpeed = 10;
        // Tạm thời tăng tốc độ
        moveSpeed = dashSpeed;
        while (elapsed < dashDuration)
        {
            rb.linearVelocity = dashDirection * moveSpeed;
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Khôi phục tốc độ ban đầu
        moveSpeed = originalMoveSpeed;
        rb.linearVelocity = Vector2.zero;
    }
}
