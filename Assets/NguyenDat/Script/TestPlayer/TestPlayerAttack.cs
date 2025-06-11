using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerAttack : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private TestWeaponAtk testWeaponAtk;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        testWeaponAtk = GetComponentInChildren<TestWeaponAtk>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Attack.performed += OnAttackPerformed;
        inputActions.Player.Attack.canceled += OnAttackCanceled;
    }

    private void OnDisable()
    {
        inputActions.Player.Attack.performed -= OnAttackPerformed;
        inputActions.Player.Attack.canceled -= OnAttackCanceled;
        inputActions.Player.Disable();
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (testWeaponAtk != null)
            testWeaponAtk.Attack();
    }

    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        if (testWeaponAtk != null)
            testWeaponAtk.Attack();
    }
    
}