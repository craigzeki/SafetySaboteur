using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using RPGCharacterAnims;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Lookups;

[RequireComponent(typeof(RPGCharacterController))]
public class Saboteur : MonoBehaviour, iTakesDamage
{
    public enum SaboteurState
    {
        INIT = 0,
        MATERIALIZING,
        ROAMING,
        LEARNING,
        SABOTAGING,
        DYING,
        DEAD,
        NUM_OF_STATES
    }

    [SerializeField] private Transform _playerAudioListenerPoint;
    [SerializeField] private Transform _playerFakeOrigin;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private SkillBar _skillBar;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _startHealth = 100;
    [SerializeField] private Transform _deathCam;
    [SerializeField] private float _deathCamTime = 4f;
    [SerializeField] private AudioSource _deathAudio;

    private MaterializeOnSpawn _materializer;

    private SaboteurState _state = SaboteurState.INIT;
    private int _currentHealth = 0;
    private RPGCharacterController _rPGCharacterController;
    private RPGCharacterMovementController _characterMovementController;

    public SaboteurState State { get => _state; }
    public Transform PlayerAudioListener { get => _playerAudioListenerPoint; }
    public Transform PlayerFakeOrigin { get => _playerFakeOrigin; }
    public Transform DeathCam { get => _deathCam; }

    public event EventHandler<SaboteurStateChangedEventArgs> StateChanged;


    private void Awake()
    {
        _rPGCharacterController = GetComponent<RPGCharacterController>();
        _characterMovementController = GetComponent<RPGCharacterMovementController>();
        _rPGCharacterController.SetHandler("Death", new Death(_characterMovementController)); 
        _materializer = GetComponentInChildren<MaterializeOnSpawn>();
        _currentHealth = _startHealth;
        if (_healthBar != null) _healthBar.OnTargetHealthReached += HealthBarReachedTarget;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8)) DoStateTransition(SaboteurState.LEARNING);
        if (Input.GetKeyDown(KeyCode.Alpha7)) DoStateTransition(SaboteurState.SABOTAGING);
        if (Input.GetKeyDown(KeyCode.Alpha9)) DoStateTransition(SaboteurState.DEAD);
        if (Input.GetKeyDown(KeyCode.Alpha6)) DoStateTransition(SaboteurState.ROAMING);

        switch (_state)
        {
            case SaboteurState.INIT:
                DoStateTransition(SaboteurState.MATERIALIZING);
                break;
            case SaboteurState.MATERIALIZING:
                if(_materializer != null)
                {
                    if (_materializer.MaterializeComplete == true) DoStateTransition(SaboteurState.ROAMING);
                }
                break;
            case SaboteurState.ROAMING:
                break;
            case SaboteurState.LEARNING:
                break;
            case SaboteurState.SABOTAGING:
                break;
            case SaboteurState.DYING:
                if(_rPGCharacterController.animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
                {
                    StartCoroutine(DeathCamTimer(_deathCamTime));
                }
                break;
            case SaboteurState.DEAD:
                break;
            case SaboteurState.NUM_OF_STATES:
            default:
                break;
        }
    }

    IEnumerator DeathCamTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        DoStateTransition(SaboteurState.DEAD);
    }

    private void DoStateTransition(SaboteurState newState)
    {
        switch (newState)
        {
            case SaboteurState.INIT:
                //do nothing, not allowed to transition back here
                break;
            case SaboteurState.MATERIALIZING:
                if(_state == SaboteurState.INIT)
                {
                    //only allowed to transition to here from INIT
                    _state = newState;
                }
                break;
            case SaboteurState.ROAMING:
                if((_state == SaboteurState.MATERIALIZING) ||
                    (_state == SaboteurState.LEARNING) ||
                    (_state == SaboteurState.SABOTAGING))
                {
                    _state = newState;
                }
                break;
            case SaboteurState.LEARNING:
                if(_state == SaboteurState.ROAMING)
                {
                    _state = newState;
                }
                break;
            case SaboteurState.SABOTAGING:
                if (_state == SaboteurState.ROAMING)
                {
                    _state = newState;
                }
                break;
            case SaboteurState.DYING:
                if((_state == SaboteurState.ROAMING) ||
                    (_state == SaboteurState.LEARNING) ||
                    (_state == SaboteurState.SABOTAGING))
                {
                    //disable the health and skill bars
                    if (_healthBar != null) _healthBar.gameObject.SetActive(false);
                    if (_skillBar != null) _skillBar.gameObject.SetActive(false);
                    //death sound
                    if (_deathAudio != null) _deathAudio.Play();
                    // Death animation.
                    if (_rPGCharacterController.HandlerExists(HandlerTypes.Death))
                    {
                        if (_rPGCharacterController.CanStartAction(HandlerTypes.Death))
                        { _rPGCharacterController.StartAction(HandlerTypes.Death, new HitContext((int)KnockdownType.Knockdown1, Vector3.back)); }
                    }
                    _state = newState;
                }
                break;
            case SaboteurState.DEAD:
                _state = newState;

                Destroy(this.gameObject);
                
                break;
            case SaboteurState.NUM_OF_STATES:
            default:
                break;
        }

        //if the state has changed, inform all listeners
        if (_state == newState) OnStateChanged(_state);
    }

    protected virtual void OnStateChanged(SaboteurState state)
    {
        SaboteurStateChangedEventArgs saboteurStateChangedEventArgs = new SaboteurStateChangedEventArgs();
        saboteurStateChangedEventArgs.state = state;
        StateChanged?.Invoke(this, saboteurStateChangedEventArgs);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        SetHealthBar();
    }

    private void SetHealthBar()
    {
        if (_healthBar == null) return;
        _healthBar.SetHealthPercent((float)((float)_currentHealth / (float)_maxHealth));
    }

    private void HealthBarReachedTarget(object sender, float percent)
    {
        if (Mathf.Approximately(percent, 0f))
        {
            DoStateTransition(SaboteurState.DYING);
        }
    }

    public bool GetIsDead()
    {
        return ((_state == SaboteurState.DYING) || (_state == SaboteurState.DEAD));
    }

    public int GetHealth()
    {
        return _currentHealth;
    }
}

public class SaboteurStateChangedEventArgs : EventArgs
{
    public Saboteur.SaboteurState state;
}
