using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    [SerializeField] private Character wormPrefab;

    private readonly Dictionary<CharacterType, Queue<Character>> disabledCharacters
       = new();

    private readonly List<Character> activeEnemies = new();
    public List<Character> ActiveEnemies => activeEnemies;
    public Character GetCharacter(CharacterType type)
    {
        Character character = null;
        if (disabledCharacters.ContainsKey(type))
        {
            if (disabledCharacters[type].Count > 0)
            {
                character = disabledCharacters[type].Dequeue();
            }
        }
        else
        {
            disabledCharacters.Add(type, new Queue<Character>());
        }

        if (character == null)
        {
            character = InstantiateCharacter(type);
        }

        activeEnemies.Add(character);
        return character;
    }
    private Character InstantiateCharacter(CharacterType type)
    {
        Character character = null;
        switch (type)
        {
            case CharacterType.Enemy:
                character = GameObject.Instantiate(wormPrefab, new Vector3(Random.Range(-30, 30), -0.4f, 30), Quaternion.LookRotation(Vector3.back));
                break;

            default:
                Debug.LogError("Unknown character type: " + type);
                break;
        }
        return character;

    }
    public void ReturnCharacter(Character character)
    {
        Queue<Character> characters = disabledCharacters[character.CharacterType];
        characters.Enqueue(character);

        activeEnemies.Remove(character);
    }
}
