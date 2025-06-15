using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTentacle : MonoBehaviour
{

    [SerializeField] private GameObject prefab;
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float spacing = 1.0f;
    [SerializeField] private float delayPerTile = 0.05f;
    [SerializeField] private float waveSpeed = 2f;

    public void Initialize()
    {
        StartCoroutine(SpawnInWave());
    }

    private IEnumerator SpawnInWave()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if ((x + z) % 2 == 0) // Checkered pattern
                {
                    float waveOffset = Mathf.Sin((x + z) * 0.5f) * waveSpeed;
                    yield return new WaitForSeconds(delayPerTile + waveOffset * 0.01f);

                    Vector3 position = new Vector3(-30,0,20) + new Vector3(x * spacing, 0, z * spacing);
                    Instantiate(prefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}


