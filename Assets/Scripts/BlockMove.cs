using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMove : BaseMoveObstacle
{
    
    public float MoveSpeed = 5f;
    public float MoveDistance = 2f;
    public float ChangeTime = 0f;
    [SerializeField] private Vector3 startPosition;
    
    [SerializeField] private Transform visualTransform;
    [SerializeField] private bool autoStart = false;
    [SerializeField] private Vector3 moveVector;
    [SerializeField] private float curMoveSpeed = 0f;
    [SerializeField] private float currentChangeTime = 0f;
    [SerializeField] private int direction = 1;


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
        float distance = Random.Range(0, MoveDistance);
        visualTransform.localPosition = startPosition + moveVector.normalized * distance;       
        curMoveSpeed = Random.Range(MoveSpeed * 0.5f, MoveSpeed);
        currentChangeTime = ChangeTime;
        Debug.Log($"Set up BlockMove at {visualTransform.localPosition} with move vector {moveVector} and distance {distance}"); 
        direction = 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (Distance() >= MoveDistance && direction == 1)
        {
            if(currentChangeTime > 0)
            {
                currentChangeTime -= Time.deltaTime;
                if(currentChangeTime <= 0)
                {
                    currentChangeTime = ChangeTime;
                    direction = -1;
                }
                return;
            }
        }
        else if (Distance() <= 0.1f && direction == -1)
        {
            if(currentChangeTime > 0)
            {
                currentChangeTime -= Time.deltaTime;
                if(currentChangeTime <= 0)
                {
                    currentChangeTime = ChangeTime;
                    direction = 1;
                }
                return;
            }
        }
        visualTransform.localPosition += moveVector * curMoveSpeed * direction * Time.deltaTime;
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


