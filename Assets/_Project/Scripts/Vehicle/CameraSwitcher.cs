using UnityEngine;
using Cinemachine;

namespace PolyStang
{
    /// <summary>
    /// Switches between primary driving camera (FreeLook) and top-down GPS-style view
    /// Updated for Sprint 4 to delegate to CameraStateManager
    /// </summary>
    public class CameraSwitcher : MonoBehaviour
    {
        [Header("Camera Manager Reference")]
        [Tooltip("Reference to the CameraStateManager that handles camera priorities")]
        public CameraStateManager cameraStateManager;

        // New Input System
        private PlayerInputActions controls;

        void Awake()
        {
            controls = new PlayerInputActions();
            
            // Auto-find CameraStateManager if not assigned
            if (cameraStateManager == null)
            {
                cameraStateManager = FindFirstObjectByType<CameraStateManager>();
                if (cameraStateManager == null)
                {
                    Debug.LogError("CameraSwitcher: CameraStateManager not found! Cannot switch cameras.");
                }
            }
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

        /// <summary>
        /// Toggle between primary and top-down driving views
        /// </summary>
        public void SwitchCamera()
        {
            if (cameraStateManager == null)
            {
                Debug.LogError("CameraSwitcher: CameraStateManager not assigned!");
                return;
            }
            
            // Delegate to CameraStateManager to handle the toggle
            cameraStateManager.ToggleDrivingView();
        }

        /// <summary>
        /// Force primary camera active (useful when entering vehicle)
        /// No longer needed but kept for backward compatibility
        /// </summary>
        public void ForceRearCamera()
        {
            // Primary camera is already activated by CameraStateManager when entering driving state
            Debug.Log("CameraSwitcher: ForceRearCamera() called - CameraStateManager handles this automatically");
        }
    }
}
