using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaknessPanel : MonoBehaviour
{
    [SerializeField] private WeaknessRow[] _weaknessRows = new WeaknessRow[(int)Skill.SkillType.NUM_OF_SKILLS];

    private void OnGUI()
    {
        for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
        {
            _weaknessRows[i].SetPadlockState(GameManager.Instance.Skills[i].State != Skill.SkillState.READY);
        }
    }
}
