using UnityEngine;

namespace HorseBet.Camera
{
    public class CameraSettings : MonoBehaviour
    {
        [Header("Target Settings")]
        public Transform target;

        [Header("Follow Settings")]
        public Vector3 offset = new Vector3(0, 5, -10);
        public float smoothSpeed = 0.125f;
        public bool lookAtTarget = true;

        [Header("Rotation Settings")]
        public bool fixedRotation = false;
        private Quaternion initialRotation;

        [Header("Idle Movement")]
        public bool isIdling = true;
        public float idleSpeed = 1.0f;
        public float idleHeight = 0.5f;
        private Vector3 startPosition;

        void Start()
        {
            initialRotation = transform.rotation;
            startPosition = transform.position;
        }

        void LateUpdate()
        {
            if (isIdling)
            {
                float yOffset = Mathf.Sin(Time.time * idleSpeed) * idleHeight;
                transform.position = startPosition + new Vector3(0, yOffset, 0);
            }
            else if (target != null)
            {
                FollowTarget();
            }
            else
            {
                Debug.LogWarning("Camera is not idling but has no target to follow!");
            }
        }

        void FollowTarget()
        {
            if (target == null)
            {
                Debug.LogWarning("Cannot follow target: target is null");
                return;
            }

            Vector3 desiredPosition = target.position + offset;

            if (smoothSpeed <= 0)
            {
                transform.position = desiredPosition;
            }
            else
            {
                Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
                transform.position = smoothedPosition;
            }

            if (fixedRotation)
            {
                transform.rotation = initialRotation;
            }
            else if (lookAtTarget)
            {
                transform.LookAt(target);
            }
        }

        public void SetTarget(Transform newTarget)
        {
            if (newTarget != null)
            {
                Debug.Log($"CameraSettings: Setting target to {newTarget.name}");
                target = newTarget;
                isIdling = false;
            }
            else
            {
                Debug.LogError("Attempted to set camera target to null!");
            }
        }

        public void EnableIdleMode()
        {
            isIdling = true;
        }

        public void LockCurrentRotation()
        {
            initialRotation = transform.rotation;
            fixedRotation = true;
        }
    }
}
