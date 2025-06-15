using System.Collections.Generic;
using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private Canvas canvas;

    [SerializeField] private BasicAllyData data;
    [SerializeField] private AllyType allyType;
    
    public AllyType AllyType => allyType;
    protected BasicAllyData Data => data;

    private UpgradeManager upgradeManager;
    private LightData lightData;
    private Vector3 direction;
    //private Vector3[] sailed;


    public override void Initialize()
    {
        upgradeManager = GameManager.Instance.UpgradeManager;
        lightData = GameManager.Instance.LightController.LightData;
        canvas = GameManager.Instance.WorldSpaceCanvas;
        base.Initialize();
        lifeComponent = new LifeComponent();

        switch (allyType)
        {
            case AllyType.Basic:
                aiComponent = new BasicShipAIHandler();
                break;
            case AllyType.Boss:
                aiComponent = new BossShipAIHandler();
                break;
            case AllyType.Box:
                aiComponent = new BasicShipAIHandler();
                break;
        }
        
        
        CharacterData.Healthbar.Initialize();
        movementComponent.Rotate(direction);
        SetUpHealthbar();
    }
    public override void Update()
    {
        if (allyType == AllyType.Box)
        {
            return;
        }
        float distance = Vector3.Distance(GameManager.Instance.LightController.hit.point, transform.position);

        if (distance < upgradeManager.Radius)
        {
            movementComponent.Speed = CharacterData.CharacterTypeData.defaultSpeed + lightData.shipSpeedUpFactor;
        }
        else
        {
            movementComponent.Speed = CharacterData.CharacterTypeData.defaultSpeed;
        }
        if(allyType == AllyType.Basic)
        {
            aiComponent.AIAction(this, AIState.MoveToTarget, Data);
        }
    }

    /*public void Returned()
    {
        sailed[0] = new Vector3(6.83f, 0, 13);
        sailed[1] = new Vector3(3.63f, 0, 12);
        sailed[2] = new Vector3(-6.23f, 0, 13);
        sailed[3] = new Vector3(-2, 0, 12);

        // TODO: Make a SO to store positions of the sailed ships

        GameObject ghost = Instantiate(CharacterData.CharacterModel, sailed[Random.Range(0, sailed.Length)], Quaternion.identity);
        Vector3 direction = target.transform.position - ghost.transform.position;
        ghost.transform.rotation = Quaternion.LookRotation(direction);
        GameManager.Instance.returnedShips.Add(ghost);
    }*/

    private void SetUpHealthbar()
    {
        CharacterData.Healthbar.transform.SetParent(canvas.transform);
    }
}
