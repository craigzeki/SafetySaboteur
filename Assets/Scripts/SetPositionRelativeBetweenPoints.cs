using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SetPositionRelativeBetweenPoints : MonoBehaviour
{
    [SerializeField] private Transform _startPos;
    [SerializeField] private Transform _endPos;
    [SerializeField] private Vector3 _ratios = Vector3.zero;

    private Vector3 _position;
    
    void Update()
    {
        //calculate the new positon based on ratios given
        _position = _endPos.localPosition - _startPos.localPosition;
        _position.Scale(_ratios);
        transform.localPosition = _startPos.localPosition + _position;
    }
}
