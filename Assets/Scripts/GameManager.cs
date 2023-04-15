using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GAME_STATE
    {
        INIT = 0,
        MENU,
        PLAYING,
        GAMEOVER,
        NUM_OF_STATES
    }

    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Skill[] _skills = new Skill[(int)Skill.SkillType.NUM_OF_SKILLS];

    private GameObject _player;
    private GAME_STATE _state = GAME_STATE.INIT;

     private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    public GameObject Player { get => _player; }
    public Skill[] Skills { get => _skills; }

    public void SkillLearnt(Skill.SkillType skill)
    {
        if (_skills[(int)skill].State == Skill.SkillState.DISABLED) _skills[(int)skill].SkillLearnt();
    }

    private void Start()
    {
        DoTransition(GAME_STATE.MENU);
    }

    private void SpawnPlayer()
    {
        if ((_playerSpawnPoint != null) && (_player == null)) _player = Instantiate(_playerPrefab, _playerSpawnPoint);
        if (_player != null) CameraDirector.Instance.SetNewPlayer(_player);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnPlayer();
        }

        //if (Input.GetKeyDown(KeyCode.Alpha1)) SkillLearnt(Skill.SkillType.PRECISION);
        //if (Input.GetKeyDown(KeyCode.Alpha2)) SkillLearnt(Skill.SkillType.SAFEOS);
        //if (Input.GetKeyDown(KeyCode.Alpha3)) SkillLearnt(Skill.SkillType.FAULT_INJECT);
        //if (Input.GetKeyDown(KeyCode.Alpha4)) SkillLearnt(Skill.SkillType.REDUNDANCY);

        
        if (Input.GetKeyDown(KeyCode.Alpha1)) _skills[(int)Skill.SkillType.PRECISION].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha2)) _skills[(int)Skill.SkillType.SAFEOS].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha3)) _skills[(int)Skill.SkillType.FAULT_INJECT].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha4)) _skills[(int)Skill.SkillType.REDUNDANCY].UseSkill();

        if (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_0) == ZekiController.BUTTON_STATE.ON) _skills[(int)Skill.SkillType.PRECISION].UseSkill();
        if (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_1) == ZekiController.BUTTON_STATE.ON) _skills[(int)Skill.SkillType.SAFEOS].UseSkill();
        if (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_2) == ZekiController.BUTTON_STATE.ON) _skills[(int)Skill.SkillType.FAULT_INJECT].UseSkill();
        if (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_3) == ZekiController.BUTTON_STATE.ON) _skills[(int)Skill.SkillType.REDUNDANCY].UseSkill();

    }

    private void DoTransition(GAME_STATE newState)
    {
        if (_state == newState) return;

        
        switch (newState)
        {
            case GAME_STATE.INIT:
                break;
            case GAME_STATE.MENU:
                DoExitActions(_state);
                DoEntryActions(newState);
                _state = newState;
                break;
            case GAME_STATE.PLAYING:
                if(_state == GAME_STATE.MENU)
                {
                    DoExitActions(_state);
                    DoEntryActions(newState);
                    _state = newState;
                }
                break;
            case GAME_STATE.GAMEOVER:
                if (_state == GAME_STATE.PLAYING)
                {
                    DoExitActions(_state);
                    DoEntryActions(newState);
                    _state = newState;
                }
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DoExitActions(GAME_STATE state)
    {
        switch (state)
        {
            case GAME_STATE.INIT:
                CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.IN_PLAY, false);
                break;
            case GAME_STATE.MENU:
                CanvasManager.Instance.UnLoadCanvas(CanvasManager.CANVAS.MENU, () => { CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.MENU, false); });
                break;
            case GAME_STATE.PLAYING:
                CanvasManager.Instance.UnLoadCanvas(CanvasManager.CANVAS.IN_PLAY, () => { CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.IN_PLAY, false); });
                break;
            case GAME_STATE.GAMEOVER:
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DoEntryActions(GAME_STATE state)
    {
        switch (state)
        {
            case GAME_STATE.INIT:
                break;
            case GAME_STATE.MENU:
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.MENU);
                break;
            case GAME_STATE.PLAYING:
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.IN_PLAY);
                SpawnPlayer();
                break;
            case GAME_STATE.GAMEOVER:
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    public void StartGame()
    {
        DoTransition(GAME_STATE.PLAYING);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
