using RPGCharacterAnims;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class TowerTerminalPlayerPoint : MonoBehaviour
{
    [SerializeField] private Tower _linkedTower;
    [SerializeField] private WeaknessPanel _weaknessPanel;
    [SerializeField] private GameObject[] _weaknessInfoPanels = new GameObject[(int)Skill.SkillType.NUM_OF_SKILLS];
    [SerializeField] private Outline _outline;
    [SerializeField] private Transform _playerRotation;
    [SerializeField] private bool[] _weaknessExpolited = new bool[(int)Skill.SkillType.NUM_OF_SKILLS];

    private RPGCharacterController _rpgCharacterController;
    private bool _playerLocked = false;
    private iWeaknessInfoPanel[] _iWeaknessInfoPanels = new iWeaknessInfoPanel[(int)Skill.SkillType.NUM_OF_SKILLS];
    //private bool[] _weaknessesAvailable = new bool[(int)Skill.SkillType.NUM_OF_SKILLS];

    private void Start()
    {
        _outline.enabled = false;
        GameManager.Instance.SkillUsed += OnSkillUsed;

        for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
        {
            _weaknessExpolited[i] = false;
            _iWeaknessInfoPanels[i] = null;
            if (_weaknessInfoPanels[i] != null)
            {
                _iWeaknessInfoPanels[i] = _weaknessInfoPanels[i].GetComponent<iWeaknessInfoPanel>();
                //if (_iWeaknessInfoPanels[i] != null) _iWeaknessInfoPanels[i].SetDependantWeaknessState((Skill.SkillType)i, true);
                //_weaknessesAvailable[i] = true;
            }
            
            else if (_weaknessPanel != null) _weaknessPanel.WeaknessRows[i].SetInfoState(false);
        }

        foreach(iWeaknessInfoPanel panel in _iWeaknessInfoPanels)
        {
            for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
            {
                if (panel != null) panel.SetDependantWeaknessState((Skill.SkillType)i, _iWeaknessInfoPanels[i] != null ? true : false);
            }
        }

        //for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
        //{
        //    if (_iWeaknessInfoPanels[i] != null) _iWeaknessInfoPanels[i].SetDependantWeaknessState(_weaknessesAvailable);
        //}

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

    private void OnSkillUsed(object sender, Skill.SkillType skill)
    {
        if(_rpgCharacterController != null) //player in range
        {

            bool doSabotage = false;
            //CHECK IF THE SKILL CAN BE USED YET BASED ON THE OTHER WEAKNESSES

            switch (skill)
            {
                case Skill.SkillType.PRECISION:
                    if (!WeaknessAvailable(Skill.SkillType.PRECISION)) break;
                    if(WeaknessAvailable(Skill.SkillType.SAFEOS) && (WeaknessExploited(Skill.SkillType.SAFEOS, true)))
                    {
                        doSabotage = true;
                    }
                    else if(!WeaknessAvailable(Skill.SkillType.SAFEOS))
                    {
                        doSabotage = true;
                    }
                    break;
                case Skill.SkillType.SAFEOS:
                    if (!WeaknessAvailable(Skill.SkillType.SAFEOS)) break;
                    if(WeaknessAvailable(Skill.SkillType.REDUNDANCY) && WeaknessExploited(Skill.SkillType.REDUNDANCY, true))
                    {
                        doSabotage = true;
                    }
                    else if(!WeaknessAvailable(Skill.SkillType.REDUNDANCY))
                    {
                        doSabotage = true;
                    }
                    break;
                case Skill.SkillType.FAULT_INJECT:
                    if (!WeaknessAvailable(Skill.SkillType.FAULT_INJECT)) break;
                    
                    if (WeaknessAvailable(Skill.SkillType.SAFEOS) && WeaknessAvailable(Skill.SkillType.REDUNDANCY))
                    {
                        if(WeaknessExploited(Skill.SkillType.SAFEOS, true) && WeaknessExploited(Skill.SkillType.REDUNDANCY, true))
                        {
                            doSabotage = true;
                        }
                    }
                    else
                    {
                        if(WeaknessAvailable(Skill.SkillType.REDUNDANCY) && WeaknessExploited(Skill.SkillType.REDUNDANCY, true))
                        {
                            doSabotage=true;
                        }
                        else if(WeaknessAvailable(Skill.SkillType.SAFEOS) && WeaknessExploited(Skill.SkillType.SAFEOS, true))
                        {
                            doSabotage = true;
                        }
                        else if(!WeaknessAvailable(Skill.SkillType.SAFEOS) && !WeaknessAvailable(Skill.SkillType.REDUNDANCY))
                        {
                            doSabotage = true;
                        }
                    }
                    
                    if(WeaknessExploited(Skill.SkillType.REDUNDANCY, true)) doSabotage=true;
                    break;
                case Skill.SkillType.REDUNDANCY:
                    if(WeaknessAvailable(Skill.SkillType.REDUNDANCY)) doSabotage=true;
                    break;
                case Skill.SkillType.NUM_OF_SKILLS:
                default:
                    break;
            }

            if(doSabotage)
            {
                _weaknessExpolited[(int)skill] = true;
                if (_iWeaknessInfoPanels[(int)skill] != null) _iWeaknessInfoPanels[(int)skill].OnSkillUsed();
                if (_linkedTower != null) _linkedTower.DoSabotage(skill);
                for(int i = 0; i < (int)Skill.SkillType.NUM_OF_SKILLS; i++)
                {
                    if (_iWeaknessInfoPanels[i] != null) _iWeaknessInfoPanels[i].SetDependantWeaknessState(skill, false);
                }
            }
            else
            {
                //Inform player not possible - try something else
            }
            
        }
    }

    private bool WeaknessAvailable(Skill.SkillType skill)
    {
        return _weaknessInfoPanels[(int)skill] != null;
    }

    private bool WeaknessExploited(Skill.SkillType skill, bool alreadyExploited)
    {
        return ((_weaknessInfoPanels[(int)skill] != null) && (_weaknessExpolited[(int)skill] == alreadyExploited));
    }

}
