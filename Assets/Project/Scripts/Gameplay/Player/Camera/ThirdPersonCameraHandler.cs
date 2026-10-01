using UnityEngine;

namespace LevelDesign.Systems.Player
{
    public class ThirdPersonCameraHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform pivot;

        [Header("Distance")]
        [SerializeField] private float maxDistance = 0f;
        [SerializeField] private float minDistance = 0.3f;

        [Header("Collision")]
        [SerializeField] private LayerMask obstructionLayers;
        [SerializeField] private float cameraRadius = 0.25f;
        [SerializeField] private float surfacePadding = 0.05f;

        [Header("Smoothing")]
        [SerializeField] private float returnSpeed = 6f;
        [SerializeField] private float obstructedSmoothTime = 0.05f;

        private Vector3 localDirection;
        private float currentDistance;
        private float distanceVelocity;

        private void Awake()
        {
            if(pivot == null){
                pivot = transform.parent;
            }

            if(pivot == null)
            {
                Debug.LogError($"{nameof(ThirdPersonCameraHandler)} needs a pivot or a parent.", this);
                enabled = false;
                return;
            }

            Vector3 localOffset = pivot.InverseTransformPoint(transform.position);

            if(maxDistance <= 0f){
                maxDistance = localOffset.magnitude;
            }

            localDirection = localOffset.sqrMagnitude > 0.0001f ? localOffset.normalized : Vector3.back;
            currentDistance = maxDistance;
        }

        private void LateUpdate()
        {
            Vector3 origin = pivot.position;
            Vector3 direction = pivot.TransformDirection(localDirection);

            float targetDistance = GetAllowedDistance(origin, direction);

            if(targetDistance < currentDistance)
            {
                currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref distanceVelocity, obstructedSmoothTime);
            }
            else
            {
                distanceVelocity = 0f;
                currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, returnSpeed * Time.deltaTime);
            }

            transform.position = origin + direction * currentDistance;
        }

        private float GetAllowedDistance(Vector3 origin, Vector3 direction)
        {
            if(Physics.SphereCast(origin, cameraRadius, direction, out RaycastHit hit, maxDistance, obstructionLayers, QueryTriggerInteraction.Ignore)){
                return Mathf.Clamp(hit.distance - surfacePadding, minDistance, maxDistance);
            }

            return maxDistance;
        }
    }
}