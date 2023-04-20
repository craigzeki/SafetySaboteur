using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FaultInjectPanel : MonoBehaviour, iWeaknessInfoPanel
{
    [SerializeField] private TextMeshProUGUI _protectedMode1Text;
    [SerializeField] private TextMeshProUGUI _protectedMode2Text;
    [SerializeField] private TextMeshProUGUI _sabotageModeText;
    [SerializeField] private string _protectedEnabledText = "TRUE";
    [SerializeField] private string _protectedDisabledText = "FALSE";
    [SerializeField] private string _sabotagedMode = "FAULT PRESENT";

    [SerializeField] private Color _startupColor;
    [SerializeField] private Color _sabotagedColor;

    public void OnSkillUsed()
    {
        _sabotageModeText.text = _sabotagedMode;
        _sabotageModeText.color = _sabotagedColor;
    }

    private void Awake()
    {
        _protectedMode2Text.text = _protectedMode1Text.text = _protectedDisabledText;
        _protectedMode2Text.color = _protectedMode1Text.color = _sabotagedColor;
    }

    public void SetDependantWeaknessState(Skill.SkillType skillType, bool state)
    {
        switch (skillType)
        {
            case Skill.SkillType.PRECISION:
                break;
            case Skill.SkillType.SAFEOS:
                _protectedMode2Text.text = state ? _protectedEnabledText : _protectedDisabledText;
                _protectedMode2Text.color = state ? _startupColor : _sabotagedColor;
                break;
            case Skill.SkillType.FAULT_INJECT:
                break;
            case Skill.SkillType.REDUNDANCY:
                _protectedMode1Text.text = state ? _protectedEnabledText : _protectedDisabledText;
                _protectedMode1Text.color = state ? _startupColor : _sabotagedColor;
                break;
            case Skill.SkillType.NUM_OF_SKILLS:
            default:
                break;
        }


    }
}
