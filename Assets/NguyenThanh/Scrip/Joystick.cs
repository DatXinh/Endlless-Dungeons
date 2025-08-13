using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    public Vector2 inputDirection;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos);
        pos = pos / background.sizeDelta * 2;

        inputDirection = Vector2.ClampMagnitude(pos, 1.0f);
        handle.anchoredPosition = inputDirection * (background.sizeDelta / 2);
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);
    public void OnPointerUp(PointerEventData eventData)
    {
        inputDirection = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    public float Horizontal => inputDirection.x;
    public float Vertical => inputDirection.y;
}
