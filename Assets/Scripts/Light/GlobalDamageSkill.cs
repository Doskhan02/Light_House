using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalDamageSkill : MonoBehaviour
{
    private float damage = 10f; // Damage amount
    private float cooldown = 5f; // Cooldown time in seconds

    private List<Character> enemies; // List of enemies in the game

    private CharacterSpawnSystem spawnSystem;

    // Start is called before the first frame update
    void Start()
    {
        spawnSystem = CharacterSpawnSystem.Instance;
        if (spawnSystem == null)
        {
            Debug.LogError("CharacterSpawnSystem instance not found.");
            return;
        }

    }

    public void ApplySkill(List<Character> targets)
    {
        // Get all enemies in the game
        enemies = spawnSystem.CharacterFactory.GetActiveCharacters(CharacterType.Enemy);
        // Apply damage to each enemy
        foreach (Character enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.lifeComponent.SetDamage(damage);
            }
        }
    }
}
