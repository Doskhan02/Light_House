using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    [Serializable]
    public class CharacterSubtypeConfig
    {
        [Tooltip("The main character category")]
        public CharacterType mainType;

        [Tooltip("The prefab for this character variant")]
        public Character prefab;

        [Tooltip("Maximum number of this type allowed in the scene (-1 = unlimited)")]
        public int maxActive = -1;

        [Tooltip("How many to create at startup for pooling")]
        public int initialPoolSize = 3;

        [Tooltip("Notes about this character variant")]
        [TextArea(1, 3)]
        public string description;

        // Automatically use the prefab name as the subtype identifier
        public string SubtypeName => prefab != null ? prefab.name : "Unknown";
    }

    [SerializeField] private List<CharacterSubtypeConfig> characterConfigs = new List<CharacterSubtypeConfig>();

    // Main pools - organized by main type and subtype
    private Dictionary<CharacterType, Dictionary<string, Queue<Character>>> pooledCharacters =
        new Dictionary<CharacterType, Dictionary<string, Queue<Character>>>();

    // Active character tracking - organized by main type and subtype
    private Dictionary<CharacterType, Dictionary<string, List<Character>>> activeCharacters =
        new Dictionary<CharacterType, Dictionary<string, List<Character>>>();

    // Tracking for all characters of a specific main type regardless of subtype
    private Dictionary<CharacterType, List<Character>> activeByMainType =
        new Dictionary<CharacterType, List<Character>>();

    // Track all active characters regardless of type
    private List<Character> allActiveCharacters = new List<Character>();

    private void Awake()
    {
        InitializePools();
    }

    private void OnValidate()
    {
        // Show a warning if any prefabs are missing
        foreach (var config in characterConfigs)
        {
            if (config.prefab == null)
            {
                Debug.LogWarning("Character Factory has a config with missing prefab!");
            }
        }
    }

    private void InitializePools()
    {
        foreach (var config in characterConfigs)
        {
            if (config.prefab == null) continue;

            string subtypeName = config.SubtypeName;

            // Initialize the main type dictionaries if needed
            if (!pooledCharacters.ContainsKey(config.mainType))
            {
                pooledCharacters[config.mainType] = new Dictionary<string, Queue<Character>>();
                activeCharacters[config.mainType] = new Dictionary<string, List<Character>>();
                activeByMainType[config.mainType] = new List<Character>();
            }

            // Initialize the subtype collections if needed
            if (!pooledCharacters[config.mainType].ContainsKey(subtypeName))
            {
                pooledCharacters[config.mainType][subtypeName] = new Queue<Character>();
                activeCharacters[config.mainType][subtypeName] = new List<Character>();
            }

            // Pre-populate the pool with initial instances
            for (int i = 0; i < config.initialPoolSize; i++)
            {
                Character instance = CreateNewInstance(config.mainType, subtypeName);
                instance.gameObject.SetActive(false);
                pooledCharacters[config.mainType][subtypeName].Enqueue(instance);
            }
        }
    }

    public Character GetCharacter(CharacterType mainType)
    {
        // Find all subtypes for this main type
        var subtypes = new List<string>();
        foreach (var config in characterConfigs)
        {
            if (config.mainType == mainType && config.prefab != null)
            {
                subtypes.Add(config.SubtypeName);
            }
        }

        if (subtypes.Count == 0)
        {
            Debug.LogError($"No subtypes configured for main type {mainType}");
            return null;
        }

        // Try to get a character from a random subtype that isn't at capacity
        List<string> availableSubtypes = subtypes.FindAll(st => !IsSubtypeAtCapacity(mainType, st));

        if (availableSubtypes.Count == 0)
        {
            Debug.Log($"All subtypes of {mainType} are at capacity");
            return null;
        }

        string selectedSubtype = availableSubtypes[UnityEngine.Random.Range(0, availableSubtypes.Count)];
        return GetCharacter(mainType, selectedSubtype);
    }

    public Character GetCharacter(CharacterType mainType, string subtypeName)
    {
        // Validate the type and subtype are configured
        CharacterSubtypeConfig config = FindConfig(mainType, subtypeName);
        if (config == null)
        {
            Debug.LogError($"Character type {mainType} with subtype '{subtypeName}' not configured in CharacterFactory");
            return null;
        }

        // Check if we've hit the limit for this subtype
        if (config.maxActive >= 0 && activeCharacters[mainType][subtypeName].Count >= config.maxActive)
        {
            Debug.Log($"Cannot spawn more {mainType}/{subtypeName}: reached limit of {config.maxActive}");
            return null;
        }

        // Get or create character
        Character character;

        if (pooledCharacters[mainType][subtypeName].Count > 0)
        {
            character = pooledCharacters[mainType][subtypeName].Dequeue();
        }
        else
        {
            character = CreateNewInstance(mainType, subtypeName);
        }

        // Track as active
        activeCharacters[mainType][subtypeName].Add(character);
        activeByMainType[mainType].Add(character);
        allActiveCharacters.Add(character);

        return character;
    }
    public void ReturnCharacter(Character character, string subtypeName)
    {
        if (character == null) return;

        CharacterType mainType = character.CharacterType;

        if (!activeCharacters.ContainsKey(mainType) ||
            !activeCharacters[mainType].ContainsKey(subtypeName))
        {
            Debug.LogError($"Cannot return character - type/subtype pool not found: {mainType}/{subtypeName}");
            return;
        }

        // Remove from active tracking
        activeCharacters[mainType][subtypeName].Remove(character);
        activeByMainType[mainType].Remove(character);
        allActiveCharacters.Remove(character);

        // Reset character state
        character.gameObject.SetActive(false);

        // Return to pool
        pooledCharacters[mainType][subtypeName].Enqueue(character);
    }

    /// <summary>
    /// Returns a character to the pool by looking up its subtype.
    /// Use the other overload when you already know the subtype for better performance.
    /// </summary>
    public void ReturnCharacter(Character character)
    {
        if (character == null) return;

        CharacterType mainType = character.CharacterType;

        // Find the original prefab name this character was created from
        CharacterIdentifier identifier = character.GetComponent<CharacterIdentifier>();
        string subtypeName = identifier != null ? identifier.SubtypeName : null;

        if (string.IsNullOrEmpty(subtypeName))
        {
            Debug.LogError($"Cannot return character {character.name} - subtype identifier not found");
            return;
        }

        // Use the optimized method
        ReturnCharacter(character, subtypeName);
    }

    private Character CreateNewInstance(CharacterType mainType, string subtypeName)
    {
        CharacterSubtypeConfig config = FindConfig(mainType, subtypeName);
        if (config == null || config.prefab == null)
        {
            Debug.LogError($"Cannot create character - config not found or prefab missing. MainType: {mainType}, SubtypeName: {subtypeName}");
            return null;
        }

        Character instance = Instantiate(config.prefab, Vector3.zero, Quaternion.identity, transform);

        // Add an identifier component to track the subtype
        CharacterIdentifier identifier = instance.GetComponent<CharacterIdentifier>();
        if (identifier == null)
        {
            identifier = instance.gameObject.AddComponent<CharacterIdentifier>();
        }
        identifier.SubtypeName = subtypeName;

        // Generate a unique identifier for this instance
        string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
        instance.name = $"{subtypeName}_{uniqueId}";

        return instance;
    }

    private CharacterSubtypeConfig FindConfig(CharacterType mainType, string subtypeName)
    {
        return characterConfigs.Find(c => c.mainType == mainType && c.SubtypeName == subtypeName);
    }

    public CharacterSubtypeConfig GetConfig(CharacterType mainType, string subtypeName)
    {
        return FindConfig(mainType, subtypeName);
    }

    // Utility methods

    /// <summary>
    /// Gets all active characters of a specific main type, regardless of subtype.
    /// </summary>
    public List<Character> GetActiveCharacters(CharacterType mainType)
    {
        if (activeByMainType.ContainsKey(mainType))
        {
            return new List<Character>(activeByMainType[mainType]);
        }
        return new List<Character>();
    }

    /// <summary>
    /// Gets all active characters of a specific main type and subtype.
    /// </summary>
    public List<Character> GetActiveCharacters(CharacterType mainType, string subtypeName)
    {
        if (activeCharacters.ContainsKey(mainType) &&
            activeCharacters[mainType].ContainsKey(subtypeName))
        {
            return new List<Character>(activeCharacters[mainType][subtypeName]);
        }
        return new List<Character>();
    }

    /// <summary>
    /// Gets all active characters regardless of type.
    /// </summary>
    public List<Character> GetAllActiveCharacters()
    {
        return new List<Character>(allActiveCharacters);
    }

    /// <summary>
    /// Gets the count of active characters for a specific main type.
    /// </summary>
    public int GetActiveCount(CharacterType mainType)
    {
        if (activeByMainType.ContainsKey(mainType))
        {
            return activeByMainType[mainType].Count;
        }
        return 0;
    }

    /// <summary>
    /// Gets the count of active characters for a specific subtype.
    /// </summary>
    public int GetActiveCount(CharacterType mainType, string subtypeName)
    {
        if (activeCharacters.ContainsKey(mainType) &&
            activeCharacters[mainType].ContainsKey(subtypeName))
        {
            return activeCharacters[mainType][subtypeName].Count;
        }
        return 0;
    }

    /// <summary>
    /// Checks if a main type has reached its combined maximum active limit across all subtypes.
    /// </summary>
    public bool IsMainTypeAtCapacity(CharacterType mainType)
    {
        int maxAllowed = 0;
        int currentActive = GetActiveCount(mainType);

        // Sum up max allowed across all subtypes
        foreach (var config in characterConfigs)
        {
            if (config.mainType == mainType && config.maxActive > 0)
            {
                maxAllowed += config.maxActive;
            }
        }

        return maxAllowed > 0 && currentActive >= maxAllowed;
    }

    /// <summary>
    /// Checks if a specific subtype has reached its maximum active limit.
    /// </summary>
    public bool IsSubtypeAtCapacity(CharacterType mainType, string subtypeName)
    {
        CharacterSubtypeConfig config = FindConfig(mainType, subtypeName);
        if (config == null || config.maxActive < 0)
        {
            return false; // No limit or not configured
        }
        else if (config.maxActive == 0)
        {
            return true; // No active allowed
        }

            return GetActiveCount(mainType, subtypeName) >= config.maxActive;
    }

    /// <summary>
    /// Gets a list of all configured subtypes for a main character type.
    /// </summary>
    public List<string> GetSubtypes(CharacterType mainType)
    {
        List<string> subtypes = new List<string>();
        foreach (var config in characterConfigs)
        {
            if (config.mainType == mainType && config.prefab != null)
            {
                subtypes.Add(config.SubtypeName);
            }
        }
        return subtypes;
    }
}

/// <summary>
/// Component to help identify a character's subtype when returning to pool
/// </summary>
public class CharacterIdentifier : MonoBehaviour
{
    public string SubtypeName { get; set; }
}