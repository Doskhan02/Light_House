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
        // Step 1: Collect all positions that match the checkered pattern
        List<Vector2Int> checkeredPositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if ((x + z) % 2 == 0)
                {
                    checkeredPositions.Add(new Vector2Int(x, z));
                }
            }
        }

        // Step 2: Shuffle the list (Fisher-Yates shuffle)
        Shuffle(checkeredPositions);

        // Step 3: Spawn each one with delay
        foreach (var pos in checkeredPositions)
        {
            float waveOffset = Mathf.Sin((pos.x + pos.y) * 0.5f) * waveSpeed;
            yield return new WaitForSeconds(delayPerTile + waveOffset * 0.01f);

            Vector3 position = new Vector3(-20, 0, 20) + new Vector3(pos.x * spacing, 0, pos.y * spacing);
            GameObject go = Instantiate(prefab, position, Quaternion.identity, transform);
            go.transform.localScale *= Random.Range(0.8f, 1.2f);
        }
    }
    private void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}


