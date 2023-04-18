using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MouseClickPanelNavigator : MonoBehaviour
{
    [SerializeField] private GameObject _panelToHide;
    public void DoNavigate(GameObject nextPanel)
    {
        if(nextPanel != null) nextPanel.SetActive(true);
        if(_panelToHide != null) _panelToHide.SetActive(false);
    }
}
