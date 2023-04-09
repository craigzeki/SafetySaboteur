using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicScannerChild : MonoBehaviour
{
    [SerializeField] private Transform _maxPoint;
    [SerializeField] private Transform _closestPoint;
    [SerializeField] private float _dampingTime = 0.1f;
    [SerializeField] private float _fastDampingTime = 0.05f;
    [SerializeField] private float _clipThreshold = 0.3f;
    [SerializeField] private int _inactiveThreshold = 5;

    private SonicScannerParent _parentScanner;

    private float _maxRange = 0f;
    private float _currentRange = 0f;
    private Vector3 _maxScale = Vector3.one;
    private Vector3 _newScale = Vector3.one;
    private Vector3 _dampingVelocity = Vector3.one;
    
    TargetPoint _targetPoint;

    private List<TargetPoint> _allColliders = new List<TargetPoint>();
    private Dictionary<int, TargetPoint> _targetPoints = new Dictionary<int, TargetPoint>();

    private void Awake()
    {
        _maxScale = transform.localScale;
        _parentScanner = GetComponentInParent<SonicScannerParent>();
        if (_maxPoint == null) return;
        _currentRange = _maxRange = Vector3.Distance(transform.position, _maxPoint.position);
        
    }

    private void Update()
    {
        //Debug.Log("Update: targetpoints: " + _allColliders.Count.ToString());
        if (_maxPoint == null) return;

        //_targetPoint = GetClosestPoint(_allColliders, transform.position, out float distance);
        _targetPoint = GetClosestPoint(_targetPoints, transform.position, out float distance);

        //Debug.Log("distance: " + distance.ToString());
        //Debug.Log("_clipThreshold: " + _clipThreshold.ToString());
        distance = Mathf.Clamp((distance + _clipThreshold), distance, _maxRange);
        //Debug.Log("Clamped distance: " + distance.ToString());
        _newScale = _maxScale;
        if(_targetPoint != null)
        {
            _closestPoint.position = _targetPoint.Position;
            _newScale.z = _maxScale.z * (distance / _maxRange);
            
        }
        //else
        //{
        //    _newScale = _maxScale;
        //}

        if(_currentRange > distance)
        {
            //snap to new distance
            transform.localScale = Vector3.SmoothDamp(transform.localScale, _newScale, ref _dampingVelocity, _fastDampingTime);
            //transform.localScale = _newScale;
            _currentRange = Vector3.Distance(transform.position, _maxPoint.position);

            //transform.localScale = _newScale;
            //_currentRange = distance;
        }
        else if(_currentRange < distance)
        {
            //lerp back out
            transform.localScale = Vector3.SmoothDamp(transform.localScale, _newScale, ref _dampingVelocity, _dampingTime);
            //transform.localScale = _newScale;
            _currentRange = Vector3.Distance(transform.position, _maxPoint.position);
        }
    }

    //private TargetPoint GetClosestPoint(List<TargetPoint> _points, Vector3 origin, out float distanceToPoint)
    //{
    //    if (_points == null) { distanceToPoint = _maxRange; return null; }
    //    if (_points.Count == 0) { distanceToPoint = _maxRange; return null; }

    //    float closestDistance = _maxRange;
    //    float currentDistance = _maxRange;
    //    TargetPoint closestPoint = null;

    //    foreach(TargetPoint point in _points)
    //    {
    //        currentDistance = Vector3.Distance(origin, point.Position);
    //        if(currentDistance < closestDistance)
    //        {
    //            closestDistance = currentDistance;
    //            closestPoint = point;
    //        }
    //    }
    //    distanceToPoint = closestDistance;
    //    return closestPoint;
    //}

    private TargetPoint GetClosestPoint(Dictionary<int, TargetPoint> _points, Vector3 origin, out float distanceToPoint)
    {
        if (_points == null) { distanceToPoint = _maxRange; return null; }
        if (_points.Count == 0) { distanceToPoint = _maxRange; return null; }

        float closestDistance = _maxRange;
        float currentDistance = _maxRange;
        TargetPoint closestPoint = null;

        foreach (TargetPoint point in _points.Values)
        {
            if (!point.IsActive) continue;
            currentDistance = Vector3.Distance(origin, point.Position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                closestPoint = point;
            }
        }
        distanceToPoint = closestDistance;
        return closestPoint;
    }

    private void FixedUpdate()
    {
        //_allColliders.Clear();
        //Debug.Log("FixedUpdate: targetpoints cleared: " + _allColliders.Count.ToString());
        foreach (TargetPoint point in _targetPoints.Values)
        {
            point.InactiveCount = Mathf.Clamp(point.InactiveCount + 1, 0, _inactiveThreshold);
            if (point.InactiveCount >= _inactiveThreshold)
            {
                point.IsActive = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_parentScanner == null) return;

        if (other.gameObject.TryGetComponent<iTakesDamage>(out iTakesDamage damageObject))
        {
            //if (!_parentScanner.TargetAquired || (_parentScanner.TargetAquired && (_parentScanner.TargetID == other.gameObject.GetInstanceID())))
            //{
            //    _parentScanner.TargetPoints.Add(new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), other.gameObject.GetInstanceID(), damageObject));
                
            //}
            if(_parentScanner.TargetPointsDict.TryGetValue(other.gameObject.GetInstanceID(), out TargetPoint targetPoint))
            {
                targetPoint.InactiveCount = 0;
                targetPoint.IsActive = true;
                targetPoint.Position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
                targetPoint.damageInterface = damageObject;
            }
            else
            {
                _parentScanner.TargetPointsDict.Add(other.gameObject.GetInstanceID(), new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPoint(transform.position), other.gameObject.GetInstanceID(), damageObject));
            }
        }
        
        //_allColliders.Add(new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), other.gameObject.GetInstanceID(), damageObject));
        if(_targetPoints.TryGetValue(other.gameObject.GetInstanceID(), out TargetPoint tp))
        {
            tp.InactiveCount = 0;
            tp.IsActive = true;
            tp.Position = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            tp.damageInterface = damageObject;
        }
        else
        {
            _targetPoints.Add(other.gameObject.GetInstanceID(), new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPoint(transform.position), other.gameObject.GetInstanceID(), damageObject));
        }
        Debug.Log("OnTriggerStay: targetpoints: " + _allColliders.Count.ToString());
    }
}
