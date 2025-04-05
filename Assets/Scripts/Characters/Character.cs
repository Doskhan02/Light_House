using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private CharacterType characterType;
    public virtual Character Target { get; }
    public CharacterData CharacterData => characterData;

    public IMovementComponent movementComponent;
    public ILifeComponent lifeComponent;
    public IAIComponent aiComponent;
    public IEffectComponent effectComponent;
    public CharacterType CharacterType => characterType;

    public virtual void Initialize()
    {
        movementComponent = new MovementComponent();
        lifeComponent = new LifeComponent();
        movementComponent.Initialize(CharacterData);
    }
    public abstract void Update();
}
