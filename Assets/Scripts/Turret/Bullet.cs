using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;
    public float damage = 1;
    public LayerMask collisionMask;

    private float elapsedTime = 0f;

    void Start()
    {
        elapsedTime = lifetime;
    }

    void Update()
    {
        elapsedTime -= Time.deltaTime;

        if (elapsedTime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        // Move forward
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);

        // Check for collisions using raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position - transform.forward * speed * Time.deltaTime,
                            transform.forward,
                            out hit,
                            speed * Time.deltaTime,
                            collisionMask))
        {
            OnHit(hit);
        }
    }

    void OnHit(RaycastHit hit)
    {
        Debug.Log("Hit: " + hit.collider.gameObject.name);

        if(hit.collider.gameObject.TryGetComponent<Character>(out Character character))
        {
            character.lifeComponent.SetDamage(damage); // Apply damage
        }

        // Optional: Do damage, instantiate explosion, etc.

        Destroy(gameObject);
    }
}