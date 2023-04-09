
using System.Collections.Generic;

using UnityEngine;
using static PublicConsts;

public class SonicScanner : MonoBehaviour, iTowerScanner
{
    [SerializeField] private Transform _targetObjectPosition;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private float _range = 7f;
    [SerializeField] private float _fieldOfViewDegrees = 40f;
    [SerializeField] private int _inactiveThreshold = 5;

    private Dictionary<int, TargetPoint> _targetPoints = new Dictionary<int, TargetPoint>();
    private List<int> _inactivePoints = new List<int>();

    private float _closestDistance = 0f;
    private float _pointDistance = 0f;
    private TowerTargetInfo _towerTargetInfo = new TowerTargetInfo();


    public void ScannerUpdate()
    {

        if (_targetPoints.Count == 0)
        {
            _towerTargetInfo.TargetUniqueID = INVALID_ID;
            _towerTargetInfo.TargetInRange = false;
            _towerTargetInfo.TargetAquired = false;
        }
        else
        {
            if (_towerTargetInfo.TargetUniqueID != INVALID_ID)
            {
                if (_targetPoints.TryGetValue(_towerTargetInfo.TargetUniqueID, out TargetPoint targetPoint))
                {
                    if(_towerTargetInfo.TargetInView && _towerTargetInfo.TargetInRange)
                    {
                        _towerTargetInfo.TargetObjectPosition = _targetObjectPosition.position = targetPoint.Position;
                        
                    }
                    else
                    {
                        _closestDistance = 10000f;
                        int prevID = _towerTargetInfo.TargetUniqueID;
                        foreach (TargetPoint point in _targetPoints.Values)
                        {
                            if (!point.IsActive) continue;
                            if (_towerTargetInfo.TargetUniqueID == point.UniqueID) continue; //skip target that can't be hit
                            _pointDistance = Vector3.Distance(_gunPoint.position, point.Position);
                            if (_pointDistance < _closestDistance)
                            {
                                _closestDistance = _pointDistance;
                                _towerTargetInfo.TargetObjectPosition = _targetObjectPosition.position = point.Position;

                                _towerTargetInfo.TargetUniqueID = point.UniqueID;
                                _towerTargetInfo.TargetAquired = true;
                                _towerTargetInfo.DamageReceiver = point.damageInterface;
                            }
                        }
                        if(_towerTargetInfo.TargetUniqueID == prevID) //we haven't found anything closer - so now need to update the data for this object as it was skipped in loop
                        {
                            if(_targetPoints.TryGetValue(prevID, out TargetPoint point))
                            {
                                
                                _towerTargetInfo.TargetObjectPosition = _targetObjectPosition.position = point.Position;
                                _towerTargetInfo.TargetUniqueID = point.UniqueID;
                                _towerTargetInfo.TargetAquired = true;
                                _towerTargetInfo.DamageReceiver = point.damageInterface;
                            }
                            
                        }
                    }
                    SetHitPointInRange(_targetObjectPosition.position);

                }
                else
                {
                    _towerTargetInfo.TargetUniqueID = INVALID_ID;
                    _towerTargetInfo.TargetInRange = false;
                    _towerTargetInfo.TargetAquired = false;
                }
            }
            else
            {
                _closestDistance = 10000f;
                foreach (TargetPoint point in _targetPoints.Values)
                {
                    if (!point.IsActive) continue;
                    _pointDistance = Vector3.Distance(_gunPoint.position, point.Position);
                    if (_pointDistance < _closestDistance)
                    {
                        _closestDistance = _pointDistance;
                        _towerTargetInfo.TargetObjectPosition = _targetObjectPosition.position = point.Position;
                        
                        _towerTargetInfo.TargetUniqueID = point.UniqueID;
                        _towerTargetInfo.TargetAquired = true;
                        _towerTargetInfo.DamageReceiver = point.damageInterface;
                    }
                }

                SetHitPointInRange(_targetObjectPosition.position);
            }
        }
    }

    private void SetHitPointInRange(Vector3 objectPoint)
    {
        Vector3 hitPoint = objectPoint;
        
        int targetLayerMask = LayerMask.GetMask("Drones", "Player");
        int layerMask = ~0; //everything
        _towerTargetInfo.DirectionToTarget = (objectPoint - _gunPoint.position).normalized;

        Vector3 samePlaneDirection = (new Vector3(objectPoint.x, _gunPoint.position.y, objectPoint.z) - _gunPoint.position).normalized;
        //check field of view
        if (Vector3.Angle(_gunPoint.forward, samePlaneDirection) > _fieldOfViewDegrees)
        {
            Debug.Log("Out of view");
            _towerTargetInfo.TargetInView = false;
        }
        else
        {
            _towerTargetInfo.TargetInView = true;
        }

        Ray ray = new Ray(_gunPoint.position, _towerTargetInfo.DirectionToTarget);
        Debug.DrawRay(_gunPoint.position, _towerTargetInfo.DirectionToTarget * _range, Color.red);
        if(Physics.Raycast(ray, out RaycastHit hit, _range, layerMask, QueryTriggerInteraction.Ignore))
        {
            if(((1 << hit.transform.gameObject.layer) & targetLayerMask) > 0)
            {
                _towerTargetInfo.TargetHitPosition = _targetPoint.position = hit.point;
                _towerTargetInfo.TargetInRange = true;
            }
            else
            {
                _towerTargetInfo.TargetInRange = false;
            }
            
        }
        else
        {
            //_towerTargetInfo.TargetHitPosition = _targetPoint.position = _gunPoint.position;
            _towerTargetInfo.TargetInRange = false;
        }

    }

    public TowerTargetInfo GetTowerTargetInfo()
    {
        return _towerTargetInfo;
    }

    private void FixedUpdate()
    {
        _inactivePoints.Clear();

        foreach (TargetPoint point in _targetPoints.Values)
        {
            point.InactiveCount = Mathf.Clamp(point.InactiveCount + 1, 0, _inactiveThreshold);
            if (point.InactiveCount >= _inactiveThreshold)
            {
                _inactivePoints.Add(point.UniqueID);
            }
        }

        foreach(int id in _inactivePoints)
        {
            _targetPoints.Remove(id);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<iTakesDamage>(out iTakesDamage damageObject))
        {
            if (_targetPoints.TryGetValue(other.gameObject.GetInstanceID(), out TargetPoint targetPoint))
            {
                targetPoint.InactiveCount = 0;
                targetPoint.Position = other.gameObject.transform.position;
                //overwrite position if it is the player as player position is on the ground plane - need to elevate
                if (other.tag == "Player")
                {
                    if(other.gameObject.TryGetComponent<Saboteur>(out Saboteur saboteur))
                    {
                        if(saboteur.PlayerFakeOrigin != null)
                        {
                            targetPoint.Position = saboteur.PlayerFakeOrigin.position;
                        }
                    }
                }
                
                targetPoint.damageInterface = damageObject;
            }
            else
            {
                _targetPoints.Add(other.gameObject.GetInstanceID(), new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPoint(transform.position), other.gameObject.GetInstanceID(), damageObject));
            }
        }
    }
}
