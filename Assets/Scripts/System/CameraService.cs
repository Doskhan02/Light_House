using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraService : MonoBehaviour
{
    private Camera mainCamera;
    private float maxOffset;
    private Vector3 startPos;
    void Start()
    {
        mainCamera = Camera.main;
        startPos = mainCamera.transform.position;
    }

    void LateUpdate()
    {
        if (GameManager.Instance.IsGameActive)
        {
            Vector2 mousePosition = GameManager.Instance.InputManager.TouchPosition();
            if (mousePosition.x > Screen.width * 0.8f && mainCamera.gameObject.transform.position.x < 2)
            {
                Vector3 offsetPos = mainCamera.gameObject.transform.position + new Vector3(2,0,0);
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, offsetPos, 0.01f);
            }
            else if (mousePosition.x < Screen.width * 0.2f && mainCamera.gameObject.transform.position.x > -2)
            {
                Vector3 offsetPos = mainCamera.gameObject.transform.position + new Vector3(-2, 0, 0);
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, offsetPos, 0.01f);
            }
            else
            {
                mainCamera.gameObject.transform.position = Vector3.Lerp(mainCamera.gameObject.transform.position, startPos, 0.01f);
            }
        }
    }
}
