using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : BaseMoveObstacle
{
    
    public float MoveSpeed = 5f;
    public float MoveDistance = 2f;
    [SerializeField] private Vector3 startPosition;
    
    [SerializeField] private Transform visualTransform;

    [SerializeField] private Vector3 currentMoveVector;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private float curMoveSpeed = 0f;

    void Awake()
    {
        if(visualTransform == null)
        {
            visualTransform = transform.GetChild(0);
        }
        startPosition = visualTransform.localPosition;
        if(autoStart)
        {
            SetUp();
        }
    }

    public override void SetUp()
    {
        base.SetUp();
        currentMoveVector = moveVector;
        float distance = Random.Range(0, MoveDistance);
        visualTransform.localPosition = startPosition + moveVector.normalized * distance;       
        curMoveSpeed = Random.Range(MoveSpeed * 0.5f, MoveSpeed);
        Debug.Log($"Set up BlockMove at {visualTransform.localPosition} with move vector {moveVector} and distance {distance}"); 
    }
    // Update is called once per frame
    void Update()
    {
        if (Distance() >= MoveDistance)
        {
            currentMoveVector = -moveVector;
        }
        else if (Distance() <= 0.1f)
        {
            currentMoveVector = moveVector;
        }
        visualTransform.localPosition += currentMoveVector * curMoveSpeed * Time.deltaTime;
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


