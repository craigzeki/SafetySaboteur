using System;
using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{

    public enum SkillType
    {
        PRECISION = 0,
        SAFEOS,
        FAULT_INJECT,
        REDUNDANCY,
        NUM_OF_SKILLS
    }

    public enum SkillState
    {
        DISABLED = 0,
        CHARGING,
        READY,
        NUM_OF_STATES
    }

    [SerializeField] private SkillState _state = SkillState.DISABLED;
    [SerializeField] private float _chargeTime = 5f;
    [SerializeField] private SkillType _skillType = SkillType.PRECISION;
    [SerializeField] private GUISkill _guiSkill;
    [SerializeField] private SkillState _newState = SkillState.DISABLED;

    private const float CHARGING_PERIOD_MS = 50f;
    private const float CHARGING_STEP = 1000f / CHARGING_PERIOD_MS;

    private Coroutine _chargingCoroutine;

    public SkillState State { get => _state; }

    private bool initComplete = false;

    private void Start()
    {
        if (_guiSkill == null) return;
        if (_guiSkill.HealthBar == null) return;
        _guiSkill.HealthBar.OnTargetHealthReached += HealthBarReachedTarget;
        initComplete = true;
    }

    private void Update()
    {
        //allow debugging from the inspector
        if (_newState != _state) DoTransition(_newState);
    }

    private void DoTransition(SkillState newState)
    {
        if (!initComplete) return;

        switch (newState)
        {
            case SkillState.DISABLED:
                break;
            case SkillState.CHARGING:
                if (_state == SkillState.DISABLED)
                {
                    _guiSkill.SetEnabled(true);
                }
                _guiSkill.HealthBar.SetHealthPercent(0f, false);
                if (_chargingCoroutine != null) StopCoroutine(_chargingCoroutine);
                _chargingCoroutine = StartCoroutine(DoCharging(_chargeTime));
                _state = newState;
                break;
            case SkillState.READY:
                if (_state == SkillState.DISABLED)
                {
                    _guiSkill.SetEnabled(true);
                    _guiSkill.HealthBar.SetHealthPercent(1f, false);
                }
                
                _state = newState;
                
                
                break;
            case SkillState.NUM_OF_STATES:
            default:
                break;
        }
        _newState = newState;
    }

    public void UseSkill()
    {
        DoTransition(SkillState.CHARGING);
    }

    public void SkillLearnt()
    {
        DoTransition(SkillState.READY);
    }

    IEnumerator DoCharging(float duration)
    {
        float percentStep = 1 / (CHARGING_STEP * duration);
        float elapsedTime = 0f;
        float percent = 0;

        while(elapsedTime < duration)
        {
            percent = Mathf.Clamp(percent + percentStep, 0f, 1f);
            
            _guiSkill.HealthBar.SetHealthPercent(percent);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(CHARGING_PERIOD_MS / 1000f);

        }
        _guiSkill.HealthBar.SetHealthPercent(1f);
    }

    private void HealthBarReachedTarget(object sender, float percent)
    {
        if (Mathf.Approximately(percent, 1f))
        {
            DoTransition(SkillState.READY);
        }
    }

}
