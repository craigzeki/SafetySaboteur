using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SonicRotator : MonoBehaviour, iTowerRotator
{

    [SerializeField] private Transform _aimer;
    [SerializeField] private Transform _xJoint;
    [SerializeField] private Transform _yJoint;
    [SerializeField] private Transform _zJoint;
    [SerializeField] private float _xSpeed = 1f;
    [SerializeField] private float _ySpeed = 1f;
    [SerializeField] private float _zSpeed = 1f;
    [SerializeField] private float _tolerance = 0.1f;
    

    private Vector3 _targetDirection = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;
    private Vector3 _rotationDelta = Vector3.zero;
    private Vector3 _tempRotation;

    private bool _isBroken = false;

    public void DoSabotage()
    {
        _isBroken = true;
    }

    public void RotateTowards(Vector3 direction)
    {
        
        _targetDirection = direction;
    }

    public void UpdateRotator()
    {
        if (_isBroken) _targetDirection = Vector3.down;
       
        _targetRotation = Quaternion.LookRotation(_targetDirection, Vector3.up);
        _rotationDelta = _targetRotation.eulerAngles - _aimer.rotation.eulerAngles;

        //shortest rotation delta = (T?C + 540°)mod360°?180° from: https://math.stackexchange.com/questions/110080/shortest-way-to-achieve-target-angle

        _rotationDelta.x = ((_targetRotation.eulerAngles.x - _aimer.rotation.eulerAngles.x + 540) % 360) - 180;
        _rotationDelta.y = ((_targetRotation.eulerAngles.y - _aimer.rotation.eulerAngles.y + 540) % 360) - 180;
        _rotationDelta.z = ((_targetRotation.eulerAngles.z - _aimer.rotation.eulerAngles.z + 540) % 360) - 180;

        if (Mathf.Abs(_rotationDelta.x) > _tolerance)
        {
            //rotate down
            _tempRotation = _xJoint.rotation.eulerAngles;
            _tempRotation.x += Mathf.Sign(_rotationDelta.x) * _xSpeed * Time.deltaTime;
            _xJoint.rotation = Quaternion.Euler(_tempRotation);
        }

        if (Mathf.Abs(_rotationDelta.y) > _tolerance)
        {
            //rotate down
            _tempRotation = _yJoint.rotation.eulerAngles;
            _tempRotation.y += Mathf.Sign(_rotationDelta.y) * _ySpeed * Time.deltaTime;
            _yJoint.rotation = Quaternion.Euler(_tempRotation);
            
        }

        if (Mathf.Abs(_rotationDelta.z) > _tolerance)
        {
            //rotate down
            _tempRotation = _zJoint.rotation.eulerAngles;
            _tempRotation.z += Mathf.Sign(_rotationDelta.z) * _xSpeed * Time.deltaTime;
            _zJoint.rotation = Quaternion.Euler(_tempRotation);
        }

    }

}
