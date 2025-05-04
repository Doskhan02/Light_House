using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawnSystem : MonoBehaviour
{
    public static CharacterSpawnSystem Instance { get; private set; }

    [SerializeField] private CharacterFactory characterFactory;

    public CharacterFactory CharacterFactory => characterFactory;
    public void Awake()
    {
        Instance = this;
    }

    public void Initialize(int currentLevel)
    {
        GameData data = GameManager.Instance.gameData;
        SetSpawnAmount(data.wormSpawnAmount, CharacterType.Enemy, "Worm");
        SetSpawnAmount(data.deepGuardianSpawnAmount, CharacterType.Enemy, "DeepGuardian");
        SetSpawnAmount(data.deepGuardianSlimeSpawnAmount, CharacterType.Enemy, "DGSlime(parent)");
    }

    public void SetSpawnAmount(int amount, CharacterType characterType, string subType)
    {
        characterFactory.GetConfig(characterType, subType).maxActive = amount;
    }

    public void SpawnCharacter(CharacterType characterType, string subType, Vector3 position)
    {
        Character character = characterFactory.GetCharacter(characterType, subType);
        if (character == null)
            return;

        character.transform.position = position;

        character.gameObject.SetActive(true);
        character.Initialize();
        if (characterType == CharacterType.Enemy || characterType == CharacterType.EnemyMinion)
        {
            character.effectComponent.Initialize(character);
            character.aiComponent.Initialize(character);
        }
        character.lifeComponent.Initialize(character);
        character.lifeComponent.OnCharacterDeath += CharacterDeathHandler;
    }
    public void SpawnCharacter(CharacterType characterType)
    {
        Character character = characterFactory.GetCharacter(characterType);
        if (character == null)
            return;
        if (characterType == CharacterType.Enemy)
        {
            character.transform.position = new Vector3(Random.Range(-30, 30), -1.0f, Random.Range(40, 50));
        }
        else if (characterType == CharacterType.Ally)
        {
            character.transform.position = new Vector3(Random.Range(-30, 30), 0f, 100);
            SpawnCharacter(CharacterType.Enemy, "GhostShip", new Vector3(Random.Range(-30, 30), 0f, 100));
        }
        character.gameObject.SetActive(true);
        character.Initialize();
        if (characterType == CharacterType.Enemy)
        {
            character.effectComponent.Initialize(character);
            character.aiComponent.Initialize(character);
        }
        character.lifeComponent.Initialize(character);
        character.lifeComponent.OnCharacterDeath += CharacterDeathHandler;
    }
    public void CharacterDeathHandler(Character deathCharacter)
    {
        deathCharacter.StopAllCoroutines();
        deathCharacter.lifeComponent.OnCharacterDeath -= CharacterDeathHandler;
        switch (deathCharacter.CharacterData.CharacterType)
        {
            case CharacterType.Ally:
                deathCharacter.gameObject.SetActive(false);
                characterFactory.ReturnCharacter(deathCharacter);
                break;
            case CharacterType.Enemy:
                deathCharacter.gameObject.SetActive(false);
                characterFactory.ReturnCharacter(deathCharacter);
                break;
            case CharacterType.EnemyMinion:
                deathCharacter.gameObject.SetActive(false);
                characterFactory.ReturnCharacter(deathCharacter);
                break;
        }
    }
    public void CharacterWipe()
    {
        StartCoroutine(DisableAllCharacters());
    }
    private IEnumerator DisableAllCharacters()
    {
        List<Character> allCharacter = characterFactory.GetAllActiveCharacters();

        foreach (Character character in allCharacter)
        {
            CharacterDeathHandler(character);
            yield return null;
        }
    }
}
