using UnityEngine;

public class AllyCharacter : Character
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject geo;
    private Vector3 direction;
    private Vector3[] sailed;
    private int score;


    public override void Initialize()
    {
        base.Initialize();
        lifeComponent = new LifeComponent();
        if (target == null)
        {
            target = GameManager.Instance.LightHouse;
        }
        
        movementComponent.Rotate(direction);
        score = Random.Range(2, 4);

        sailed = new Vector3[4];
    }
    public override void Update()
    {
        float distance = Vector3.Distance(GameManager.Instance.LightController.hit.point, transform.position);
        if (distance < 4)
        {
            movementComponent.Speed = CharacterData.DefaultSpeed + 2;
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
        sailed[0] = new Vector3(6.83f, 1, Random.Range(12f, 13f));
        sailed[1] = new Vector3(3.63f, 1, Random.Range(12f, 13f));
        sailed[2] = new Vector3(-6.23f, 1, Random.Range(12f, 13f));
        sailed[3] = new Vector3(-2, 1, Random.Range(12f, 13f));

        GameObject ghost = Instantiate(geo);
        int i = Random.Range(0,sailed.Length);
        ghost.transform.position = sailed[i];
        ghost.transform.rotation = Quaternion.LookRotation(Vector3.back);
    }
}
