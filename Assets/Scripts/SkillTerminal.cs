using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Skill;

[RequireComponent(typeof(SphereCollider))]
public class SkillTerminal : MonoBehaviour
{
    [SerializeField] private Skill.SkillType _skillType = Skill.SkillType.PRECISION;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private AudioSource _learningAudio;
    [SerializeField] private AudioSource _learningCompleteAudio;
    
    private SkillBar _skillBar;

    private SphereCollider _collider;
    private Coroutine _learningCorourtine;

    private const float LEARNING_PERIOD_MS = 50f;
    private const float LEARNING_STEP = 1000f / LEARNING_PERIOD_MS;
    private const float LEARNING_COMPLETE_PAUSE = 1f;

    private bool _learningComplete = false;

    IEnumerator DoLearning(float duration)
    {
        float percentStep = 1 / (LEARNING_STEP * duration);
        float percent = 0;

        Debug.Log("Duration: " + duration.ToString());
        while (!_learningComplete)
        {
            percent = Mathf.Clamp(percent + percentStep, 0f, 1f);

            _skillBar.SetSkillPercent(percent);



            yield return new WaitForSeconds(LEARNING_PERIOD_MS / 1000f);

        }
        _skillBar.SetSkillPercent(1f);

        if (_learningAudio != null) _learningAudio.Stop();
        if (_learningCompleteAudio != null) _learningCompleteAudio.Play();
        yield return new WaitForSeconds(LEARNING_COMPLETE_PAUSE);
        _skillBar.SetSkillPercent(0f, false);
        _skillBar.SetEnabled(false);
        
    }

   
    private void OnTriggerEnter(Collider other)
    {
        if (GameManager.Instance.Skills[(int)_skillType].State != Skill.SkillState.DISABLED) return;
        if(other.gameObject.tag == "Player")
        {
            _skillBar = other.gameObject.GetComponentInChildren<SkillBar>();
            if(_skillBar != null)
            {
                if (_learningAudio != null) _learningAudio.Play();
                _skillBar.SetEnabled(true);
                _skillBar.SetSkillPercent(0f, false);
                _skillBar.OnTargetSkillReached += SkillBarReachedTarget;
                ZekiController.Instance.SetButtonLEDState(GameManager.Instance.Skills[(int)_skillType].ControllerButton, ZekiController.BUTTON_LED_STATE.LED_GLOW);
                if(_learningCorourtine != null) StopCoroutine(_learningCorourtine);
                _learningCorourtine = StartCoroutine(DoLearning(_duration));
            }
        }
    }

 

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            _skillBar = other.gameObject.GetComponentInChildren<SkillBar>();
            if (_skillBar != null)
            {
                if (_learningAudio != null) _learningAudio.Stop();
                _skillBar.OnTargetSkillReached -= SkillBarReachedTarget;
                _skillBar.SetSkillPercent(0f, false);
                if (GameManager.Instance.Skills[(int)_skillType].State == SkillState.DISABLED)
                {
                    ZekiController.Instance.SetButtonLEDState(GameManager.Instance.Skills[(int)_skillType].ControllerButton, ZekiController.BUTTON_LED_STATE.LED_OFF);
                }
                
                _skillBar.SetEnabled(false);
                if (_learningCorourtine != null) StopCoroutine(_learningCorourtine);
            }
        }
    }

    private void SkillBarReachedTarget(object sender, float percent)
    {
        if (Mathf.Approximately(percent, 1f))
        {
            _learningComplete = true;
            GameManager.Instance.SkillLearnt(_skillType);
        }
    }
}
