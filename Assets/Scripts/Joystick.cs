using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Joystick : MonoBehaviour
{
    private InputManager inputManager;

    [SerializeField] private GameObject target;
    [SerializeField] private CanvasScaler canvasScaler;

    private float speed;

    public Vector2 direction;
    void Start()
    {
        inputManager = GameManager.Instance.InputManager;
        speed = 250;
    }

    void Update()
    {
        float scaleFactor = canvasScaler.referenceResolution.x / (float)Screen.width;
        float adjustedSpeed = speed / scaleFactor;

        direction = inputManager.Joystick().normalized;
        target.transform.Translate(direction * adjustedSpeed * Time.deltaTime);
    }
}
