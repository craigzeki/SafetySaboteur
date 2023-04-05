using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image _image;
    [SerializeField] float _smoothTime = 0.3f;
    [SerializeField] float _targetTolerance = 0.001f;
    [ColorUsage(true, true)]
    [SerializeField] Color _maxColour;
    [ColorUsage(true, true)]
    [SerializeField] Color _minColour;
    [SerializeField] float _colourLerpStartPercent = 0.8f;
    [SerializeField] float _colourLerpEndPercent = 0.2f;
    private float _maxScale = 1;
    [SerializeField] private float _targetScale = 1f;
    private Vector3 _newScale = Vector3.one;
    private float _smoothVelocity = 0f;
    private float _currentPercentage = 1f;
    private Color _newColor;
    private Material _material;

    public event EventHandler<float> OnTargetHealthReached;

    private void Awake()
    {
        
        _newScale = transform.localScale;
        _targetScale = _maxScale = transform.localScale.x;
        if(_colourLerpStartPercent <= _colourLerpEndPercent)
        {
            _colourLerpEndPercent = 0f;
            _colourLerpStartPercent = 1f;
        }
        if(_image != null)
        {
            _material = _image.material;
        }
        
    }

    void Update()
    {
        transform.LookAt(Camera.main.transform);
        _newScale.x = Mathf.SmoothDamp(transform.localScale.x, _targetScale, ref _smoothVelocity, _smoothTime);
        
        if(_newScale.x - _targetScale <= _targetTolerance)
        {
            _newScale.x = _targetScale;
            TargetHealthPercentReached(_targetScale);
        }
        transform.localScale = _newScale;
        _currentPercentage = _newScale.x / _maxScale;
        if((_currentPercentage < _colourLerpStartPercent) && (_currentPercentage > _colourLerpEndPercent))
        {
            _newColor = Color.Lerp(_minColour, _maxColour, ((_currentPercentage - _colourLerpEndPercent) / (_colourLerpStartPercent - _colourLerpEndPercent)));
        }
        else if(_currentPercentage <= _colourLerpEndPercent)
        {
            _newColor = _minColour;
        }
        else if(_currentPercentage > _colourLerpStartPercent)
        {
            _newColor = _maxColour;
        }
        _material.SetColor("_EmissionColor", _newColor);
        
    }

    public void SetHealthPercent(float percent)
    {
        _targetScale = Mathf.Clamp(percent, 0f, 1f) * _maxScale;
    }

    protected virtual void TargetHealthPercentReached(float healthPercent)
    {
        OnTargetHealthReached?.Invoke(this, healthPercent);
    }
}
