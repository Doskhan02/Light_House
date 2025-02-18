using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricLightController : MonoBehaviour
{
    [SerializeField] private VLight spotLight;   // ������ �� �������� ����� (������ � ����������� Light)
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    public float radius = 100;
    public LayerMask mask;
    private Vector3 offset;

    void Start()
    {
        spotLight.gameObject.SetActive(false);
        LayerMask mask = LayerMask.GetMask("LightTarget");
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = (transform.position - mainCamera.transform.position)/32;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        spotLight.gameObject.SetActive(true);   //����� ��� ������� � ���������
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100f;                                         
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);       //������ ���������� � �������� ���� � �������


                
            Vector3 direction = mousePosition - (transform.position + offset);  
            spotLight.transform.rotation = Quaternion.LookRotation(direction);  
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out hit, Mathf.Infinity))    //���� ���� ��� ������������ ���� ��������� ����� � ����������� �� ���������                                    
            {
                float distance = hit.distance;
                
                spotLight.spotRange = distance + 10;    //����� 10 ����� ������ ���� ���� ����� ����� ���� �����
                spotLight.spotAngle = 2 * (Mathf.Atan((radius/spotLight.spotRange)) * Mathf.Rad2Deg);
                spotLight.slices = (int)spotLight.spotRange * 10;
            }
                
        }
                       
    }
}
