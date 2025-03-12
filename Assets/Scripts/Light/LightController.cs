using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class LightController : MonoBehaviour
{
    [SerializeField] private LightData data;
    [SerializeField] private GameObject target;

    public LightData LightData => data;

    [SerializeField] private Light spotLight;
    [SerializeField] private LayerMask mask;

    private UpgradeManager upgradeManager;
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    private Vector3 offset;

    void Start()
    {
        upgradeManager = GameManager.Instance.UpgradeManager;
        spotLight.gameObject.SetActive(false);
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = transform.position - mainCamera.transform.position;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        spotLight.gameObject.SetActive(true);
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

                spotLight.range = distance * 1.25f;
                spotLight.spotAngle = 2 * (Mathf.Atan(upgradeManager.Radius / spotLight.range) * Mathf.Rad2Deg);
                spotLight.intensity = spotLight.range;
                if (distance < 45)
                {
                    spotLight.intensity = 0; spotLight.range = 0;
                }
            }

            Vector3 direction = hit.point - transform.position;
            spotLight.transform.rotation = Quaternion.LookRotation(direction);

        }
    }
}

