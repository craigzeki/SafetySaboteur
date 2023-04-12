using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthBar))]
public class GUISkill : MonoBehaviour
{
    [SerializeField] private float _startPercentage = 0f;
    [SerializeField] private Image _image;
    [SerializeField] private Color _enabledColor;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private bool _enabledAtStart = false;

    private HealthBar _healthBar ;

    private void Awake()
    {
        _healthBar = GetComponent<HealthBar>();
    }

    void Start()
    {
        SetEnabled(_enabledAtStart);
    }

    public void SetEnabled(bool enabled)
    {
        
        _healthBar.SetHealthPercent(_startPercentage, false);
        _healthBar.HealthObject.SetActive(enabled);
        if(_image != null) _image.color = enabled ? _enabledColor : _disabledColor;

    }

}
