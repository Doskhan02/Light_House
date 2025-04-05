using System.Collections.Generic;
using UnityEngine;

public class WormBodyFollow : MonoBehaviour
{
    public Transform[] wormBones; 
    public float rotationSpeed = 5f; 
    public float followDelay = 0.2f; 

    private Quaternion[] targetRotations;

    void Start()
    {
        targetRotations = new Quaternion[wormBones.Length];

        for (int i = 0; i < wormBones.Length; i++)
        {
            targetRotations[i] = wormBones[i].rotation;
        }
    }

    void Update()
    {
        targetRotations[0] = wormBones[0].rotation;

        for (int i = 1; i < wormBones.Length; i++)
        {
            targetRotations[i] = Quaternion.Slerp(
                targetRotations[i],
                wormBones[i - 1].rotation,
                followDelay
            );

            wormBones[i].rotation = Quaternion.Slerp(
                wormBones[i].rotation,
                targetRotations[i],
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
