using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light spotLight;   // ������ �� �������� ����� (������ � ����������� Light)
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
        offset = transform.position - mainCamera.transform.position;
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

            if (mousePosition.y < -37)                                          // ������� ����������� ����� ����� ����� ���������
            {
                mousePosition = new Vector3 (mousePosition.x, -37,0);
            }
                
            Vector3 direction = mousePosition - (transform.position + offset);  
            spotLight.transform.rotation = Quaternion.LookRotation(direction);  
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out hit, Mathf.Infinity))    //���� ���� ��� ������������ ���� ��������� ����� � ����������� �� ���������                                    
            {
                float distance = hit.distance;
                
                spotLight.range = distance + 10;    //����� 10 ����� ������ ���� ���� ����� ����� ���� �����
                spotLight.spotAngle = 2 * (Mathf.Atan((radius/spotLight.range)) * Mathf.Rad2Deg);
                spotLight.intensity = spotLight.range / 10; //��� ���� ����� �������� 10�� ���� ������������� �������
                if(distance< 50)
                {
                    spotLight.intensity =0; spotLight.range = 0;
                }                                           //��� ������ ����� ��� ������ �������������
            }
                
        }
                       
    }
}
