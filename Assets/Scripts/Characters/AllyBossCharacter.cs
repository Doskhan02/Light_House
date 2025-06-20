using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AllyBossCharacter : AllyCharacter
{
    [SerializeField] private Turret[] turrets;
    [SerializeField] private float initAmmo;
    [SerializeField] private Vector3[] waypoints;
    
    public Vector3[] Waypoints { get => waypoints; set => waypoints = value; }

    private float currentAmmo;
    
    public override Character Target
    {
        get
        {
            Character target = null;
            float minDistance = 50;
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy);
            foreach (var character in list)
            {
                if (!character.gameObject.activeSelf)
                    continue;

                float distanceBetween = Vector3.Distance(character.transform.position, transform.position);
                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = character;
                }
            }
            return target;
        }
    }
    public Character AmmoBox
    {
        get
        {
            Character target = null;
            float minDistance = float.MaxValue; // Start with max value to find closest
            List<Character> list = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally);

            foreach (var character in list)
            {
                if (!character.gameObject.activeSelf)
                    continue;

                if (character == this)
                    continue;

                float distanceBetween = Vector3.Distance(character.transform.position, transform.position);

                if (distanceBetween < minDistance)
                {
                    minDistance = distanceBetween;
                    target = character;
                }
            }

            return target;
        }
    }


    public override void Initialize()
    {
        base.Initialize();
        foreach (Turret turret in turrets)
        {
            turret.Initialize(initAmmo);
            currentAmmo +=  turret.CurrentAmmo;
        }
    }

    public override void Update()
    {
        CheckForAmmo();
        switch (currentAmmo)
        {
            case <= 0 when AmmoBox != null:
                aiComponent.AIAction(AmmoBox,AIState.MoveToTarget,Data);
                return;
            case > 0:
                aiComponent.AIAction(this, AIState.Idle, Data);
                break;
        }
    }

    private void CheckForAmmo()
    {
        if(turrets.Length == 0)
            return;
        currentAmmo = 0;
        foreach (Turret turret in turrets)
        {
            currentAmmo += turret.CurrentAmmo;
        }
    }

    public void ReloadTurrets(float ammo)
    {
        foreach (Turret turret in turrets)
        {
            turret.Reload(ammo);
        }
    }
}
