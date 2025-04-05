using UnityEngine;

public class Test : MonoBehaviour
{
    private float radius;

    void Update()
    {
        radius = GameManager.Instance.UpgradeManager.Radius;
        transform.localScale = new Vector3(radius * 0.4f, radius * 0.4f, 1);
        transform.position = GameManager.Instance.LightController.hit.point + new Vector3 (0,0.1f,0); 
    }
}
