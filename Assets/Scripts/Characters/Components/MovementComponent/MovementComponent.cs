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

}
