using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    public Vector3 Direction;
    public float MaxAngle = 30f;
    public float RotationSpeed = 90f;

    private float currentAngle = 0f;

    void Update()
    {
        float targetAngle = Direction.x != 0 ? -Mathf.Sign(Direction.x) * MaxAngle : 0f;
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, RotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }
}

