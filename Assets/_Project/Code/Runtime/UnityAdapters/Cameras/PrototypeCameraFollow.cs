using UnityEngine;

namespace DeepSeal.UnityAdapters.Cameras
{
    /// <summary>
    /// Minimal prototype camera follow component for the playable prototype scene.
    /// Attach this to the Main Camera and assign the player Transform explicitly in the scene.
    /// This is intentionally not Cinemachine; it is a small Unity adapter for the prototype pass.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(UnityEngine.Camera))]
    public sealed class PrototypeCameraFollow : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 0f, -10f);

        [Header("Follow")]
        [SerializeField] private bool snapToTargetOnStart = true;
        [SerializeField] private bool smoothFollow = true;
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private float maxFollowSpeed = 50f;
        [SerializeField] private bool followX = true;
        [SerializeField] private bool followY = true;

        private Vector3 followVelocity;
        private bool warnedMissingTarget;

        private void Start()
        {
            if (snapToTargetOnStart)
            {
                SnapToTarget();
            }
        }

        private void LateUpdate()
        {
            if (!TryGetDesiredPosition(out Vector3 desiredPosition))
            {
                return;
            }

            desiredPosition = ApplyAxisLocks(desiredPosition);

            if (smoothFollow && smoothTime > 0f)
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position,
                    desiredPosition,
                    ref followVelocity,
                    smoothTime,
                    maxFollowSpeed,
                    Time.deltaTime);
                return;
            }

            transform.position = desiredPosition;
            followVelocity = Vector3.zero;
        }

        [ContextMenu("Snap To Target")]
        public void SnapToTarget()
        {
            if (!TryGetDesiredPosition(out Vector3 desiredPosition))
            {
                return;
            }

            transform.position = ApplyAxisLocks(desiredPosition);
            followVelocity = Vector3.zero;
        }

        private bool TryGetDesiredPosition(out Vector3 desiredPosition)
        {
            desiredPosition = transform.position;

            if (target == null)
            {
                if (!warnedMissingTarget)
                {
                    Debug.LogWarning(
                        "Prototype camera follow requires a target Transform reference.",
                        this);
                    warnedMissingTarget = true;
                }

                return false;
            }

            warnedMissingTarget = false;
            desiredPosition = target.position + targetOffset;
            return true;
        }

        private Vector3 ApplyAxisLocks(Vector3 desiredPosition)
        {
            Vector3 currentPosition = transform.position;

            if (!followX)
            {
                desiredPosition.x = currentPosition.x;
            }

            if (!followY)
            {
                desiredPosition.y = currentPosition.y;
            }

            return desiredPosition;
        }

        private void Reset()
        {
            targetOffset = new Vector3(0f, 0f, -10f);
            snapToTargetOnStart = true;
            smoothFollow = true;
            smoothTime = 0.12f;
            maxFollowSpeed = 50f;
            followX = true;
            followY = true;
        }

        private void OnValidate()
        {
            smoothTime = Mathf.Max(0f, smoothTime);
            maxFollowSpeed = Mathf.Max(0.01f, maxFollowSpeed);
        }
    }
}