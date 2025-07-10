using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StunningHit : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float health;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas canvas;
    
    private float maxHealth;
    
    private float stunTime;
    
    private RaycastHit hit;
    private UpgradeManager upgradeManager;
    private bool initialized;

    private AllyBossCharacter bossCharacter;
    private EnemyBossCharacter enemyBossCharacter;
        

    private void Start()
    {
        canvas.worldCamera = Camera.main;
        initialized = true;
        stunTime = 4.5f;
        maxHealth = health;
        healthSlider.value = health / maxHealth;
        upgradeManager = GameManager.Instance.UpgradeManager;
        bossCharacter = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Ally, "Boat3").FirstOrDefault() as AllyBossCharacter;
        enemyBossCharacter = CharacterSpawnSystem.Instance.CharacterFactory.GetActiveCharacters(CharacterType.Enemy, "DT(BOSS)").FirstOrDefault() as EnemyBossCharacter;
        if (bossCharacter != null) bossCharacter.Stun(true);
    }
    public void Update()
    {
        if(!initialized)
            return;
        hit = GameManager.Instance.LightController.hit;
        var distance = hit.point - transform.position;
        if (distance.magnitude < upgradeManager.Radius)
        {
            if (health < upgradeManager.Damage)
            {
                bossCharacter?.Stun(false);
                animator.SetTrigger("Hit");
            }
            SetDamage(upgradeManager.Damage);
        }
        stunTime -= Time.deltaTime;
        if (stunTime < 0)
        {
            Destroy();
        }
    }

    private void SetDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Invoke("Destroy", 1f);
        }
        healthSlider.value = health /  maxHealth;
    }

    private void Destroy()
    {
        enemyBossCharacter.Rise();
        Destroy(gameObject);
    }
}
