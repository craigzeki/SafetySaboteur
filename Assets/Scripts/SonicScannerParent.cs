using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicScannerParent : MonoBehaviour
{
    

    private const int INVALID_ID = -1;

    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private bool _targetAquired = false;
    [SerializeField] private int _targetID = INVALID_ID;

    public List<TargetPoint> TargetPoints = new List<TargetPoint>();
    private float _closestDistance = 0f;
    private float _pointDistance = 0f;

    public bool TargetAquired { get => _targetAquired; }
    public int TargetID { get => _targetID; }

    void Update()
    {

        if(TargetPoints.Count == 0)
        {
            _targetAquired = false;
            _targetID = INVALID_ID;
            return;
        }

        
        _closestDistance = 10000f;
        foreach (TargetPoint point in TargetPoints)
        {
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

    private void FixedUpdate()
    {
        TargetPoints.Clear();
    }


}

public class TargetPoint
{
    public Vector3 Position;
    public int UniqueID;
    public iTakesDamage damageInterface;

    public TargetPoint(Vector3 position, int uniqueID, iTakesDamage damageInterface)
    {
        Position = position;
        UniqueID = uniqueID;
        this.damageInterface = damageInterface ?? throw new ArgumentNullException(nameof(damageInterface));
    }
}