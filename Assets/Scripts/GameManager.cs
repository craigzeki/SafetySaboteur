using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] private List<String> _sceneNames = new List<String>();
    [SerializeField] private CinemachineBrain _mainCam;

    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private DroneArmyManager _droneArmyManager;

    [SerializeField] private Skill[] _skills = new Skill[(int)Skill.SkillType.NUM_OF_SKILLS];

    [SerializeField] private Texture2D _mouseCursor;
    [SerializeField] private Vector2 _cursorHotSpot;

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _dronesText;

    private GameObject _player;
    private GAME_STATE _state = GAME_STATE.INIT;
    private CursorMode cursorMode = CursorMode.Auto;
    private uint _score = 0;
    private uint _droneSurvivedCount = 0;
    private uint _droneTotalCount = 0;

    private int _currentScene = -1;

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
    public CinemachineBrain MainCam { get => _mainCam;  }

    public void SkillLearnt(Skill.SkillType skill)
    {
        if (_skills[(int)skill].State == Skill.SkillState.DISABLED) _skills[(int)skill].SkillLearnt();
    }

    private void Awake()
    {
        if(_mouseCursor != null) Cursor.SetCursor(_mouseCursor, _cursorHotSpot, cursorMode);
        LoadNextScene();
        UpdateUI();
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void Start()
    {
        DoTransition(GAME_STATE.MENU);
    }

    private void UpdateUI()
    {
        if (_dronesText != null) _dronesText.text = _droneSurvivedCount.ToString("00") + "/" + _droneTotalCount.ToString("00");
        if (_scoreText != null) _scoreText.text = _score.ToString("000000");
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

    public void SetTotalDrones(uint totalDrones)
    {
        _droneTotalCount = totalDrones;
        UpdateUI();
    }

    public void UpdateScore(uint points, bool incrementDroneCount = false)
    {
        _score += points;
        if (incrementDroneCount) _droneSurvivedCount++;
        UpdateUI();
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
                if (_droneArmyManager != null) _droneArmyManager.StartSpawning();
                break;
            case GAME_STATE.GAMEOVER:
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    public void RegisterSpawnPoint(Transform spawnPoint)
    {
        _playerSpawnPoint = spawnPoint;
    }

    public void RegisterDroneArmyManager(DroneArmyManager droneArmyManager)
    {
        _droneArmyManager = droneArmyManager;
    }

    public void StartGame()
    {
        DoTransition(GAME_STATE.PLAYING);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadNextScene()
    {

        _currentScene++;
        if (_currentScene < _sceneNames.Count)
        {
            foreach (Skill skill in _skills)
            {
                skill.SkillReset();
            }
            SceneManager.LoadScene(_sceneNames[_currentScene]);
        }
        else { _currentScene--; }
    }
}
