using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerAction;

    private void Awake()
    {
        playerAction = new PlayerInputActions();
    }
    private void OnEnable()
    {
        playerAction.Enable();
        playerAction.TouchScreen.QuitApplication.performed += OnQuitPerformed;
    }

    private void OnDisable()
    {
        playerAction.Disable();
        playerAction.TouchScreen.QuitApplication.performed -= OnQuitPerformed;
    }

    public Vector2 TouchPosition()
    {
        return playerAction.TouchScreen.TouchPosition.ReadValue<Vector2>();
    }

    public Vector2 Joystick()
    {
        return playerAction.TouchScreen.Move.ReadValue<Vector2>();
    }
    private void OnQuitPerformed(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
}
