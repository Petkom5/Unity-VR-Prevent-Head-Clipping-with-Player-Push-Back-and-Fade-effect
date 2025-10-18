using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script handles head collisions by detecting nearby obstacles and pushing the character back.
// Place this script in a new GameObject under the MainCamera and Set the necessary references in the inspector.
// This Script is an alternative to the HeadCollisionHandler + HeadCollisionDetector Scripts in the main project by using the camera transform for detection and checking for additional directions.
public class HeadCollisionHandler : MonoBehaviour
{
    [SerializeReference] private CharacterController _characterController;
    [SerializeReference] private Transform _cameraTransform;
    [SerializeField] private float pushBackStrength = 1.0f;
    [SerializeField, Range(0, 0.5f)] private float _detectionDelay = 0.05f;

    [SerializeField] private float _detectionDistance = 0.2f;
    [SerializeField] private LayerMask _detectionLayers;

    private float _currentTime = 0;

    void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > _detectionDelay)
        {
            _currentTime = 0;

            List<RaycastHit> detectedColliderHits = PerformDetection(_cameraTransform.position, _detectionDistance, _detectionLayers);
            if (detectedColliderHits.Count <= 0)
            {
                return;
            }
            Vector3 pushBackDirection = CalculatePushBackDirection(detectedColliderHits);

            _characterController.Move(pushBackStrength * Time.deltaTime * pushBackDirection.normalized);
        }
    }

    private List<RaycastHit> PerformDetection(Vector3 position, float distance, LayerMask mask)
    {
        List<RaycastHit> detectedHits = new();
        List<Vector3> directions = new() { transform.forward, -transform.forward, transform.right, -transform.right, transform.up, -transform.up };

        foreach (var dir in directions)
        {
            if (Physics.Raycast(position, dir, out RaycastHit hit, distance, mask))
            {
                detectedHits.Add(hit);
            }
        }
        return detectedHits;
    }

    private Vector3 CalculatePushBackDirection(List<RaycastHit> colliderHits)
    {
        Vector3 combinedNormal = Vector3.zero;
        foreach (RaycastHit hitPoint in colliderHits)
        {
            combinedNormal += new Vector3(hitPoint.normal.x, 0, hitPoint.normal.z);
        }
        return combinedNormal;
    }
}