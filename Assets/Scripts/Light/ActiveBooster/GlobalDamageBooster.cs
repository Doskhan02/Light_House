using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GlobalDamageBooster", menuName = "Scriptable Objects/Active Booster/Global Damage Booster", order = 1)]
public class GlobalDamageBooster : ActiveBooster
{
    public float damage;
    public float totalFadeDuration;
    public VolumeProfile globalVolumeProfile;
    [HideInInspector] Volume volume;

    public override void ApplyBooster(List<Character> targets)
    {
        GameObject go = new GameObject("GlobalDamageBooster");
        volume = go.AddComponent<Volume>();
        volume.profile = globalVolumeProfile;
        ActiveBoosterManager.Instance.StartCoroutine(Effect());
        foreach (Character target in targets)
        {
            target.lifeComponent.SetDamage(damage);
        }
    }
    private IEnumerator Effect()
    {
        float fadeDuration = totalFadeDuration * 0.5f;
        float holdDuration = totalFadeDuration - (fadeDuration * 2f);

        // Fade In
        volume.weight = 0f;
        while (volume.weight < 1f)
        {
            volume.weight += Time.deltaTime / fadeDuration;
            yield return null;
        }
        volume.weight = 1f;

        // Hold
        yield return new WaitForSeconds(holdDuration);

        // Fade Out
        while (volume.weight > 0f)
        {
            volume.weight -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        volume.weight = 0f;

        GameObject.Destroy(volume.gameObject);
    }


}
