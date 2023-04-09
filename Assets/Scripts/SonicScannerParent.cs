using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using static PublicConsts;
public class SonicScannerParent : MonoBehaviour
{ 
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private bool _targetAquired = false;
    [SerializeField] private int _targetID = INVALID_ID;
    [SerializeField] private int _inactiveThreshold = 5;

    public List<TargetPoint> TargetPoints = new List<TargetPoint>();
    public Dictionary<int, TargetPoint> TargetPointsDict = new Dictionary<int, TargetPoint>();

    private float _closestDistance = 0f;
    private float _pointDistance = 0f;

    public bool TargetAquired { get => _targetAquired; }
    public int TargetID { get => _targetID; }

    void Update()
    {

        if(TargetPointsDict.Count == 0)
        {
            _targetAquired = false;
            _targetID = INVALID_ID;
        }
        else
        {
            if(_targetAquired && (_targetID != INVALID_ID))
            {
                if(TargetPointsDict.TryGetValue(_targetID, out TargetPoint targetPoint))
                {
                    if(targetPoint.IsActive)
                    {
                        _targetPoint.position = targetPoint.Position;
                    }
                    else
                    {
                        _targetID = INVALID_ID;
                        _targetAquired = false;
                    }
                    
                }
                else
                {
                    _targetID = INVALID_ID;
                    _targetAquired = false;
                }
            }
            else
            {
                _closestDistance = 10000f;
                foreach (TargetPoint point in TargetPointsDict.Values)
                {
                    if (!point.IsActive) continue;
                    _pointDistance = Vector3.Distance(_gunPoint.position, point.Position);
                    if (_pointDistance < _closestDistance)
                    {
                        _closestDistance = _pointDistance;
                        _targetPoint.position = point.Position;
                        _targetID = point.UniqueID;
                        _targetAquired = true;
                    }

                }
            }
        }

        //if(TargetPoints.Count == 0)
        //{
        //    _targetAquired = false;
        //    _targetID = INVALID_ID;
        //    return;
        //}

        
        //_closestDistance = 10000f;

        //if((_targetID != INVALID_ID) && _targetAquired)
        //{

        //}

        //foreach (TargetPoint point in TargetPoints)
        //{
        //    _pointDistance = Vector3.Distance(_gunPoint.position, point.Position);
        //    if (_pointDistance < _closestDistance)
        //    {
        //        _closestDistance = _pointDistance;
        //        _targetPoint.position = point.Position;
        //        _targetID = point.UniqueID;
        //        _targetAquired = true;
        //    }

        //}
        
        
    }

    private void FixedUpdate()
    {
        //TargetPoints.Clear();
        foreach(TargetPoint point in TargetPointsDict.Values)
        {
            point.InactiveCount = Mathf.Clamp(point.InactiveCount + 1, 0, _inactiveThreshold);
            if(point.InactiveCount >= _inactiveThreshold)
            {
                point.IsActive = false;
            }
        }
    }


}

