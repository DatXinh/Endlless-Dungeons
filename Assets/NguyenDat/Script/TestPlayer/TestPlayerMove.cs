using System;
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
        animator = GetComponent<Animator>();
    }

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;

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
}
