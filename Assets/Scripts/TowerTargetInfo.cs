using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PublicConsts;

[Serializable]
public class TowerTargetInfo
{
    [SerializeField] public bool TargetAquired = false;
    [SerializeField] public bool TargetInRange = false;
    [SerializeField] public bool TargetInView = false;
    [SerializeField] public Vector3 TargetObjectPosition = Vector3.zero;
    [SerializeField] public Vector3 TargetHitPosition = Vector3.zero;
    [SerializeField] public Vector3 DirectionToTarget = Vector3.zero;
    [SerializeField] public int TargetUniqueID = INVALID_ID;
    [SerializeField] public iTakesDamage DamageReceiver;
}
