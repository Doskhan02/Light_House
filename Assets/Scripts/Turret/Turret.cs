using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float spreadFactor;

    private float currentAmmo;
    public float CurrentAmmo => currentAmmo;
    
    public bool isShooting = false;
    private float elapsedTime = 0f;
    private Vector3 targetPos;
    
    private Character Target {
        get
        {
            Character target = null;
            float minDistance = 500;
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy);
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].gameObject.activeSelf)
                    continue;
                
                float distanceBetween = Vector3.Distance(list[i].transform.position, transform.position);
                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = list[i];
                }

            }
            return target;
        }
    }

    public void Initialize(float initAmmo)
    {
        currentAmmo = initAmmo;    
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
        if (transform.localEulerAngles.y > minRotation &&
            transform.localEulerAngles.y < maxRotation)
        {
            isShooting = false;
            transform.localRotation = 
                Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
        }
        else
        {
            if(Target == null)
            {
                isShooting = false;
                transform.localRotation = 
                    Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
                return;
            }
            Vector3 direction = Target.transform.position - transform.position;
            targetPos = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(targetPos - transform.position, Vector3.up), 
                Time.deltaTime * rotationSpeed);
            isShooting = Quaternion.Angle(transform.rotation, 
                Quaternion.LookRotation(targetPos - transform.position, Vector3.up)) < 1f;
            firePoint.rotation = Quaternion.LookRotation(direction, transform.up);
        }
        
        if (!isShooting)
            return;
        if (elapsedTime < 0 && currentAmmo > 0)
        {
            // Random angles for spread
            float xSpread = Random.Range(-spreadFactor, spreadFactor);
            float ySpread = Random.Range(-spreadFactor, spreadFactor);
            float zSpread = Random.Range(-spreadFactor, spreadFactor);

            // Create a random spread rotation
            Quaternion spreadRotation = Quaternion.Euler(xSpread, ySpread, zSpread);

            // Apply spread to the firePoint's rotation
            Quaternion finalRotation = firePoint.rotation * spreadRotation;

            // Instantiate bullet with the final rotation
            GameObject.Instantiate(bullet, firePoint.position, finalRotation);
            currentAmmo--;

            // Reset fire rate timer
            elapsedTime = fireRate;
        }

        elapsedTime -= Time.deltaTime;
    }

    public void Reload(float ammo)
    {
        currentAmmo += ammo;
    }
}
