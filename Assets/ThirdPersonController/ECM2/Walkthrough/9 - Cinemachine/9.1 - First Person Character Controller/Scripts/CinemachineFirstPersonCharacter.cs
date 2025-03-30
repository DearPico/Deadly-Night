using UnityEngine;

namespace ECM2.Walkthrough.Ex91
{
    public class CinemachineFirstPersonCharacter : Character
    {
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow.")]
        public GameObject cameraTarget;
        
        [Space(15.0f)]
        [Tooltip("Cinemachine Camera positioned at desired crouched height.")]
        public GameObject crouchedCamera;
        [Tooltip("Cinemachine Camera positioned at desired un-crouched height.")]
        public GameObject unCrouchedCamera;
        
        // Current camera target pitch
        
        private float _cameraTargetPitch;
        
        /// <summary>
        /// Add input (affecting Yaw).
        /// This is applied to the Character's rotation.
        /// </summary>
        
        public void AddControlYawInput(float value)
        {
            AddYawInput(value);
        }
        
        /// <summary>
        /// Add input (affecting Pitch).
        /// This is applied to the cameraTarget's local rotation.
        /// </summary>
        
        public void AddControlPitchInput(float value, float minValue = -80.0f, float maxValue = 80.0f)
        {
            if (value == 0.0f)
                return;
            
            _cameraTargetPitch = MathLib.ClampAngle(_cameraTargetPitch + value, minValue, maxValue);
            cameraTarget.transform.localRotation = Quaternion.Euler(-_cameraTargetPitch, 0.0f, 0.0f);
        }
        
        /// <summary>
        /// When character crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>

        protected override void OnCrouched()
        {
            base.OnCrouched();
            
            crouchedCamera.SetActive(true);
            unCrouchedCamera.SetActive(false);
        }
        
        /// <summary>
        /// When character un-crouches, toggle Crouched / UnCrouched cameras.
        /// </summary>

        protected override void OnUnCrouched()
        {
            base.OnUnCrouched();
            
            crouchedCamera.SetActive(false);
            unCrouchedCamera.SetActive(true);
        }

        protected override void Start()
        {
            base.Start();
            
            // Disable Character's rotation mode, we'll handle it here
            
            SetRotationMode(Character.RotationMode.None);
            
            // Lock cursor
            
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
