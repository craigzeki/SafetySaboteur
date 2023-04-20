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
        RESET,
        MENU,
        PLAYING,
        GAMEOVER,
        LEVEL_COMPLETE,
        NUM_OF_STATES
    }

    [SerializeField] private List<String> _sceneNames = new List<String>();
    [SerializeField] private CinemachineBrain _mainCam;

    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private uint _playerStartLives = 3;
    [SerializeField] private DroneArmyManager _droneArmyManager;

    [SerializeField] private Skill[] _skills = new Skill[(int)Skill.SkillType.NUM_OF_SKILLS];

    [SerializeField] private Texture2D _mouseCursor;
    [SerializeField] private Vector2 _cursorHotSpot;

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _dronesText;

    [SerializeField] private TextMeshProUGUI _gameOverScoreText;
    [SerializeField] private TextMeshProUGUI _gameOverDronesText;

    [SerializeField] private TextMeshProUGUI _levelCompleteScoreText;
    [SerializeField] private TextMeshProUGUI _levelCompleteDronesText;

    [SerializeField] private TextMeshProUGUI _livesValueText;

    private GameObject _player;
    private Saboteur _saboteur;
    private GAME_STATE _state = GAME_STATE.INIT;
    private CursorMode cursorMode = CursorMode.Auto;
    private uint _score = 0;
    private uint _droneSurvivedCount = 0;
    [SerializeField] private uint _droneLevelCount = 0;
    private uint _droneTotalCount = 0;
    private uint _playerLives;

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
    public CinemachineBrain MainCam { get => _mainCam; }
    public GAME_STATE State { get => _state; }

    public event EventHandler<Skill.SkillType> SkillUsed;

    public void SkillLearnt(Skill.SkillType skill)
    {
        if (_skills[(int)skill].State == Skill.SkillState.DISABLED) _skills[(int)skill].SkillLearnt();
    }

    private void Awake()
    {
        if (_mouseCursor != null) Cursor.SetCursor(_mouseCursor, _cursorHotSpot, cursorMode);
        LoadNextScene();
        UpdateUI();
        UpdateLivesText();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    private void Start()
    {
        DoTransition(GAME_STATE.RESET);
    }

    private void UpdateUI()
    {
        if (_dronesText != null) _dronesText.text = _droneSurvivedCount.ToString("00") + "/" + _droneTotalCount.ToString("00");
        if (_scoreText != null) _scoreText.text = _score.ToString("000000");
        if (_gameOverDronesText != null) _gameOverDronesText.text = "Drones Saved: " + _droneSurvivedCount.ToString("00") + "/" + _droneTotalCount.ToString("00");
        if (_gameOverScoreText != null) _gameOverScoreText.text = "Score: " + _score.ToString("000000");
        if (_levelCompleteDronesText != null) _levelCompleteDronesText.text = "Drones Saved: " + _droneSurvivedCount.ToString("00") + "/" + _droneTotalCount.ToString("00");
        if (_levelCompleteScoreText != null) _levelCompleteScoreText.text = "Score: " + _score.ToString("000000");
    }

    private void SpawnPlayer()
    {
        if ((_playerSpawnPoint != null) && (_player == null)) _player = Instantiate(_playerPrefab, _playerSpawnPoint);
        if (_player != null)
        {
            if (_player.TryGetComponent<Saboteur>(out _saboteur))
            {
                _saboteur.StateChanged += OnPlayerStateChanged;
            }
            else
            {
                _saboteur = null;
            }
            CameraDirector.Instance.SetNewPlayer(_player);
        }
    }

    void Update()
    {
        if (_state == GAME_STATE.PLAYING) PlayingUpdate();


        DoStateActions();

    }

    private void PlayingUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.F1)) LoadNextScene();

        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();

        //if(Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    SpawnPlayer();
        //}

        if ((Input.GetKeyDown(KeyCode.Alpha1)) || (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_0) == ZekiController.BUTTON_STATE.ON))
        {
            if (_skills[(int)Skill.SkillType.PRECISION].State == Skill.SkillState.READY)
            {
                _skills[(int)Skill.SkillType.PRECISION].UseSkill();
                OnSkillUsed(Skill.SkillType.PRECISION);
            }
        }

        if ((Input.GetKeyDown(KeyCode.Alpha2)) || (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_1) == ZekiController.BUTTON_STATE.ON))
        {
            if (_skills[(int)Skill.SkillType.SAFEOS].State == Skill.SkillState.READY)
            {
                _skills[(int)Skill.SkillType.SAFEOS].UseSkill();
                OnSkillUsed(Skill.SkillType.SAFEOS);
            }
        }

        if ((Input.GetKeyDown(KeyCode.Alpha3)) || (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_2) == ZekiController.BUTTON_STATE.ON))
        {
            if (_skills[(int)Skill.SkillType.FAULT_INJECT].State == Skill.SkillState.READY)
            {
                _skills[(int)Skill.SkillType.FAULT_INJECT].UseSkill();
                OnSkillUsed(Skill.SkillType.FAULT_INJECT);
            }
        }

        if ((Input.GetKeyDown(KeyCode.Alpha4)) || (ZekiController.Instance.GetButtonState(ZekiController.BUTTON.BUTTON_3) == ZekiController.BUTTON_STATE.ON))
        {
            if (_skills[(int)Skill.SkillType.REDUNDANCY].State == Skill.SkillState.READY)
            {
                _skills[(int)Skill.SkillType.REDUNDANCY].UseSkill();
                OnSkillUsed(Skill.SkillType.REDUNDANCY);
            }
        }
    }

    public void OnDroneDestroy()
    {
        _droneLevelCount++;
        if (_droneLevelCount == _droneTotalCount)
        {

            if (_currentScene == _sceneNames.Count - 1)
            {
                DoTransition(GAME_STATE.GAMEOVER);
            }
            else
            {
                DoTransition(GAME_STATE.LEVEL_COMPLETE);
            }
        }
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

    private void DoStateActions()
    {
        switch (_state)
        {
            case GAME_STATE.INIT:
                break;
            case GAME_STATE.RESET:
                DoTransition(GAME_STATE.MENU);
                break;
            case GAME_STATE.MENU:
                break;
            case GAME_STATE.PLAYING:
                break;
            case GAME_STATE.GAMEOVER:
                break;
            case GAME_STATE.LEVEL_COMPLETE:
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DoTransition(GAME_STATE newState)
    {
        if (_state == newState) return;


        switch (newState)
        {
            case GAME_STATE.INIT:
                break;
            case GAME_STATE.RESET:
                DoExitActions(_state);
                DoEntryActions(newState);
                _state = newState;
                break;
            case GAME_STATE.MENU:
                DoExitActions(_state);
                DoEntryActions(newState);
                _state = newState;
                break;
            case GAME_STATE.PLAYING:
                if ((_state == GAME_STATE.MENU) || (_state == GAME_STATE.LEVEL_COMPLETE))
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
            case GAME_STATE.LEVEL_COMPLETE:
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

                break;
            case GAME_STATE.RESET:

                break;
            case GAME_STATE.MENU:
                CanvasManager.Instance.UnLoadCanvas(CanvasManager.CANVAS.MENU, () => { CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.MENU, false); });
                break;
            case GAME_STATE.PLAYING:
                CanvasManager.Instance.UnLoadCanvas(CanvasManager.CANVAS.IN_PLAY, () => { CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.IN_PLAY, false); });

                break;
            case GAME_STATE.GAMEOVER:
                break;
            case GAME_STATE.LEVEL_COMPLETE:
                CanvasManager.Instance.UnLoadCanvas(CanvasManager.CANVAS.LEVEL_COMPLETE, () => { CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.LEVEL_COMPLETE, false); });
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
            case GAME_STATE.RESET:
                CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.GAME_OVER, false);
                CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.IN_PLAY, false);
                CanvasManager.Instance.SetCanvasEnable(CanvasManager.CANVAS.LEVEL_COMPLETE, false);
                _playerLives = _playerStartLives;
                _droneSurvivedCount = 0;
                _droneTotalCount = 0;
                _droneLevelCount = 0;
                _score = 0;
                UpdateScore(0, false);
                UpdateLivesText();
                DisableSkills();
                LoadStartScene();
                break;
            case GAME_STATE.MENU:
                CameraDirector.Instance.SetCamera(CameraDirector.CameraList.MenuCam);
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.MENU);
                break;
            case GAME_STATE.PLAYING:
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.IN_PLAY);
                SpawnPlayer();
                if (_droneArmyManager != null) _droneArmyManager.StartSpawning();
                break;
            case GAME_STATE.GAMEOVER:
                CameraDirector.Instance.SetCamera(CameraDirector.CameraList.MenuCam);
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.GAME_OVER);
                break;
            case GAME_STATE.LEVEL_COMPLETE:
                CameraDirector.Instance.SetCamera(CameraDirector.CameraList.MenuCam);
                CanvasManager.Instance.LoadCanvas(CanvasManager.CANVAS.LEVEL_COMPLETE);
                DisableSkills();
                break;
            case GAME_STATE.NUM_OF_STATES:
            default:
                break;
        }
    }

    private void DisableSkills()
    {
        foreach(Skill skill in _skills)
        {
            if (skill != null) skill.SkillReset();
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

    public void ResetGame()
    {
        DoTransition(GAME_STATE.RESET);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadNextScene()
    {

        _currentScene++;
        if (_currentScene < _sceneNames.Count)
        {
            foreach (Skill skill in _skills)
            {
                skill.SkillReset();
            }
            _player = null;
            _droneLevelCount = 0;

            SceneManager.LoadScene(_sceneNames[_currentScene]);
            //DoTransition(GAME_STATE.PLAYING);
        }
        else { _currentScene--; }
    }

    private void UpdateLivesText()
    {
        if (_livesValueText != null) _livesValueText.text = _playerLives.ToString("0");
    }

    private void ResetScene()
    {
        foreach (Skill skill in _skills)
        {
            skill.SkillReset();
        }
        SceneManager.LoadScene(_sceneNames[_currentScene]);
    }

    private void LoadStartScene()
    {
        foreach (Skill skill in _skills)
        {
            skill.SkillReset();
        }
        _currentScene = 0;
        SceneManager.LoadScene(_sceneNames[_currentScene]);
    }

    protected virtual void OnSkillUsed(Skill.SkillType _skill)
    {
        SkillUsed?.Invoke(this, _skill);
    }

    protected virtual void OnPlayerStateChanged(object sender, SaboteurStateChangedEventArgs saboteurStateChangedEventArgs)
    {
        switch (saboteurStateChangedEventArgs.state)
        {
            case Saboteur.SaboteurState.INIT:
                break;
            case Saboteur.SaboteurState.MATERIALIZING:
                break;
            case Saboteur.SaboteurState.ROAMING:
                break;
            case Saboteur.SaboteurState.LEARNING:
                break;
            case Saboteur.SaboteurState.SABOTAGING:
                break;
            case Saboteur.SaboteurState.DYING:
                break;
            case Saboteur.SaboteurState.DEAD:

                if (_saboteur != null) _saboteur.StateChanged -= OnPlayerStateChanged;

                _playerLives = (uint)Mathf.Clamp((int)_playerLives - 1, (int)0, (int)_playerStartLives);

                if (_playerLives > 0)
                {
                    _player = null;
                    _saboteur = null;
                    SpawnPlayer();
                }
                else
                {
                    DoTransition(GAME_STATE.GAMEOVER);
                }
                UpdateLivesText();
                break;
            case Saboteur.SaboteurState.NUM_OF_STATES:
            default:
                break;
        }


    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_state != GAME_STATE.LEVEL_COMPLETE) return;
        if(scene.name == _sceneNames[_currentScene])
        {
            DoTransition(GAME_STATE.PLAYING);
        }
    }
}



