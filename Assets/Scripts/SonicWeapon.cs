using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SonicWeapon : MonoBehaviour, iTowerWeapon
{
    [SerializeField] private List<VisualEffect> _arcEffects = new List<VisualEffect>();
    [SerializeField] private List<VisualEffect> _originSparkEffects = new List<VisualEffect>();
    [SerializeField] private List<VisualEffect> _targetSparkEffects = new List<VisualEffect>();
    [SerializeField] private Transform _vfxStartPos;
    [SerializeField] private Transform _vfxEndPos;

    
    [SerializeField] private Transform _origin;
    [SerializeField] private Transform _target;
    [SerializeField] private float _projectileSpeed = 0.1f;

    [SerializeField] private AudioSource _weaponAudio;

    //defined at class scope instead of function scope to increase performance (no memory create / cleanup)
    private Coroutine _lerpFireCoR;
    private Coroutine _lerpStopFireCoR;
    private float _lerpDuration; //will be calculated based on distance and projectile speed
    private Vector3 _lerpStartPos = Vector3.zero; //fire point of weapon
    private Vector3 _lerpEndPos = Vector3.zero; //target pos
    private float _lerpTimeElapsed = 0; //counts the time in lerp
    private Vector3 _lerpNewPos = Vector3.zero; //new pos to calc
    private float _lerpProgress = 0; //progress between 0 and 1

    private bool _isFiring = false;
    private bool _isFiringComplete = false;

    
    
    private ZekiController.BUTTON_STATE _currentButtonState = ZekiController.BUTTON_STATE.UNKNOWN;
    private ZekiController.BUTTON_STATE _previousButtonState = ZekiController.BUTTON_STATE.UNKNOWN;
    private bool _switchFireTriggered = false;
    private bool _switchStopFireTriggered = false;
    private bool _switchFirstFrame = true;

    private Coroutine _firingCoroutine;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        StopAllEffects();
    }

    private void StartFiring()
    {
        _lerpStartPos = _vfxStartPos.position = _vfxEndPos.position = _origin.position;
        _lerpEndPos = _target.position;
        _lerpDuration = Mathf.Abs(Vector3.Distance(_lerpStartPos, _lerpEndPos)) / _projectileSpeed;
        _lerpFireCoR = StartCoroutine(FireWeapon());
        _isFiring = true;
        _isFiringComplete = false;
        _switchFireTriggered = false;
        if(_weaponAudio != null) _weaponAudio.Play();
    }

    private void StopFiring()
    {
        if (_lerpFireCoR != null) StopCoroutine(_lerpFireCoR);
        _lerpStopFireCoR = StartCoroutine(StopWeapon());
        //_isFiring = false;
        _isFiringComplete = false;
        _switchStopFireTriggered = false;
        if (_firingCoroutine != null) StopCoroutine(_firingCoroutine);
        _firingCoroutine = null;
        if (_weaponAudio != null) _weaponAudio.Stop();
    }

    // Update is called once per frame
    void Update()
    {

        _vfxStartPos.transform.position = _origin.position;
        _vfxStartPos.transform.rotation = Quaternion.LookRotation(_vfxStartPos.transform.InverseTransformDirection(_origin.forward), _vfxStartPos.transform.InverseTransformDirection(_origin.up));


        //_currentButtonState = ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_3);
        

        //if(!_switchFirstFrame)
        //{
        //    if (((_previousButtonState == ZekiController.BUTTON_STATE.UNKNOWN) || (_previousButtonState == ZekiController.BUTTON_STATE.ON)) && (_currentButtonState == ZekiController.BUTTON_STATE.OFF)) _switchStopFireTriggered = true;
        //    if (((_previousButtonState == ZekiController.BUTTON_STATE.UNKNOWN) || (_previousButtonState == ZekiController.BUTTON_STATE.OFF)) && (_currentButtonState == ZekiController.BUTTON_STATE.ON)) _switchFireTriggered = true;
        //}
        //else
        //{
        //    _switchFirstFrame = false;
        //}
       
        //_previousButtonState = _currentButtonState;

        if (Input.GetKeyUp(KeyCode.F) || _switchFireTriggered)
        {

            StartFiring();
        }

        if(Input.GetKeyUp(KeyCode.G) || _switchStopFireTriggered)
        {
            StopFiring();
        }

        //if(_isFiring)
        //{
            //_lerpDuration = Mathf.Abs(Vector3.Distance(_lerpStartPos, _lerpEndPos)) / _projectileSpeed;
            //_lerpEndPos = _target.position;
        //}

        if(_isFiringComplete)
        {
            _vfxEndPos.position = _target.position;
        }
    }

    private void StopAllEffects()
    {
        foreach (VisualEffect effect in _arcEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }

        foreach (VisualEffect effect in _originSparkEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }

        foreach (VisualEffect effect in _targetSparkEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }
    }

    private void StartAllEffects()
    {
        foreach (VisualEffect effect in _arcEffects)
        {
            effect.enabled = true;
            effect.Play();
        }

        foreach (VisualEffect effect in _originSparkEffects)
        {
            effect.enabled = true;
            effect.Play();
        }

        foreach (VisualEffect effect in _targetSparkEffects)
        {
            effect.enabled = true;
            effect.Play();
        }
    }

    IEnumerator FireWeapon()
    {
        _lerpTimeElapsed = 0;

        foreach (VisualEffect effect in _arcEffects)
        {
            effect.enabled = true;
            effect.Play();
        }

        foreach (VisualEffect effect in _originSparkEffects)
        {
            effect.enabled = true;
            effect.Play();
        }

        while (_lerpTimeElapsed < _lerpDuration)
        {
            _lerpProgress = _lerpTimeElapsed / _lerpDuration;
            _lerpNewPos.x = Mathf.Lerp(_lerpStartPos.x, _lerpEndPos.x, _lerpProgress);
            _lerpNewPos.y = Mathf.Lerp(_lerpStartPos.y, _lerpEndPos.y, _lerpProgress);
            _lerpNewPos.z = Mathf.Lerp(_lerpStartPos.z, _lerpEndPos.z, _lerpProgress);
            _vfxEndPos.position = _lerpNewPos;
            _lerpTimeElapsed += Time.deltaTime;
            yield return null;
        }

        _vfxEndPos.position = _lerpEndPos;
        _isFiringComplete = true;

        foreach (VisualEffect effect in _targetSparkEffects)
        {
            effect.enabled = true;
            effect.Play();
        }
        _lerpFireCoR = null;
    }

    IEnumerator StopWeapon()
    {
        _lerpTimeElapsed = 0;

        

        foreach (VisualEffect effect in _originSparkEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }

        while (_lerpTimeElapsed < _lerpDuration)
        {
            _lerpProgress = _lerpTimeElapsed / _lerpDuration;
            _lerpNewPos.x = Mathf.Lerp(_lerpStartPos.x, _lerpEndPos.x, _lerpProgress);
            _lerpNewPos.y = Mathf.Lerp(_lerpStartPos.y, _lerpEndPos.y, _lerpProgress);
            _lerpNewPos.z = Mathf.Lerp(_lerpStartPos.z, _lerpEndPos.z, _lerpProgress);
            _vfxStartPos.position = _lerpNewPos;
            _lerpTimeElapsed += Time.deltaTime;
            yield return null;
        }

        _vfxStartPos.position = _lerpEndPos;

        foreach (VisualEffect effect in _arcEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }

        foreach (VisualEffect effect in _targetSparkEffects)
        {
            effect.Stop();
            effect.enabled = false;
        }
        _isFiring = false;
        _lerpStopFireCoR = null;
    }

    public void FireSingle(int damage, iTakesDamage damageReceiver)
    {
        throw new NotImplementedException();
    }

    public void FireContinuous(int damage, float damageInterval, iTakesDamage damageReceiver)
    {
        if (_isFiring) return;
        if (damageReceiver == null) return;
        if (_firingCoroutine != null) StopCoroutine(_firingCoroutine);
        StartFiring();
        _firingCoroutine = StartCoroutine(Firing(damage, damageInterval, damageReceiver));
    }

    public void Stop()
    {
        if(!_isFiring) return;
        StopFiring();
        if (_firingCoroutine != null) StopCoroutine(_firingCoroutine);
        _firingCoroutine = null;
    }

    IEnumerator Firing(int damage, float damageInterval, iTakesDamage damageReceiver)
    {
        damageReceiver.TakeDamage(damage);
        while (damageReceiver.GetHealth() > 0)
        {
            yield return new WaitForSeconds(damageInterval);
            damageReceiver.TakeDamage(damage);
        }
    }
}
