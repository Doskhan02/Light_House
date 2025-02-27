using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

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
    }

    private void OnDisable()
    {
        playerAction.Disable();
    }

    public Vector2 TouchPosition()
    {
        return playerAction.TouchScreen.TouchPosition.ReadValue<Vector2>();
    }
}
