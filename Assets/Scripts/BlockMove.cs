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
    [SerializeField] private Transform visualTransform;

    [SerializeField] private Vector3 currentMoveVector;

    void Awake()
    {
        if(visualTransform == null)
        {
            visualTransform = transform.GetChild(0);
        }
        startPosition = visualTransform.localPosition;
    }

    public override void SetUp()
    {
        base.SetUp();
        currentMoveVector = moveVector;
        visualTransform.localPosition = startPosition;
        float distance = Random.Range(-MoveDistance, MoveDistance);
        phaseOne = distance >= 0;
    }
    // Update is called once per frame
    void Update()
    {
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
        visualTransform.localPosition += currentMoveVector * MoveSpeed * Time.deltaTime;
    }

    public float Distance()
    {
        if(moveVector.x != 0)
        {
            return Mathf.Abs(visualTransform.localPosition.x - startPosition.x);
        }
        else if(moveVector.y != 0)
        {
            return Mathf.Abs(visualTransform.localPosition.y - startPosition.y);
        }
        else if(moveVector.z != 0)
        {
            return Mathf.Abs(visualTransform.localPosition.z - startPosition.z);
        }
        return 0f;
    }
}


