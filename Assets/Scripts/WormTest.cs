using System.Collections.Generic;
using UnityEngine;

public class WormBodyFollow : MonoBehaviour
{
    public Transform[] wormBones; // Assign bones from Head to Tail
    public float rotationSpeed = 5f; // How quickly each joint rotates
    public float followDelay = 0.2f; // Delay effect for trailing movement

    private Quaternion[] targetRotations; // Stores the rotation targets

    void Start()
    {
        targetRotations = new Quaternion[wormBones.Length];

        // Initialize each bone's rotation target
        for (int i = 0; i < wormBones.Length; i++)
        {
            targetRotations[i] = wormBones[i].rotation;
        }
    }

    void Update()
    {
        // First joint follows its direct movement
        targetRotations[0] = wormBones[0].rotation;

        // Apply rotation to the rest of the joints
        for (int i = 1; i < wormBones.Length; i++)
        {
            // Gradually adjust the rotation target based on the previous bone
            targetRotations[i] = Quaternion.Slerp(
                targetRotations[i],
                wormBones[i - 1].rotation,
                followDelay
            );

            // Smoothly rotate towards the target
            wormBones[i].rotation = Quaternion.Slerp(
                wormBones[i].rotation,
                targetRotations[i],
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
