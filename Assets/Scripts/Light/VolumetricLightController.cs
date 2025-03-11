using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricLightController : MonoBehaviour
{
    [SerializeField] private VLight spotLight;   
    [SerializeField] private LayerMask mask;

    private LightData lightData;
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    public float radius = 100;
    private Vector3 offset;

    void Start()
    {
        spotLight.gameObject.SetActive(false);
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = (transform.position - mainCamera.transform.position);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        spotLight.gameObject.SetActive(true);   
        lightData = GameManager.Instance.LightController.LightData;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            Vector3 mousePosition = GameManager.Instance.InputManager.TouchPosition();
            mousePosition.z = 100f;                                         
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);       

            ray = mainCamera.ScreenPointToRay(GameManager.Instance.InputManager.TouchPosition());
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))    
            {
                float distance = hit.distance - offset.magnitude;

                spotLight.spotRange = distance * 1.25f;    
                spotLight.spotAngle = 2 * (Mathf.Atan((lightData.baseRadius / spotLight.spotRange)) * Mathf.Rad2Deg);
            }

            Vector3 direction = hit.point - transform.position;  
            spotLight.transform.rotation = Quaternion.LookRotation(direction);  
            
        }            
    }
}
