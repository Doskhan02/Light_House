using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInputActions playerAction;
    private Joystick uiJoystick; // Reference to UI joystick

    private void Awake()
    {
        playerAction = new PlayerInputActions();
        // Find the UI joystick in the scene
        uiJoystick = FindObjectOfType<Joystick>();
    }
    
    private void OnEnable()
    {
        playerAction.Enable();
        
        // Subscribe to quit action based on platform
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // For Windows, use keyboard input (like Escape key)
        if (playerAction.TouchScreen.QuitApplication != null)
            playerAction.TouchScreen.QuitApplication.performed += OnQuitPerformed;
#elif UNITY_ANDROID || UNITY_IOS
        // For mobile platforms, use the original touch screen quit
        playerAction.TouchScreen.QuitApplication.performed += OnQuitPerformed;
#else
        // Fallback for other platforms
        if (playerAction.TouchScreen?.QuitApplication != null)
            playerAction.TouchScreen.QuitApplication.performed += OnQuitPerformed;
#endif
    }

    private void OnDisable()
    {
        playerAction.Disable();
        
        // Unsubscribe from quit action based on platform
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (playerAction.TouchScreen.QuitApplication != null)
            playerAction.TouchScreen.QuitApplication.performed -= OnQuitPerformed;
#elif UNITY_ANDROID || UNITY_IOS
        playerAction.TouchScreen.QuitApplication.performed -= OnQuitPerformed;
#else
        if (playerAction.TouchScreen?.QuitApplication != null)
            playerAction.TouchScreen.QuitApplication.performed -= OnQuitPerformed;
#endif
    }

    public Vector2 TouchPosition()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // On Windows, return mouse position instead of touch position
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        // On mobile platforms, return actual touch position
        return playerAction.TouchScreen.TouchPosition.ReadValue<Vector2>();
#endif
    }

    public bool TouchScreen()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // On Windows, use mouse click instead of touch
        return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
        // On mobile platforms, use actual touch input
        return playerAction.TouchScreen.Touch.triggered;
#endif
    }

    public Vector2 Joystick()
    {
        // If UI joystick exists, use its direction
        if (uiJoystick != null)
        {
            return uiJoystick.GetDirection();
        }
        
        // Fallback to Input Actions if no UI joystick found
        return playerAction.TouchScreen.Move.ReadValue<Vector2>();
    }
    
    private void OnQuitPerformed(InputAction.CallbackContext context)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        // On Windows, you might want to add a confirmation dialog
        Application.Quit();
#elif UNITY_WEBGL
        // WebGL can't quit, so do nothing or show a message
        Debug.Log("Cannot quit in WebGL build");
#else
        // On mobile platforms, quit normally
        Application.Quit();
#endif
    }
}