using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;
    [SerializeField] private float rotationSpeed;
    
    private float elapsedTime = 0f;
    private Vector3 targetPos;
    
    private Character Target {
        get
        {
            Character target = null;
            target = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy,"DT(BOSS)").First();
            return target;
        }
    }

    private void Start()
    {
        if(firePoint == null)
        {
            firePoint = transform;
        }
        if (bullet == null)
        {
            Debug.LogError("Bullet prefab is not assigned in the Turret script.");
        }
    }
    
    private void Update()
    {
        if (transform.localEulerAngles.y < minRotation &&
            transform.localEulerAngles.y > maxRotation)
        {
            transform.localRotation = 
                Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
        }
        else
        {
            if(Target == null)
            {
                transform.localRotation = 
                    Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
                return;
            }
            Vector3 direction = Target.transform.position - transform.position;
            targetPos = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(targetPos - transform.position, Vector3.up), 
                Time.deltaTime * rotationSpeed);
            firePoint.rotation = Quaternion.LookRotation(direction, transform.up);
        }
    }

    public void Fire()
    {
        GameObject.Instantiate(bullet, firePoint.position, firePoint.rotation);
    }
}
