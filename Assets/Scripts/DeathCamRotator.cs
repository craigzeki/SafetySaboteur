using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamRotator : MonoBehaviour
{
    [SerializeField] private Transform _followPosition;
    [SerializeField] private float _rotationSpeed;
    // Update is called once per frame
    void Update()
    {
        transform.position = _followPosition.position;
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }
}
