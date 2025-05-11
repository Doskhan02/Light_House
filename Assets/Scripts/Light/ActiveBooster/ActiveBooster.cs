using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActiveBooster : ScriptableObject
{
    [Header("Active booster parameter:")]
    public BoosterType boosterType; // Type of the booster
    public float cooldown = 5f; // Cooldown time in seconds
    public Sprite icon; // Icon for the booster
    public virtual void ApplyBooster() { }
    public virtual void ApplyBooster(List<Character> targets) { }
}

public enum BoosterType
{
    None,
    Global,
    TempStatIncrease,
}
