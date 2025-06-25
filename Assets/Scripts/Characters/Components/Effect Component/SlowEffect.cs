using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SlowEffect", menuName = "Scriptable Objects/Effects/Slow")]
public class SlowEffect : Effect
{
    [Header("Damage Properties")]
    [Tooltip("Base damage per second")]
    public float slowAmount = 5f;

    [Tooltip("Should damage scale with stacks?")]
    public bool scaleWithStacks = true;
}
