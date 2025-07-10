using UnityEngine;
using UnityEngine.VFX;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticleEffect;
    [SerializeField] private ParticleSystem deadParticleEffect2;
    [SerializeField] private GameObject DOT_visualEffect;
    [SerializeField] private GameObject SlowVisualEffect;

    public static ParticleManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHitParticleEffect(Transform parent)
    {
        ParticleSystem effect = Instantiate(hitParticleEffect, parent.position, Quaternion.identity, parent);
        
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
    }
    public void PlayDOTParticleEffect(Transform parent)
    {
        GameObject effect = Instantiate(DOT_visualEffect, parent.position, Quaternion.identity, parent);
        effect.GetComponentInChildren<VisualEffect>().Play();
        Destroy(effect.gameObject, 1f);
    }

    public void PlaySlowParticleEffect(Transform parent)
    {
        GameObject effect = Instantiate(SlowVisualEffect, parent.position, Quaternion.identity, parent);
        effect.GetComponentInChildren<VisualEffect>().Play();
        Destroy(effect.gameObject, 1f);
    }

    public void PlayDeadParticleEffect(Transform position)
    {
        ParticleSystem effect = Instantiate(deadParticleEffect2, position.position, Quaternion.identity);
        effect.Play();
        Destroy(effect.gameObject, 1f);
    }
}
