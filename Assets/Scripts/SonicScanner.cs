
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
    [SerializeField] private TowerTargetInfo _towerTargetInfo = new TowerTargetInfo();
    [SerializeField] private TowerTargetInfo _newTargetInfo = new TowerTargetInfo();

    private bool _limitedFoV = false;

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
                        SetHitPointInRange(ref _towerTargetInfo, Color.red);
                    }
                    else
                    {
                        _closestDistance = 10000f;
                        
                        _newTargetInfo = new TowerTargetInfo();
                        //_closestDistance = Vector3.Distance(_gunPoint.position, _towerTargetInfo.TargetHitPosition);
                        foreach (TargetPoint newPoint in _targetPoints.Values)
                        {
                            if (!newPoint.IsActive) continue;
                            if (_towerTargetInfo.TargetUniqueID == newPoint.UniqueID) continue; //skip target that can't be hit
                            _pointDistance = Vector3.Distance(_gunPoint.position, newPoint.Position);
                            if (_pointDistance < _closestDistance)
                            {
                                
                                _closestDistance = _pointDistance;
                                _newTargetInfo.TargetObjectPosition = _targetObjectPosition.position = newPoint.Position;

                                _newTargetInfo.TargetUniqueID = newPoint.UniqueID;
                                _newTargetInfo.TargetAquired = true;
                                _newTargetInfo.DamageReceiver = newPoint.damageInterface;
                            }
                        }

                        //now update the current target info so that it can be compared with
                        if (_targetPoints.TryGetValue(_towerTargetInfo.TargetUniqueID, out TargetPoint oldPoint))
                        {

                            _towerTargetInfo.TargetObjectPosition = _targetObjectPosition.position = oldPoint.Position;
                            _towerTargetInfo.TargetUniqueID = oldPoint.UniqueID;
                            _towerTargetInfo.TargetAquired = true;
                            _towerTargetInfo.DamageReceiver = oldPoint.damageInterface;
                            
                        }

                        SetHitPointInRange(ref _towerTargetInfo, Color.red);

                        if (_newTargetInfo.TargetUniqueID != INVALID_ID) //we haven't found anything closer - so now need to update the data for this object as it was skipped in loop
                        {
                            SetHitPointInRange(ref _newTargetInfo, Color.green);
                            if (_towerTargetInfo.Priority < _newTargetInfo.Priority) _towerTargetInfo = _newTargetInfo;
                        }
                        
                    }
                    

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

                SetHitPointInRange(ref _towerTargetInfo, Color.red);
            }
        }
    }

    

    private void SetHitPointInRange(ref TowerTargetInfo targetInfo, Color debugColor)
    {
        int targetLayerMask;

        if (_limitedFoV)
        {
            targetLayerMask = LayerMask.GetMask("Player");
        }
        else
        {
            targetLayerMask = LayerMask.GetMask("Drones", "Player");
        }
        
        int layerMask = ~LayerMask.GetMask("Towers"); //everything but the tower itself
        targetInfo.DirectionToTarget = (targetInfo.TargetObjectPosition - _gunPoint.position).normalized;
        targetInfo.Priority = 0;
        Vector3 samePlaneDirection = (new Vector3(targetInfo.TargetObjectPosition.x, _gunPoint.position.y, targetInfo.TargetObjectPosition.z) - _gunPoint.position).normalized;
        //check field of view
        if (Vector3.Angle(_gunPoint.forward, samePlaneDirection) > _fieldOfViewDegrees)
        {
            Debug.Log("Out of view");
            targetInfo.TargetInView = false;
        }
        else
        {
            targetInfo.TargetInView = true;
            targetInfo.Priority++;
        }

        Ray ray = new Ray(_gunPoint.position, targetInfo.DirectionToTarget);
        Debug.DrawRay(_gunPoint.position, targetInfo.DirectionToTarget * _range, Color.red, 0.5f);
        if(Physics.Raycast(ray, out RaycastHit hit, _range, layerMask, QueryTriggerInteraction.Ignore))
        {
            if(((1 << hit.transform.gameObject.layer) & targetLayerMask) > 0)
            {
                targetInfo.TargetHitPosition = _targetPoint.position = hit.point;
                targetInfo.TargetInRange = true;
                targetInfo.Priority++;
            }
            else
            {
                //hidden behind something
                targetInfo.TargetInRange = false;
                targetInfo.TargetInView = false;
                targetInfo.Priority = 0;
            }
            
        }
        else
        {
            //_towerTargetInfo.TargetHitPosition = _targetPoint.position = _gunPoint.position;
            targetInfo.TargetInRange = false;
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

    public void DoSabotage()
    {
        _limitedFoV = true;
    }
}
