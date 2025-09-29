using UnityEngine;
using UnityEngine.InputSystem;

namespace ScaryGame
{
    public class CharacterControls : MonoBehaviour
    {
        [SerializeField] float _rotatingSpeed;
        TouchControls _touchControls;
        Vector3 _fingerLastPos;
        Camera _mainCamera;


        private void Awake()
        {
            _touchControls = new TouchControls();
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            _touchControls.Enable();
        }

        private void OnDisable()
        {
            _touchControls.Disable();
        }

        private void Start()
        {
            _touchControls.Controls.TouchPos.performed += ctx => SetFingerPosition(ctx);
        }

        private void OnDestroy()
        {
            _touchControls.Controls.TouchPos.performed -= ctx => SetFingerPosition(ctx);
        }

        private void Update()
        {
            if (GameManager.instance.gameState != GameManager.GameState.running) return;
            Vector2 diff = _fingerLastPos - transform.position;
            float rotationz = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, rotationz);
        }

        void SetFingerPosition(InputAction.CallbackContext context)
        {
            if (GameManager.instance.gameState != GameManager.GameState.running) return;
            Debug.Log("Touch Performed");
            Vector2 fingerPos = context.ReadValue<Vector2>();
            Vector2 worldFingerPos = _mainCamera.ScreenToWorldPoint(fingerPos);
            _fingerLastPos = worldFingerPos;
        }
    }
}
