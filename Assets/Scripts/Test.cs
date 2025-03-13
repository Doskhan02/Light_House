using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private LightData data;
    void Start()
    {
        data = GameManager.Instance.LightController.LightData;
        transform.localScale = new Vector3(data.baseRadius * 0.4f, data.baseRadius * 0.4f, 1);
    }

    void Update()
    {
        transform.position = GameManager.Instance.LightController.hit.point + new Vector3 (0,0.1f,0); 
    }
}
