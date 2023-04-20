using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketWeapon : MonoBehaviour, iTowerWeapon
{
    [SerializeField] private Transform _rocketSpawnPoint;
    [SerializeField] private GameObject _rocketPrefab;
    [SerializeField] private float _reloadDuration = 2.5f;
    [SerializeField] private AudioSource _fireSound;
    private bool _isLoaded = false;
    private GameObject _loadedRocket;
    private Coroutine _reloadCoroutine;
    private Rocket _rocket;

    public void FireContinuous(int damage, float damageInterval, iTakesDamage damageReceiver)
    {
        return;
    }

    public void FireSingle(int damage, iTakesDamage damageReceiver)
    {
        FireSingle(damage, damageReceiver.GetTransform());
    }

    public void FireSingle(int damage, Transform targetObject)
    {
        if (!_isLoaded) return;
        _isLoaded = false;
        //fire rocket
        _rocket?.Launch(targetObject, damage);

        _fireSound?.Play();
        if (_reloadCoroutine != null) StopCoroutine(_reloadCoroutine);
        _reloadCoroutine = StartCoroutine(Reload());
        
    }

    public bool IsLoaded()
    {
        return _isLoaded;
    }

    public void Stop()
    {
        return;
    }


    IEnumerator Reload()
    {
        yield return new WaitForSeconds(_reloadDuration);
        if (_rocketPrefab != null) _loadedRocket = Instantiate(_rocketPrefab, _rocketSpawnPoint);
        if (_loadedRocket != null) _rocket = _loadedRocket.GetComponentInChildren<Rocket>();
        _isLoaded = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _reloadCoroutine = StartCoroutine(Reload());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
