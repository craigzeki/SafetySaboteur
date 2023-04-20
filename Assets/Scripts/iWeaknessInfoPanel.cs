using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface iWeaknessInfoPanel
{
    public void OnSkillUsed();
    public void SetDependantWeaknessState(Skill.SkillType skillType, bool state);
}
