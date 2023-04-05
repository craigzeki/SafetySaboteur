using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpeedAdjuster : MonoBehaviour
{
    private DroneManager _otherDroneMovement;
    private DroneManager _myDroneMovement;

    private void Awake()
    {
        if(_myDroneMovement == null)
        {
            _myDroneMovement = GetComponentInParent<DroneManager>();
        }
    }
    private void FixedUpdate()
    {
        if (_myDroneMovement == null) return;

        int layerMask = 1 << 7; //Drones layer
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1, layerMask))
        {
            if (hit.transform.TryGetComponent<DroneManager>(out _otherDroneMovement))
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
