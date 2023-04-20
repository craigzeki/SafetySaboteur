using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RedundancyPanel : MonoBehaviour, iWeaknessInfoPanel
{

    [SerializeField] private TextMeshProUGUI _activeModeText;
    [SerializeField] private string _startupMode = "ENABLED";
    [SerializeField] private string _sabotagedMode = "DISABLED";

    [SerializeField] private Color _startupColor;
    [SerializeField] private Color _sabotagedColor;

    public void OnSkillUsed()
    {
        _activeModeText.text = _sabotagedMode;
        _activeModeText.color = _sabotagedColor;
    }

    private void Awake()
    {

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
                
                break;
            case Skill.SkillType.NUM_OF_SKILLS:
            default:
                break;
        }
    }
}
