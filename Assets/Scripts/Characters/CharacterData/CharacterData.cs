using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private CharacterTypeData characterTypeData;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private LayerMask characterMask;
    [SerializeField] private ProgressBar healthbar;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject characterModel;

    public Character Character => character;
    public CharacterType CharacterType => characterType;
    public CharacterTypeData CharacterTypeData => characterTypeData;
    public Transform CharacterTransform => characterTransform;
    public CharacterController CharacterController => characterController;
    public LayerMask CharacterMask => characterMask;
    public ProgressBar Healthbar => healthbar;
    public Animator Animator => animator;
    public GameObject CharacterModel => characterModel;
}
