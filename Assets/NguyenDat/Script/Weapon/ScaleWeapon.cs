using UnityEngine;

public class ScaleWeapon : MonoBehaviour
{
    private TestPlayerMove testPlayerMove;
    public JoystickAttackAndAim joystickAttackAndAim;
    public WeaponData weaponData;

    private void Awake()
    {
        testPlayerMove = GetComponentInParent<TestPlayerMove>();
        weaponData = GetComponentInChildren<WeaponData>();
    }

    private void Update()
    {
        if (testPlayerMove == null || joystickAttackAndAim == null) return;

        // Chỉ thay đổi scale khi đang giữ joystick tấn công
        if (joystickAttackAndAim.IsHolding)
        {
            Vector3 scale = transform.localScale;
            if (weaponData == null) return;
            if (weaponData.weaponType == WeaponType.Sword ||
                weaponData.weaponType == WeaponType.MagicStaff ||
                weaponData.weaponType == WeaponType.Rogue ||
                weaponData.weaponType == WeaponType.Spear||
                weaponData.weaponType == WeaponType.SpellBook)
            {
                scale.x = testPlayerMove.isFacingRight ? 1 : -1;
                scale.y = testPlayerMove.isFacingRight ? 1 : -1;
                transform.localScale = scale;
            }
            else if (weaponData.weaponType == WeaponType.Bow)
            {
                scale.x = testPlayerMove.isFacingRight ? 1 : -1;
                transform.localScale = scale;
            }
            else
            {
                return;
            }
            
        }
    }
}
