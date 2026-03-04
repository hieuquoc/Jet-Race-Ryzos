using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : BaseMoveObstacle
{
    
    public float MoveSpeed = 5f;
    public float MoveDistance = 2f;

    private Vector3 startPosition;
    private bool phaseOne = true;
    public Vector3 moveVector;

    [SerializeField] private Vector3 currentMoveVector;

    public override void SetUp()
    {
        base.SetUp();
        startPosition = transform.position;
        currentMoveVector = moveVector;
        float distance = Random.Range(-MoveDistance, MoveDistance);
        phaseOne = distance >= 0;
        transform.position += moveVector.normalized * distance;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log("start position: " + startPosition + " position: " + transform.position + " phaseOne: " + phaseOne + " distance: " + Distance());
        if (Distance() >= MoveDistance && phaseOne)
        {
            currentMoveVector *= -1;
            phaseOne = false;
        }
        else if (Distance() <= 0.1f && !phaseOne)
        {
            currentMoveVector *= -1;
            phaseOne = true;
        }
        transform.Translate(currentMoveVector * MoveSpeed * Time.deltaTime);
    }

    public float Distance()
    {
        if(moveVector.x != 0)
        {
            return Mathf.Abs(transform.position.x - startPosition.x);
        }
        else if(moveVector.y != 0)
        {
            return Mathf.Abs(transform.position.y - startPosition.y);
        }
        else if(moveVector.z != 0)
        {
            return Mathf.Abs(transform.position.z - startPosition.z);
        }
        return 0f;
    }
}


