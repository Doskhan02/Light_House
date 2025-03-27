using UnityEngine;

public class AllyCharacter : Character
{
    private GameObject target;
    [SerializeField] private GameObject geo;

    [SerializeField] private Canvas canvas;

    private UpgradeManager upgradeManager;
    private LightData lightData;
    private Vector3 direction;
    private Vector3[] sailed;
    private readonly int score = 2;


    public override void Initialize()
    {
        upgradeManager = GameManager.Instance.UpgradeManager;
        lightData = GameManager.Instance.LightController.LightData;
        canvas = GameManager.Instance.WorldSpaceCanvas;
        base.Initialize();
        lifeComponent = new LifeComponent();
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("ShipTarget");
        }
        
        CharacterData.Healthbar.Initialize();
        movementComponent.Rotate(direction);
        sailed = new Vector3[4];
        SetUpHealthbar();
    }
    public override void Update()
    {
        float distance = Vector3.Distance(GameManager.Instance.LightController.hit.point, transform.position);

        if (distance < upgradeManager.Radius)
        {
            movementComponent.Speed = CharacterData.DefaultSpeed + lightData.shipSpeedUpFactor;
        }
        else
        {
            movementComponent.Speed = CharacterData.DefaultSpeed;
        }

        direction = target.transform.position - transform.position;
        movementComponent.Move(direction);
        movementComponent.Rotate(direction);
        if (direction.magnitude < 9)
        {
            Returned();
            lifeComponent.SetDamage(lifeComponent.MaxHealth);
            GameManager.Instance.ScoreSystem.AddScore(score);
            gameObject.SetActive(false);


        }
    }

    public void Returned()
    {
        sailed[0] = new Vector3(6.83f, 1, 13);
        sailed[1] = new Vector3(3.63f, 1, 12);
        sailed[2] = new Vector3(-6.23f, 1, 13);
        sailed[3] = new Vector3(-2, 1, 12);

        GameObject ghost = Instantiate(geo, sailed[Random.Range(0, sailed.Length)], Quaternion.identity);
        Vector3 direction = target.transform.position - ghost.transform.position;
        ghost.transform.rotation = Quaternion.LookRotation(direction);
        GameManager.Instance.returnedShips.Add(ghost);
    }

    public void SetUpHealthbar()
    {
        CharacterData.Healthbar.transform.SetParent(canvas.transform);
    }
}
