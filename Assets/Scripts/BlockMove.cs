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
    [SerializeField] private float currentChangeTime = 0f;
    [SerializeField] private int direction = 1;
    [SerializeField] private float recoverSpeed = 1;


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
        if(direction > 0)
            visualTransform.localPosition += moveVector * MoveSpeed * direction * Time.deltaTime;
            else
            visualTransform.localPosition += moveVector * MoveSpeed * recoverSpeed * direction * Time.deltaTime;
        if(moveVector.x != 0)
        {
            SnapPositionX();
        }
        else if(moveVector.y != 0)
        {
            SnapPositionY();
        }
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
        return 0f;
    }

    public void SnapPositionX()
    {
        Vector3 maxPos = MaxPosition();
        if(startPosition.x < maxPos.x)
        {
            float x = Mathf.Clamp(visualTransform.localPosition.x, startPosition.x, maxPos.x);
            visualTransform.localPosition = new Vector3(x, visualTransform.localPosition.y, visualTransform.localPosition.z);
        }
        else
        {
            float x = Mathf.Clamp(visualTransform.localPosition.x, maxPos.x, startPosition.x);
            visualTransform.localPosition = new Vector3(x, visualTransform.localPosition.y, visualTransform.localPosition.z);
        }
    }

    public void SnapPositionY()
    {
        Vector3 maxPos = MaxPosition();
        if(startPosition.y < maxPos.y)
        {
            float y = Mathf.Clamp(visualTransform.localPosition.y, startPosition.y, maxPos.y);
            visualTransform.localPosition = new Vector3(visualTransform.localPosition.x, y, visualTransform.localPosition.z);
        }
        else
        {
            float y = Mathf.Clamp(visualTransform.localPosition.y, maxPos.y, startPosition.y);
            visualTransform.localPosition = new Vector3(visualTransform.localPosition.x, y, visualTransform.localPosition.z);
        }
    }

    public Vector3 MaxPosition()
    {
        return startPosition + moveVector.normalized * MoveDistance;
    }
}


