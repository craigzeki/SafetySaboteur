using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicScannerChild : MonoBehaviour
{
    private SonicScannerParent _parentScanner;

    private void Awake()
    {
        _parentScanner = GetComponentInParent<SonicScannerParent>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (_parentScanner == null) return;

        if (other.gameObject.TryGetComponent<iTakesDamage>(out iTakesDamage damageObject))
        {
            if (!_parentScanner.TargetAquired || (_parentScanner.TargetAquired && (_parentScanner.TargetID == other.gameObject.GetInstanceID())))
            {
                _parentScanner.TargetPoints.Add(new TargetPoint(other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), other.gameObject.GetInstanceID(), damageObject));
            }

        }
    }
}
