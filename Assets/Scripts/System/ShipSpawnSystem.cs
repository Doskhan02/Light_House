using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipSpawnSystem : MonoBehaviour
{
    [SerializeField] private Character shipPrefab;

    private Dictionary<CharacterType, Queue<Character>> disabledCharacters
       = new Dictionary<CharacterType, Queue<Character>>();

    private List<Character> activeCharacters = new List<Character>();
    public List<Character> ActiveCharacters => activeCharacters;
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

        activeCharacters.Add(character);
        return character;
    }
    private Character InstantiateCharacter(CharacterType type)
    {
        Character character = null;
        switch (type)
        {
            case CharacterType.Ally:
                character = GameObject.Instantiate(shipPrefab, new Vector3(0, 0.5f,50), Quaternion.LookRotation(Vector3.back));
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
        Debug.Log(characters.Count + "" + characters.Peek());
        activeCharacters.Remove(character);
    }
}
