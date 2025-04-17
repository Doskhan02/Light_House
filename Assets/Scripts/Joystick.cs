using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    [Header("Joystick Settings")]
    [SerializeField] private Image joystick; // Only the handle now
    public RectTransform target;
    public RectTransform canvasRect;
    public CanvasScaler canvasScaler;
    public float speed = 50f;
    public float joystickRadius = 100f;

    private Vector2 direction;
    private Vector2 startTouchPos;

    void Start()
    {
        SetJoystickVisible(false);
    }

    void Update()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch == null)
            return;

        var touch = Touchscreen.current.primaryTouch;

        if (touch.press.wasPressedThisFrame)
        {
            ShowJoystick(touch.position.ReadValue());
        }
        else if (touch.press.isPressed)
        {
            UpdateJoystick(touch.position.ReadValue());
        }
        else if (touch.press.wasReleasedThisFrame)
        {
            HideJoystick();
        }

        // Move the target based on input
        float scaleFactor = canvasScaler.scaleFactor;
        float adjustedSpeed = (speed / scaleFactor) * direction.magnitude;
        Vector3 newPosition = target.anchoredPosition + (direction.normalized * adjustedSpeed * Time.deltaTime);

        float halfWidth = canvasRect.rect.width / 2;
        float halfHeight = canvasRect.rect.height / 3;

        float targetHalfWidth = target.rect.width / 2;
        float targetHalfHeight = target.rect.height / 2;

        newPosition.x = Mathf.Clamp(newPosition.x, -halfWidth + targetHalfWidth, halfWidth - targetHalfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -halfHeight + targetHalfHeight, halfHeight - targetHalfHeight);

        target.anchoredPosition = newPosition;
    }

    private void ShowJoystick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out startTouchPos
        );

        joystick.rectTransform.anchoredPosition = startTouchPos;
        SetJoystickVisible(true);
    }

    private void UpdateJoystick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out Vector2 currentPos
        );

        Vector2 delta = currentPos - startTouchPos;
        Vector2 clamped = Vector2.ClampMagnitude(delta, joystickRadius);
        joystick.rectTransform.anchoredPosition = startTouchPos + clamped;
        direction = clamped / joystickRadius;
    }

    private void HideJoystick()
    {
        direction = Vector2.zero;
        SetJoystickVisible(false);
    }

    private void SetJoystickVisible(bool visible)
    {
        Color color = joystick.color;
        color.a = visible ? 1f : 0f;
        joystick.color = color;
    }
}