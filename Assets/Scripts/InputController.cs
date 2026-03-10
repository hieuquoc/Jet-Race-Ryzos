using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.EventSystems;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace ZyroX
{
    public class InputController : MonoBehaviour
    {
        public Transform Camera;
        public SpaceShip spaceShip;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private bool isInputEnabled = false;
        [SerializeField] private bool isLockInput = true;
        public static InputController Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        [SerializeField] private int holdDirection = 0; // -1 trái, 1 phải, 0 không giữ

        void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }

        void OnDisable()
        {
            TouchSimulation.Disable();
            EnhancedTouchSupport.Disable();
        }

        void Start()
        {
        }

        void Update()
        {
            if (!isInputEnabled) return;
            if(!isLockInput)
                HandleInput();

            if (holdDirection != 0 && Camera != null)
            {
                Camera.localPosition += Vector3.right * holdDirection * moveSpeed * Time.deltaTime;
            }
        }

        private void HandleInput()
        {
            var touches = Touch.activeTouches;
            if (touches.Count == 0)
            {
                holdDirection = 0;
                return;
            }

            var touch = touches[0];
            // ignore touches that are over UI elements
            if (IsPointerOverUI(touch.screenPosition))
            {
                holdDirection = 0;
                spaceShip.Direction = Vector3.zero;
                return;
            }
            var phase = touch.phase;

            if (phase == UnityEngine.InputSystem.TouchPhase.Began ||
                phase == UnityEngine.InputSystem.TouchPhase.Stationary ||
                phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                float half = Screen.width * 0.5f;
                holdDirection = touch.screenPosition.x < half ? -1 : 1;
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                holdDirection = 0;
            }
            spaceShip.Direction = Vector3.right * holdDirection;
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            if (EventSystem.current == null) return false;
            var ped = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);
            return results.Count > 0;
        }

        public void SetInputEnabled(bool enabled)
        {
            holdDirection = 0;
            isInputEnabled = enabled;
            spaceShip.Direction = Vector3.zero;
        }

        public void OverrideDirection(int direction, float duration = 0f)
        {
            StartCoroutine(OverrideDirectionCoroutine(direction, duration));
        }

        IEnumerator OverrideDirectionCoroutine(int direction, float duration)
        {
            isLockInput = true;
            holdDirection = direction;
            yield return new WaitForSeconds(duration);
            isLockInput = false;
        }
    }
}



