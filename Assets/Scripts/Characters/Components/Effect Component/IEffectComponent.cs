using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEffectComponent : ICharacterComponent
{
    void CheckForEffectsInLight();
    void AddEffect(Effect effect, int stacks = 1);
    void RemoveEffect(Effect effect);
    void RemoveAllEffects();
    bool HasEffect(Effect effect);
    bool HasEffectOfType<T>() where T : Effect;
    List<ActiveEffect> GetActiveEffects();
}
