using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpeedAdjuster : MonoBehaviour
{
    private DroneMovement _otherDroneMovement;
    private DroneMovement _myDroneMovement;

    private void Awake()
    {
        if(_myDroneMovement == null)
        {
            _myDroneMovement = GetComponentInParent<DroneMovement>();
        }
    }
    private void FixedUpdate()
    {
        if (_myDroneMovement == null) return;

        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, layerMask))
        {
            if (hit.transform.TryGetComponent<DroneMovement>(out _otherDroneMovement))
            {
                _myDroneMovement.Speed = Mathf.Lerp(_myDroneMovement.Speed, _otherDroneMovement.Speed, (1 - hit.distance));
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1, Color.white);
        }
    }

}
