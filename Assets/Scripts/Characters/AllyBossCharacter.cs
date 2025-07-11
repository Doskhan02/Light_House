using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AllyBossCharacter : AllyCharacter
{
    [SerializeField] private bool[] turret;
    [SerializeField] private GameObject turretGeo;
    [SerializeField] private Vector3[] waypoints;
    private bool firePending = false;
    private bool isStuned = false;
    
    private RaycastHit hit;
    private UpgradeManager upgradeManager;
    
    public bool FirePending => firePending;
    public bool[] Turret => turret;
    
    public Vector3[] Waypoints { get => waypoints; set => waypoints = value; }
    
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
    private Character AmmoBox
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

    private GameObject safeZone
    {
        get
        {
            var safeZone = GameObject.FindWithTag("SafeZone");
            return safeZone;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        turret = new bool[GameManager.Instance.PieceAmount];
        hit = GameManager.Instance.LightController.hit;
        upgradeManager = GameManager.Instance.UpgradeManager;
    }

    public override void Update()
    {
        if (isStuned)
        {
            aiComponent.AIAction(this, AIState.Fear, Data);
            return;
        }

        if (Vector3.Distance(transform.position, hit.point) < upgradeManager.Radius)
        {
            var newSpeed = CharacterData.CharacterTypeData.defaultSpeed + 4f;
            movementComponent.Speed = newSpeed;
        }
        
        if (!CheckTurret() && AmmoBox != null)
        {
            aiComponent.AIAction(AmmoBox,AIState.MoveToTarget,Data);
        }
        else if(AmmoBox == null || CheckTurret())
        {
            aiComponent.AIAction(this, AIState.Idle, Data);
        }
        turretGeo.gameObject.SetActive(CheckTurret());
    }

    public void Stun(bool stun)
    {
        isStuned = stun;
        if (isStuned)
        {
            CancelInvoke(nameof(StopStun));
            Invoke(nameof(StopStun), 4.5f);
        }
    }

    private void StopStun()
    {
        Stun(false);
    }
    
    private bool CheckTurret()
    {
        for (int i = 0; i < turret.Length; i++)
        {
            if (!turret[i])
            {
                firePending = false;
                return false;
            }
        }
        firePending = true;
        return true;
    }

    public void AddPiece()
    {
        for (int i = 0; i < turret.Length; i++)
        {
            if (!turret[i])
            {
                turret[i] = true;
                break;
            }
        }
    }

    public void Fire()
    {
        if (turretGeo.gameObject.TryGetComponent<Turret>(out Turret turret))
        {
            if (firePending)
            {
                turret.Fire();
                EmptyTurrets();
            }
        }
    }

    public void EmptyTurrets()
    {
        for (int i = 0; i < turret.Length; i++)
        {
            turret[i] = false;
        }
        firePending = false;
    }
}
