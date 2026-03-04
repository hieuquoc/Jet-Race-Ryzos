using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : MonoBehaviour
{
    public MoveDirection MoveDirection = MoveDirection.UpDown;
    public float MoveSpeed = 5f;
    public float MoveDistance = 2f;

    private Vector3 startPosition;
    private bool phaseOne = true;

    void OnEnable()
    {
        startPosition = transform.position;
        phaseOne = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (MoveDirection)
        {
            case MoveDirection.Left:
            case MoveDirection.Right:
                if (phaseOne && Vector3.Distance(transform.position, startPosition) >= MoveDistance)
                {
                    phaseOne = false;
                }
                else if (!phaseOne && Vector3.Distance(transform.position, startPosition) <= 0.1f)
                {
                    phaseOne = true;
                }
                break;
            case MoveDirection.UpDown:
                if (phaseOne && Vector3.Distance(transform.position, startPosition) >= MoveDistance)
                {
                    phaseOne = false;
                }
                else if (!phaseOne && Vector3.Distance(transform.position, startPosition) <= 0.1f)
                {
                    phaseOne = true;
                }
                break;
        }
        MoveDirection currentDirection = phaseOne ? MoveDirection : GetOppositeDirection(MoveDirection);
        MoveInDirection(currentDirection, MoveSpeed);
    }

    void MoveInDirection(MoveDirection direction, float speed)
    {
        Vector3 moveVector = Vector3.zero;
        switch (direction)
        {
            case MoveDirection.Left:
                moveVector = Vector3.left;
                break;
            case MoveDirection.Right:
                moveVector = Vector3.right;
                break;
            case MoveDirection.UpDown:
                moveVector = Vector3.up;
                break;
        }
        transform.Translate(moveVector * speed * Time.deltaTime);
    }

    MoveDirection GetOppositeDirection(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Left:
                return MoveDirection.Right;
            case MoveDirection.Right:
                return MoveDirection.Left;
            case MoveDirection.UpDown:
                return MoveDirection.UpDown;
            default:
                return direction;
        }
    }
}

public enum MoveDirection
{
    Left,
    Right,
    UpDown
}
