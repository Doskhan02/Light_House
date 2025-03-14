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
        Speed = characterData.DefaultSpeed;
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
        Vector3 origin = characterData.CharacterTransform.position + Vector3.up * 0.5f;
        float radius = characterData.CharacterController.radius * 2;
        LayerMask mask = characterData.CharacterMask.value;

        // Primary forward cast
        if (Physics.SphereCast(origin, radius, direction, out RaycastHit hit, radius, mask))
        {
            Vector3 normal = new Vector3(hit.normal.x, 0, hit.normal.z);
            direction += normal;
            direction.Normalize();
            Debug.DrawRay(hit.point, normal * 2, Color.red);
        }

        Vector3 rightOffset = Quaternion.Euler(0, 30, 0) * direction;
        Vector3 leftOffset = Quaternion.Euler(0, -30, 0) * direction;

        if (Physics.SphereCast(origin, radius, rightOffset, out RaycastHit rightHit, radius, mask))
        {
            Vector3 normal = new Vector3(rightHit.normal.x, 0, rightHit.normal.z);
            direction += normal;
            direction.Normalize();
            Debug.DrawRay(rightHit.point, normal * 2, Color.green);
        }

        if (Physics.SphereCast(origin, radius, leftOffset, out RaycastHit leftHit, radius, mask))
        {
            Vector3 normal = new Vector3(leftHit.normal.x, 0, leftHit.normal.z);
            direction += normal;
            direction.Normalize();
            Debug.DrawRay(leftHit.point, normal * 2, Color.blue);
        }

        return direction;
    }
}
