using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDebugger : MonoBehaviour
{
    private Quaternion _rotation;

    
    void Awake()
    {
        _rotation = transform.rotation;
        Debug.Log("Initial Rotation: " + _rotation.eulerAngles.ToString(), gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_rotation != transform.rotation)
        {
            Debug.Log("Rotation changed from: " + _rotation.eulerAngles.ToString() + " to: " + transform.rotation.eulerAngles.ToString(), gameObject);
            _rotation = transform.rotation;
            
        }
    }
}
