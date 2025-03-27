using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffect", menuName = "Scriptable Objects/Effects/Base Effect")]
public abstract class Effect : ScriptableObject
{

    [Tooltip("Display name shown in UI")]
    public string displayName;

    [TextArea(2, 5)]
    [Tooltip("Description of what this effect does")]
    public string description;

    [Tooltip("Icon representing this effect in UI")]
    public Sprite icon;

    [Header("Effect Properties")]
    [Tooltip("Can this effect stack with itself?")]
    public bool canStack = false;

    [Tooltip("Maximum stack count if stacking is enabled")]
    public int maxStacks = 1;

    [Tooltip("Duration of effect in seconds (-1 for permanent until removed)")]
    public float duration = 5f;

    [Header("Visual Feedback")]
    [Tooltip("Color to tint the character when affected")]
    public Color effectColor = Color.white;

    [Tooltip("Particle effect prefab to spawn on the affected character")]
    public GameObject effectParticlePrefab;
}
