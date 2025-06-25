using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleHit : MonoBehaviour
{
    [SerializeField]private float hitDelay;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private float pulseSpeed = 8f;
    [SerializeField] private float damage;
    [SerializeField] private Animator animator;
    
    private float elapsedTime = 0;
    private bool isHit = false;

    private RaycastHit lightHit;
    
    // Start is called before the first frame update
    private void Start()
    {
        elapsedTime = hitDelay;
    }

    // Update is called once per frame
    private void Update()
    {
        lightHit = GameManager.Instance.LightController.hit;
        if (Vector3.Distance(lightHit.point, transform.position) < GameManager.Instance.UpgradeManager.Radius)
        {
            Destroy(gameObject);
        }
        if (elapsedTime > 0)
        {
            
            float fadeAmount = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f; // Oscillates between 0 and 1

            Color baseColor = Color.red; // or whatever your hit color is
            sprite.color = Color.Lerp(Color.clear, baseColor, fadeAmount);

            elapsedTime -= Time.deltaTime;
            if (Mathf.Round(elapsedTime) % 4 == 0)
            {
                animator.SetTrigger("Hit");
                Invoke(nameof(Destroy),8f);
            }
        }
        else
        {
            if(isHit)
                return;
            sprite.color = Color.clear;
            foreach (Collider collider in Physics.OverlapSphere(transform.position, 5f))
            {
                var ally = collider.gameObject.GetComponentInParent<AllyCharacter>();
                if (ally != null)
                {
                    ally.lifeComponent.SetDamage(damage);
                }
            }
            isHit = true;
        }
    }

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
