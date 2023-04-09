using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

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

    private MaterializeOnSpawn _materializer;

    private SaboteurState _state = SaboteurState.INIT;
    


    public SaboteurState State { get => _state; }
    public Transform PlayerAudioListener { get => _playerAudioListenerPoint; }
    public Transform PlayerFakeOrigin { get => _playerFakeOrigin; }

    public event EventHandler<SaboteurStateChangedEventArgs> StateChanged;


    private void Awake()
    {
        _materializer = GetComponentInChildren<MaterializeOnSpawn>();
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
                break;
            case SaboteurState.DEAD:
                break;
            case SaboteurState.NUM_OF_STATES:
            default:
                break;
        }
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
                break;
            case SaboteurState.DEAD:
                Destroy(this.gameObject, 1f);
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
        
    }

    public int GetHealth()
    {
        return 1000;
    }
}

public class SaboteurStateChangedEventArgs : EventArgs
{
    public Saboteur.SaboteurState state;
}
