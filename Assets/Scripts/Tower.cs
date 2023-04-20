using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting;

public class Tower : MonoBehaviour
{
    public enum TowerState
    {
        INIT = 0,
        SCANNING,
        FIRING,
        RELOADING,
        DESTROYED,
        NUM_OF_STATES

    }

    [SerializeField] private TowerTargetInfo _targetInfo;
    [SerializeField] private GameObject _scannerObject;
    [SerializeField] private GameObject _rotatorObject;
    [SerializeField] private GameObject _weaponObject;
    [SerializeField] private bool _continuousShot = true;
    [SerializeField] private int _damage = 5;
    [SerializeField] private float _damageInterval = 1f;


    private iTowerScanner _scanner;
    private iTowerRotator _rotator;
    private iTowerWeapon _weapon;

    [SerializeField] private TowerState _currentState;
    private TowerState _newState;

    public TowerState NewState
    {
        set
        {
            _newState = value;
            DoTransition(_newState);
        }
    }

    private void DoTransition(TowerState newState)
    {
        switch (newState)
        {
            case TowerState.INIT:
                break;
            case TowerState.SCANNING:
                if((_currentState == TowerState.INIT) || (_currentState == TowerState.FIRING))
                {
                    _currentState = _newState;
                }
                break;
            case TowerState.FIRING:
                if((_currentState == TowerState.SCANNING) || (_currentState == TowerState.RELOADING))
                {
                    _currentState = _newState;
                }
                break;
            case TowerState.RELOADING:
                if(_currentState == TowerState.FIRING)
                {
                    _currentState= _newState;
                }
                break;
            case TowerState.DESTROYED:
                //can get here from all states
                break;
            case TowerState.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DoStateActions()
    {
        switch (_currentState)
        {
            case TowerState.INIT:
                break;
            case TowerState.SCANNING:
                _scanner.ScannerUpdate();
                _targetInfo = _scanner.GetTowerTargetInfo();
                if (_targetInfo == null) return;
                if (!_targetInfo.TargetAquired) return;
                
                _rotator.RotateTowards(_targetInfo.DirectionToTarget);
                _rotator.UpdateRotator();

                if(_targetInfo.TargetInView && _targetInfo.TargetInRange) NewState = TowerState.FIRING;
                break;
            case TowerState.FIRING:
                
                if(!_targetInfo.DamageReceiver.IsUnityNull())
                {
                    //(_targetInfo.DamageReceiver.GetHealth() <= 0)
                    if (
                        (_targetInfo.DamageReceiver.GetIsDead()) ||
                        (!_targetInfo.TargetAquired) ||
                        (!_targetInfo.TargetInRange) ||
                        (!_targetInfo.TargetInView))
                    {
                        _weapon?.Stop();
                        NewState = TowerState.SCANNING;
                    }
                    else
                    {
                        if(_continuousShot) _weapon?.FireContinuous(_damage, _damageInterval, _targetInfo.DamageReceiver);
                        else
                        {
                            if(_weapon?.IsLoaded() == true)
                            {
                                _weapon?.FireSingle(_damage, _targetInfo.DamageReceiver);
                            }
                        }
                    }
                }
                else
                {
                    _weapon?.Stop();
                    NewState = TowerState.SCANNING;
                }
                _scanner.ScannerUpdate();
                _targetInfo = _scanner.GetTowerTargetInfo();
                if (_targetInfo == null) return;
                if (!_targetInfo.TargetAquired) return;

                _rotator.RotateTowards(_targetInfo.DirectionToTarget);
                _rotator.UpdateRotator();

                break;
            case TowerState.RELOADING:
                break;
            case TowerState.DESTROYED:
                break;
            case TowerState.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void Awake()
    {
        _currentState = TowerState.INIT;
        
        if(_scannerObject != null)
        {
            _scanner = _scannerObject.GetComponent<iTowerScanner>();
        }
        else
        {
            _scanner = GetComponent<iTowerScanner>();
        }

        if(_rotatorObject != null)
        {
            _rotator = _rotatorObject.GetComponent<iTowerRotator>();
        }
        else
        {
            _rotator = GetComponent<iTowerRotator>();
        }

        if(_weaponObject != null)
        {
            _weapon = _weaponObject.GetComponent<iTowerWeapon>();
        }
        else
        {
            _weapon = GetComponent<iTowerWeapon>();
        }
    }

    private void Start()
    {
        NewState = TowerState.SCANNING;
    }

    void Update()
    {
        if (GameManager.Instance.State == GameManager.GAME_STATE.PLAYING) DoStateActions();
        else _weapon?.Stop();
    }

    public void DoSabotage(Skill.SkillType _skill)
    {
        switch (_skill)
        {
            case Skill.SkillType.PRECISION:
                _scanner.DoSabotage();
                break;
            case Skill.SkillType.SAFEOS:
                break;
            case Skill.SkillType.FAULT_INJECT:
                _rotator.DoSabotage();
                break;
            case Skill.SkillType.REDUNDANCY:
                break;
            case Skill.SkillType.NUM_OF_SKILLS:
            default:
                break;
        }

    }
}
