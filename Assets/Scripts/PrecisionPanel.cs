using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrecisionPanel : MonoBehaviour, iWeaknessInfoPanel
{
    [SerializeField] private TextMeshProUGUI _activeModeText;
    [SerializeField] private string _startupMode = "FLOAT";
    [SerializeField] private string _sabotagedMode = "INTEGER";

    [SerializeField] private Color _startupColor;
    [SerializeField] private Color _sabotagedColor;

    public void OnSkillUsed()
    {
        _activeModeText.text = _sabotagedMode;
        _activeModeText.color = _sabotagedColor;
    }

}
