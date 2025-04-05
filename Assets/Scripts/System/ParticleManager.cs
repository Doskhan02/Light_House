using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitParticleEffect;
    [SerializeField] private ParticleSystem dotParticleEffect;

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
        ParticleSystem effect = Instantiate(dotParticleEffect, parent.position, Quaternion.identity, parent);
        effect.Play();
        Destroy(effect.gameObject, effect.main.duration);
    }
}
