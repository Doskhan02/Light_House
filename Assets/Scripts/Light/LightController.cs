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
    [SerializeField] private GameObject lightGeo;

    public LightData LightData => data;

    [SerializeField] private LayerMask mask;

    private UpgradeManager upgradeManager;
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    private Vector3 offset;

    void Start()
    {
        upgradeManager = GameManager.Instance.UpgradeManager;
        lightGeo.SetActive(false);
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = lightGeo.transform.position - mainCamera.transform.position;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        lightGeo.SetActive(true);
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            ray = mainCamera.ScreenPointToRay(target.transform.position);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask.value))
            {
                Debug.DrawLine(ray.origin, hit.point);
                float distance = hit.distance - offset.magnitude;

                lightGeo.transform.rotation = Quaternion.LookRotation(hit.point - lightGeo.transform.position);
                lightGeo.transform.localScale = new Vector3(upgradeManager.Radius, upgradeManager.Radius, distance * 0.6f);

            }

            Vector3 direction = hit.point - transform.position;

        }
    }
}

