using RPGCharacterAnims;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class TowerTerminalPlayerPoint : MonoBehaviour
{
    [SerializeField] private Tower _linkedTower;
    [SerializeField] private GameObject[] _weaknessInfoPanels = new GameObject[(int)Skill.SkillType.NUM_OF_SKILLS];
    [SerializeField] private Outline _outline;
    [SerializeField] private Transform _playerRotation;
    private RPGCharacterController _rpgCharacterController;
    private bool _playerLocked = false;
    private iWeaknessInfoPanel[] _iWeaknessInfoPanels = new iWeaknessInfoPanel[(int)Skill.SkillType.NUM_OF_SKILLS];

    private void Start()
    {
        _outline.enabled = false;
        GameManager.Instance.SkillUsed += OnSkillUsed;

        for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
        {
            _iWeaknessInfoPanels[i] = null;
            if(_weaknessInfoPanels[i] != null) _iWeaknessInfoPanels[i] = _weaknessInfoPanels[i].GetComponent<iWeaknessInfoPanel>();
        }

    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null) GameManager.Instance.SkillUsed -= OnSkillUsed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (_outline == null) return;
        if(other.gameObject.tag == "Player")
        {
            _outline.enabled = true;
            _rpgCharacterController = other.gameObject.GetComponent<RPGCharacterController>();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (_outline == null) return;
        if (other.gameObject.tag == "Player")
        {
            _outline.enabled = false;
            _rpgCharacterController = null;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.State != GameManager.GAME_STATE.PLAYING) return;

        if (_outline == null) return;

        if (_rpgCharacterController != null)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(!_playerLocked)
                {
                    _rpgCharacterController.Lock(true, true, false, 0f, 0f);
                    _rpgCharacterController.gameObject.transform.position = transform.position;
                    _rpgCharacterController.gameObject.transform.rotation = _playerRotation.rotation;
                    CameraDirector.Instance.SetCamera(CameraDirector.CameraList.ShoulderCam);
                    _playerLocked = true;
                    _outline.enabled = false;
                }
                else
                {
                    //do not unlock player movement here, wait for cam to be live (see further down)
                    CameraDirector.Instance.SetCamera(CameraDirector.CameraList.RoamingCam);
                    _playerLocked = false;
                    _outline.enabled = true;
                }
                
            }
        }

        if(!_playerLocked && _rpgCharacterController != null)
        {
            if(!CameraDirector.Instance.GetIsLive(CameraDirector.CameraList.ShoulderCam))
            {
                _rpgCharacterController.Unlock(true, true);
            }
        }
    }

    public void ButtonClick()
    {
        Debug.Log("BUTTON");
    }

    private void OnSkillUsed(object sender, Skill.SkillType _skill)
    {
        if(_rpgCharacterController != null) //player in range
        {
            if (_iWeaknessInfoPanels[(int)_skill] != null) _iWeaknessInfoPanels[(int)_skill].OnSkillUsed();
            if(_linkedTower != null) _linkedTower.DoSabotage(_skill);
        }
    }
    
    
}
