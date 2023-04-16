using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneSpeedAdjuster : MonoBehaviour
{
    private DroneManager _otherDroneMovement;
    private DroneManager _myDroneMovement;
    private GameObject _otherDrone;

    private float _startSpeed;
    private float _velocity;

    private void Awake()
    {
        if(_myDroneMovement == null)
        {
            _myDroneMovement = GetComponentInParent<DroneManager>();
            _startSpeed = _myDroneMovement.Speed;
        }
    }
    private void FixedUpdate()
    {
        if (_myDroneMovement == null) return;

        int layerMask = 1 << 7; //Drones layer
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.5f, layerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.transform.TryGetComponent<DroneManager>(out _otherDroneMovement))
            {
                _otherDrone = _otherDroneMovement.gameObject;
                //_myDroneMovement.Speed = Mathf.Lerp(_startSpeed, _otherDroneMovement.Speed, Mathf.Abs( 1 - hit.distance));
                _myDroneMovement.Speed = _otherDroneMovement.Speed;
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);
        }
        else
        {
            if(_otherDrone.IsUnityNull())
            {
                _myDroneMovement.Speed = Mathf.SmoothDamp(_myDroneMovement.Speed, _startSpeed, ref _velocity, 1f);
            }
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1.5f, Color.white);
        }
    }

}
