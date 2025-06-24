using UnityEngine;

public class ScaleWeapon : MonoBehaviour
{
    private TestPlayerMove testPlayerMove;
    public JoystickAttackAndAim joystickAttackAndAim;

    private void Awake()
    {
        testPlayerMove = GetComponentInParent<TestPlayerMove>();
    }

    private void Update()
    {
        if (testPlayerMove == null || joystickAttackAndAim == null) return;

        // Chỉ thay đổi scale khi đang giữ joystick tấn công
        if (joystickAttackAndAim.IsHolding)
        {
            Vector3 scale = transform.localScale;
            scale.x = testPlayerMove.isFacingRight ? 1 : -1;
            transform.localScale = scale;
        }
    }
}
