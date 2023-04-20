using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SafeOSPanel : MonoBehaviour, iWeaknessInfoPanel
{
    [SerializeField] private TextMeshProUGUI _protectedModeText;
    [SerializeField] private TextMeshProUGUI _sabotageModeText;
    [SerializeField] private string _protectedEnabledText = "TRUE";
    [SerializeField] private string _protectedDisabledText = "FALSE";
    [SerializeField] private string _sabotagedMode = "DISABLED";

    [SerializeField] private Color _startupColor;
    [SerializeField] private Color _sabotagedColor;

    public void OnSkillUsed()
    {
        _sabotageModeText.text = _sabotagedMode;
        _sabotageModeText.color = _sabotagedColor;
    }

    private void Awake()
    {
        _protectedModeText.text = _protectedDisabledText;
        _protectedModeText.color = _sabotagedColor;
    }

    public void SetDependantWeaknessState(Skill.SkillType skillType, bool state)
    {
        switch (skillType)
        {
            case Skill.SkillType.PRECISION:
                break;
            case Skill.SkillType.SAFEOS:
                break;
            case Skill.SkillType.FAULT_INJECT:
                break;
            case Skill.SkillType.REDUNDANCY:
                _protectedModeText.text = state ? _protectedEnabledText : _protectedDisabledText;
                _protectedModeText.color = state ? _startupColor : _sabotagedColor;
                break;
            case Skill.SkillType.NUM_OF_SKILLS:
            default:
                break;
        }


    }
}
