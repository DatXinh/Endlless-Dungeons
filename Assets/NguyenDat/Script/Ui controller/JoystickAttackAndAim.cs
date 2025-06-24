using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickAttackAndAim : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI Components")]
    public RectTransform joystickHandle;
    public RectTransform joystickBackground;

    [Header("Weapon Control")]
    public Transform weaponTransform;
    public TestWeaponAtk weaponAtk;

    // Thêm property để truy cập trạng thái giữ joystick và hướng tấn công từ bên ngoài
    public bool isHolding;
    public Vector2 aimDirection;

    public bool IsHolding => isHolding;
    public Vector2 AimDirection => aimDirection;

    private void Update()
    {
        if (!isHolding || weaponAtk == null)
            return;

        if (aimDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            weaponTransform.rotation = Quaternion.Euler(0, 0, angle);
        }
        //Debug.Log($"Aim Direction: {aimDirection}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 direction = localPoint / (joystickBackground.sizeDelta / 2f);
        direction = Vector2.ClampMagnitude(direction, 1f);
        aimDirection = direction;

        joystickHandle.anchoredPosition = direction * (joystickBackground.sizeDelta.x / 2f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHolding = true;
        OnDrag(eventData);
        if (weaponAtk != null)
            weaponAtk.Attack();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        aimDirection = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;

        if (weaponAtk != null)
            weaponAtk.AttackEnd();
    }
}
