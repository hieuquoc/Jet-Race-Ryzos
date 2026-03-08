using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
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

        void OnTriggerEnter(Collider other)
        {
            Debug.Log("SpaceShip collided with " + other.gameObject.name);
            GameManager.Instance.GameOver();
        }
    }
}


