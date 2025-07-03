using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform joystickHandle;
    public RectTransform joystickBackground;
    [HideInInspector]
    public float moveSpeed;

    public Rigidbody2D playerRigidbody;

    private TestPlayerMove playerScript;

    private void Start()
    {
        joystickHandle.anchoredPosition = Vector2.zero;

        if (playerRigidbody != null)
        {
            playerScript = playerRigidbody.GetComponent<TestPlayerMove>();
            moveSpeed = playerScript.moveSpeed;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        Vector2 direction = position / (joystickBackground.sizeDelta / 2);
        direction = Vector2.ClampMagnitude(direction, 1);

        joystickHandle.anchoredPosition = direction * (joystickBackground.sizeDelta.x / 2);

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = direction * moveSpeed; // Updated to use linearVelocity
        }

        if (playerScript != null)
        {
            playerScript.SetMoveInput(direction);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;

        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero; // Updated to use linearVelocity
        }

        if (playerScript != null)
        {
            playerScript.SetMoveInput(Vector2.zero);
        }
    }
}
