using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovementComponent : IMovementComponent
{
    public CharacterData characterData;
    public float speed;
    public float Speed 
    { 
        get => speed; 
        set
        {
            if(speed < 0) 
                speed = 0;
            speed = value;
        } 
    }

    public void Initialize(CharacterData characterData)
    {
        this.characterData = characterData;
        speed = characterData.DefaultSpeed;
    }

    public void Initialize(Character selfCharacter)
    {
        
        
    }

    public void Move(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Vector3 move = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        move = AdjustDirectionWithCollision(move);

        characterData.CharacterController.Move(move * Speed * Time.deltaTime);
    }

    public void Rotate(Vector3 direction)
    {
        float smooth = 0.05f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(characterData.CharacterTransform.eulerAngles.y, targetAngle, ref smooth, smooth);

        Quaternion rotate = Quaternion.Euler(0, angle, 0);
        characterData.CharacterTransform.rotation = rotate;
    }

    private Vector3 AdjustDirectionWithCollision(Vector3 direction)
    {
        Vector3 origin = characterData.CharacterTransform.position + Vector3.up * 0.5f; // Adjust height if needed

        if (Physics.SphereCast(origin, characterData.CharacterController.radius * 2, direction, out RaycastHit hit, characterData.CharacterController.radius * 2, characterData.CharacterMask.value))
        {
            Vector3 normal = new Vector3 (hit.normal.x, 0,hit.normal.z) ;
            direction += normal;  // Adjust direction by adding the normal
            direction.Normalize();  // Normalize to maintain speed

            Debug.DrawRay(hit.point, normal * 2, Color.red); // Debugging: Show normal direction
            Debug.Log("Collision detected! Adjusting movement.");
        }

        return direction;
    }
}
