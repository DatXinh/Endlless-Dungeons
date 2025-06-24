using UnityEngine;

public class ScaleWeapon : MonoBehaviour
{
    private TestPlayerMove testPlayerMove;
    private JoystickAttackAndAim joystickAttackAndAim;
    private bool lastFacingRight;
    private Vector3 originalScale;

    private void Awake()
    {
        testPlayerMove = GetComponentInParent<TestPlayerMove>();
        joystickAttackAndAim = GetComponentInParent<JoystickAttackAndAim>();
        originalScale = transform.localScale;
        if (testPlayerMove != null)
            lastFacingRight = testPlayerMove.isFacingRight;
        SetScale(lastFacingRight);
    }

    private void Update()
    {
        if (testPlayerMove == null || joystickAttackAndAim == null) return;

        // Chỉ thay đổi scale khi đang giữ joystick tấn công
        if (joystickAttackAndAim.IsHolding)
        {
            if (testPlayerMove.isFacingRight != lastFacingRight)
            {
                SetScale(testPlayerMove.isFacingRight);
                lastFacingRight = testPlayerMove.isFacingRight;
            }
        }
    }

    private void SetScale(bool facingRight)
    {
        // Chỉ thay đổi trục x, giữ nguyên y và z
        float newX = facingRight ? Mathf.Abs(originalScale.x) : -Mathf.Abs(originalScale.x);
        if (transform.localScale.x != newX)
        {
            transform.localScale = new Vector3(newX, originalScale.y, originalScale.z);
        }
    }
}
