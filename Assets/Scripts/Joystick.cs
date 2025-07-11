using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    [Header("Joystick Settings")]
    [SerializeField] private Image joystickBackground; // Outer circle background
    [SerializeField] private Image joystickHandle;     // Inner handle
    public RectTransform target;
    public RectTransform canvasRect;
    public CanvasScaler canvasScaler;
    public float speed = 50f;
    public float joystickRadius = 100f;

    private Vector2 direction;
    private Vector2 startTouchPos;
    private bool isJoystickActive = false;

    void Start()
    {
        SetJoystickVisible(false);
    }

    void Update()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
        MoveTarget();
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private void HandleMouseInput()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShowJoystick(Mouse.current.position.ReadValue());
        }
        else if (Mouse.current.leftButton.isPressed && isJoystickActive)
        {
            UpdateJoystick(Mouse.current.position.ReadValue());
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HideJoystick();
        }
    }
#endif

    private void HandleTouchInput()
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
    }

    private void MoveTarget()
    {
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

        // Position both background and handle
        joystickBackground.rectTransform.anchoredPosition = startTouchPos;
        joystickHandle.rectTransform.anchoredPosition = startTouchPos;
        SetJoystickVisible(true);
        isJoystickActive = true;
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

        joystickHandle.rectTransform.anchoredPosition = startTouchPos + clamped;
        direction = clamped / joystickRadius;
    }

    private void HideJoystick()
    {
        direction = Vector2.zero;
        SetJoystickVisible(false);
        isJoystickActive = false;
    }

    private void SetJoystickVisible(bool visible)
    {
        float alpha = visible ? 1f : 0f;
        Color bgColor = joystickBackground.color;
        bgColor.a = alpha;
        joystickBackground.color = bgColor;

        Color handleColor = joystickHandle.color;
        handleColor.a = alpha;
        joystickHandle.color = handleColor;
    }

    // Public method to get joystick direction for InputManager
    public Vector2 GetDirection()
    {
        return direction;
    }
}