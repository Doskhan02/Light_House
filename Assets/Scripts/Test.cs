using UnityEngine;

public class Test : MonoBehaviour
{
    private float radius;

    void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = GameManager.Instance.IsGameActive;
        radius = GameManager.Instance.UpgradeManager.Radius;
        transform.localScale = new Vector3(radius * 0.6f, radius * 0.6f , 1);
        transform.position = GameManager.Instance.LightController.hit.point + new Vector3 (0,0.1f,0); 
    }
}
