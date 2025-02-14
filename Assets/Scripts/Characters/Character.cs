using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;

    public CharacterData CharacterData => characterData;

    public IMovementComponent movementComponent;
    public void Initialize(Character selfCharacter)
    {
        throw new System.NotImplementedException();
    }

    public virtual void Initialize()
    {
        movementComponent = new MovementComponent();
        movementComponent.Initialize(CharacterData);
    }
    public abstract void Update();
}
