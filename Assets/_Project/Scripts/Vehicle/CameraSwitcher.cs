using UnityEngine;
using Cinemachine;

namespace PolyStang
{
    /// <summary>
    /// Switches between rear and front driving cameras using Cinemachine priority
    /// Updated for Sprint 3 to use priority-based switching instead of SetActive
    /// </summary>
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Virtual Cameras")]
        public CinemachineVirtualCamera rearVirtualCamera;
        public CinemachineVirtualCamera frontVirtualCamera;

        [Header("Priority Settings")]
        [SerializeField] private int activePriority = 10;
        [SerializeField] private int inactivePriority = 5; // Still higher than walking camera (0)

        private bool isRearActive = true;

        // New Input System
        private PlayerInputActions controls;

        void Awake()
        {
            controls = new PlayerInputActions();
        }

        void OnEnable()
        {
            controls.Driving.Enable();
            controls.Driving.CameraSwitch.performed += _ => SwitchCamera();
        }

        void OnDisable()
        {
            controls.Driving.CameraSwitch.performed -= _ => SwitchCamera();
            controls.Driving.Disable();
        }

        void Start()
        {
            // Set initial camera priorities (rear active)
            if (rearVirtualCamera != null)
                rearVirtualCamera.Priority = activePriority;
            if (frontVirtualCamera != null)
                frontVirtualCamera.Priority = inactivePriority;

            isRearActive = true;
        }

        public void SwitchCamera()
        {
            if (rearVirtualCamera == null || frontVirtualCamera == null)
            {
                Debug.LogWarning("CameraSwitcher: Camera references not set!");
                return;
            }

            if (isRearActive)
            {
                // Switch to front camera
                rearVirtualCamera.Priority = inactivePriority;
                frontVirtualCamera.Priority = activePriority;
                isRearActive = false;
                Debug.Log("Camera: Switched to Front");
            }
            else
            {
                // Switch to rear camera
                frontVirtualCamera.Priority = inactivePriority;
                rearVirtualCamera.Priority = activePriority;
                isRearActive = true;
                Debug.Log("Camera: Switched to Rear");
            }
        }

        /// <summary>
        /// Force rear camera active (useful when entering vehicle)
        /// </summary>
        public void ForceRearCamera()
        {
            if (rearVirtualCamera == null || frontVirtualCamera == null) return;

            rearVirtualCamera.Priority = activePriority;
            frontVirtualCamera.Priority = inactivePriority;
            isRearActive = true;
        }
    }
}
