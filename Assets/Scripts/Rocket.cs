using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SonicRotator))]
public class Rocket : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _range = 8f;
    [SerializeField] private float _lookSpeed = 0.5f;
    [SerializeField] private AudioSource _explosionAudio;
    [SerializeField] private LayerMask _collideWith;

    private iTowerRotator _rotator;

    private Transform _target;
    private bool _isLaunched;

    private Vector3 _direction;
    private Quaternion _lookRotation;
    private Quaternion _currentRotation;
    private float _distanceTravelled;
    private iTakesDamage _damageReceiver;
    private int _damageCaused;
    private Renderer _renderer;
    private Collider _collider;
    
    public void Launch(Transform target, int damageCaused)
    {
        _damageCaused = damageCaused;
        _distanceTravelled = 0;
        _target = target;
        _rotator.RotateTowards(_target.position);
        _isLaunched = true;
    }

    private void Awake()
    {
        _rotator = GetComponent<SonicRotator>();
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    void Update()
    {
        if(_isLaunched)
        {
            _rotator.RotateTowards(_target.position);
            _rotator.UpdateRotator();
            transform.position += transform.up * _speed * Time.deltaTime;
            _distanceTravelled += _speed * Time.deltaTime;
            if(_distanceTravelled > _range)
            {
                Explode();
            }
        }
    }

    private void Explode()
    {
        _explosionAudio?.Play();
        //explosion animation
        _damageReceiver?.TakeDamage(_damageCaused);
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    { 
        if(_collideWith == (_collideWith | (1 << other.gameObject.layer)))
        {
            other.gameObject.TryGetComponent<iTakesDamage>(out _damageReceiver);
            Explode();
        }
        
    }
}
