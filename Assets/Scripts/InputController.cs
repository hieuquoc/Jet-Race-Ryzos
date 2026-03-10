using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
            // legacy Input system doesn't need explicit enable
        }

        void OnDisable()
        {
            // legacy Input system doesn't need explicit disable
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
                Debug.Log("Moving camera with holdDirection: " + holdDirection);
                Camera.localPosition += Vector3.right * holdDirection * moveSpeed * Time.deltaTime;
            }
            spaceShip.Direction = Vector3.right * holdDirection;
        }

        private void HandleInput()
        {
            // keyboard input (arrow keys or A/D) takes priority (legacy Input)
            int keyboardDir = 0;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) keyboardDir = -1;
            else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) keyboardDir = 1;

            if (keyboardDir != 0)
            {
                holdDirection = keyboardDir;
                return;
            }

            // mouse input (desktop) - treat as single touch
            if (Input.GetMouseButton(0))
            {
                Vector2 pos = Input.mousePosition;
                if (IsPointerOverUI(pos))
                {
                    holdDirection = 0;
                    return;
                }
                float half = Screen.width * 0.5f;
                holdDirection = pos.x < half ? -1 : 1;
                return;
            }

            // touch input (mobile)
            if (Input.touchCount == 0)
            {
                holdDirection = 0;
                return;
            }

            UnityEngine.Touch touch = Input.touches[0];
            Vector2 screenPos = touch.position;
            // ignore touches that are over UI elements
            if (IsPointerOverUI(screenPos))
            {
                holdDirection = 0;
                return;
            }

            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                float half = Screen.width * 0.5f;
                holdDirection = screenPos.x < half ? -1 : 1;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                holdDirection = 0;
            }
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



