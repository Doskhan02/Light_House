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

    public void SpawnCharacter(CharacterType characterType, string subType)
    {
        Character character = characterFactory.GetCharacter(characterType, subType);
        if (character == null)
            return;
        if (characterType == CharacterType.Enemy)
        {
            character.transform.position = new Vector3(Random.Range(-30, 30), -0.4f, Random.Range(40, 50));
        }
        else if (characterType == CharacterType.Ally) 
        {
            character.transform.position = new Vector3(Random.Range(-30, 30), 0.5f, 100);
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
