using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class SwitchCamOnTargetDistance : MonoBehaviour
{
    [SerializeField] private float cameraDistance;
    [SerializeField] private float targetDistance;
    [SerializeField] private float targetAngle;
    [SerializeField] private float targetDistanceH;
    [SerializeField] private float targetDistancePercentage;
    [SerializeField] private float distanceTriggerPercentage;
    [SerializeField] private CinemachineVirtualCamera nextVCam;

    private Transform targetTransform;
    private CinemachineVirtualCamera thisVCam;
    private bool blending = false;

    private const int HIGH_PRIORITY = 99;
    private const int LOW_PRIORITY = 0;

    private void Awake()
    {
        thisVCam = GetComponent<CinemachineVirtualCamera>();
        UpdateTarget();
        
    }

    private void UpdateTarget()
    {

        if (thisVCam != null) targetTransform = thisVCam.Follow;
    }

    void Update()
    {
        if (thisVCam == null) return;
        if (targetTransform == null) return;
        if (!CinemachineCore.Instance.IsLive(thisVCam)) { blending = false; return; }
        if (blending) return;

        cameraDistance = Vector3.Distance(transform.position, nextVCam.transform.position);
        targetDistance = Vector3.Distance(transform.position, targetTransform.position);
        targetAngle = transform.rotation.eulerAngles.x;
        targetDistanceH = targetDistance * Mathf.Cos(Mathf.Deg2Rad * targetAngle);
        targetDistancePercentage = targetDistanceH / cameraDistance;
        if (targetDistancePercentage < distanceTriggerPercentage)
        {
            blending = true;
            nextVCam.Priority = HIGH_PRIORITY;
            thisVCam.Priority = LOW_PRIORITY;
        }
        
    }
}
