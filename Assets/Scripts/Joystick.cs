using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    public RectTransform target;
    public RectTransform canvasRect; 
    public CanvasScaler canvasScaler;
    public float speed = 50f;

    private Vector2 direction;
    private InputManager inputManager;

    void Start()
    {
        inputManager = GameManager.Instance.InputManager;
    }

    void Update()
    {
        float scaleFactor = canvasScaler.scaleFactor;
        float adjustedSpeed = speed / scaleFactor;

        direction = inputManager.Joystick().normalized;
        Vector3 newPosition = target.anchoredPosition + (direction * adjustedSpeed * Time.deltaTime);

        float halfWidth = canvasRect.rect.width / 2;
        float halfHeight = canvasRect.rect.height / 2;

        float targetHalfWidth = target.rect.width / 2;
        float targetHalfHeight = target.rect.height / 2;

        newPosition.x = Mathf.Clamp(newPosition.x, -halfWidth + targetHalfWidth, halfWidth - targetHalfWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, -halfHeight + targetHalfHeight, halfHeight - targetHalfHeight);

        target.anchoredPosition = newPosition;
    }
}
