using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light spotLight;   // ������ �� �������� ����� (������ � ����������� Light)
    [SerializeField] private LayerMask mask;
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
            Vector3 mousePosition = GameManager.Instance.InputManager.TouchPosition();
            mousePosition.z = 100f;                                         
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);       //������ ���������� � �������� ���� � �������
            Debug.Log(mousePosition);
            if (mousePosition.y < -37)                                          // ������� ����������� ����� ����� ����� ���������
            {
                mousePosition = new Vector3 (mousePosition.x, -37,mainCamera.nearClipPlane);
            }
                
            Vector3 direction = mousePosition - (transform.position + offset);  
            spotLight.transform.rotation = Quaternion.LookRotation(direction);  
            ray = mainCamera.ScreenPointToRay(GameManager.Instance.InputManager.TouchPosition());
            Debug.DrawLine(ray.origin, hit.point, Color.red);
            if (Physics.Raycast(ray,out hit, Mathf.Infinity,mask.value))    //���� ���� ��� ������������ ���� ��������� ����� � ����������� �� ���������                                    
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                float distance = hit.distance;
                
                spotLight.range = distance + 10;    //����� 10 ����� ������ ���� ���� ����� ����� ���� �����
                spotLight.spotAngle = 2 * (Mathf.Atan((radius/spotLight.range)) * Mathf.Rad2Deg);
                spotLight.intensity = spotLight.range / 5; //��� ���� ����� �������� 10�� ���� ������������� �������
                if(distance< 50)
                {
                    spotLight.intensity =0; spotLight.range = 0;
                }                                           
            }
                
        }
                       
    }
}
