using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;

    public Transform spriteTransform;
    public Transform weaponParent;

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

        if (moveInput.x != 0 && spriteTransform != null)
        {
            bool facingRight = moveInput.x > 0;
            transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
        }
    }
}
