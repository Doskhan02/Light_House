using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private Light spotLight;   // Ссылка на источник света (объект с компонентом Light)
    private Ray ray;
    private Camera mainCamera;
    public RaycastHit hit;
    public float radius = 100;
    public LayerMask mask;
    private Vector3 offset;

    void Start()
    {
        LayerMask mask = LayerMask.GetMask("LightTarget");
        mainCamera = Camera.main;
        ray = new Ray(transform.position, transform.forward);
        offset = transform.position - mainCamera.transform.position;
        Physics.Raycast(ray, out hit, Mathf.Infinity);
    }

    public void Initialize()
    {
        spotLight.gameObject.SetActive(true);   //Метод для запуска в менеджере
    }

    void Update()
    {
        if (GameManager.Instance.IsGameActive)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 100f;                                         
            mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);       //Меняем координаты с экранных коор в мировые

            if (mousePosition.y < -38)                                          // Создаем ограничения цифра взята через дебаггинг
            {
                mousePosition.y = -38;
                mousePosition.z = 0;
            }
                
            Vector3 direction = mousePosition - (transform.position + offset);  
            spotLight.transform.rotation = Quaternion.LookRotation(direction);  
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray,out hit, Mathf.Infinity))    //Этот блок для высчитывания угла источника света в зависимости от дальности                                    
            {
                float distance = hit.distance;
                spotLight.range = distance + 10;    //цифру 10 можно менять если края диска света хуже видны
                spotLight.spotAngle = 2 * (Mathf.Atan((radius/spotLight.range)) * Mathf.Rad2Deg);
                spotLight.intensity = spotLight.range / 10; //тут тоже можно поменять 10ку если интенсивность тусклая
                                                            //чем меньше цифра тем больше интенсивность
            }
                
        }
                       
    }
}
