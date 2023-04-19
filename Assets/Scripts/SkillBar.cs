using System;
using UnityEngine;

public class SkillBar : MonoBehaviour
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
    [SerializeField] bool _alwaysRotateToFaceCam = true;
    [SerializeField] private GameObject _skillObject;
    private float _maxScale = 1;
    [SerializeField] private float _targetScale = 1f;
    [SerializeField] private bool _enabledAtStart = false;
    [SerializeField] Canvas _canvas;
    private Vector3 _newScale = Vector3.one;
    private float _smoothVelocity = 0f;
    private float _currentPercentage = 1f;
    private Color _newColor;
    private Material _material;
    private bool _lerp = true;
    private bool _barDisabled = false;


    public GameObject SkillObject { get => _skillObject; }
    public bool GetBarDisabled { get => _barDisabled; }

    public event EventHandler<float> OnTargetSkillReached;
    public event EventHandler OnBarDisabled;

    private void Awake()
    {
        if (_skillObject == null) _skillObject = this.gameObject;
        _newScale = transform.localScale;
        _targetScale = _maxScale = transform.localScale.x;
        if (_colourLerpStartPercent <= _colourLerpEndPercent)
        {
            _colourLerpEndPercent = 0f;
            _colourLerpStartPercent = 1f;
        }
        if (_image != null)
        {
            //_material = _image.material;
            _material = new Material(_image.material);
            _image.material = _material;
        }

        SetEnabled(_enabledAtStart);
    }

    public void SetBarDisabled()
    {

        BarDisabled();
        gameObject.SetActive(false);
    }

    public void SetEnabled(bool enabled)
    {
        if (_barDisabled) return;
        if (_canvas != null) _canvas.enabled = enabled;
        
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GAME_STATE.PLAYING) return;
        if (_barDisabled) return;
        if (_alwaysRotateToFaceCam)
        {
            transform.LookAt(Camera.main.transform);
        }

        if (_lerp)
        {
            _newScale.x = Mathf.SmoothDamp(_skillObject.transform.localScale.x, _targetScale, ref _smoothVelocity, _smoothTime);
        }
        else
        {
            _newScale.x = _targetScale;
        }

        if (Mathf.Abs(_newScale.x - _targetScale) <= _targetTolerance)
        {
            _newScale.x = _targetScale;
            TargetSkillPercentReached(_targetScale);
        }
        _skillObject.transform.localScale = _newScale;
        _currentPercentage = _newScale.x / _maxScale;
        if ((_currentPercentage < _colourLerpStartPercent) && (_currentPercentage > _colourLerpEndPercent))
        {
            _newColor = Color.Lerp(_minColour, _maxColour, ((_currentPercentage - _colourLerpEndPercent) / (_colourLerpStartPercent - _colourLerpEndPercent)));
        }
        else if (_currentPercentage <= _colourLerpEndPercent)
        {
            _newColor = _minColour;
        }
        else if (_currentPercentage > _colourLerpStartPercent)
        {
            _newColor = _maxColour;
        }
        _material.SetColor("_EmissionColor", _newColor);

    }

    public void SetSkillPercent(float percent, bool lerp = true)
    {
        if (_barDisabled) return;
        _targetScale = Mathf.Clamp(percent, 0f, 1f) * _maxScale;
        _lerp = lerp;
        if (!lerp)
        {
            _newScale.x = _targetScale;
            _skillObject.transform.localScale = _newScale;
        }
    }

    protected virtual void TargetSkillPercentReached(float skillPercent)
    {
        OnTargetSkillReached?.Invoke(this, skillPercent);
    }

    protected virtual void BarDisabled()
    {
        _barDisabled = true;
        OnBarDisabled?.Invoke(this, EventArgs.Empty);
    }
}
