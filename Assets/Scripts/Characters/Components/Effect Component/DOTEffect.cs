using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DOT Effect", menuName = "Scriptable Objects/Effects/Damage Over Time")]
public class DOTEffect : Effect
{
    [Header("Damage Properties")]
    [Tooltip("Base damage per second")]
    public float damagePerSecond = 5f;

    [Tooltip("Should damage scale with stacks?")]
    public bool scaleWithStacks = true;

    [Tooltip("Time between damage ticks")]
    public float tickRate = 1f;
}
