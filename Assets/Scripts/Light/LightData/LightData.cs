using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightData", menuName = "Scriptable Objects/Light Data/Base Light Data")]
public class LightData : ScriptableObject
{
    public float baseRadius;
    public float baseDamage;
    public float baseAttackRate;

    public float shipSpeedUpFactor;
}
