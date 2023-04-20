using UnityEngine;
using PathCreation;
using System.Collections;
using ZekstersLab.DebugLib;
using System;

// Moves along a path at constant speed.
// Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
public class DroneManager : MonoBehaviour, iTakesDamage
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _startHealth = 100;
    [SerializeField] private uint _points;
    [SerializeField] private PathCreator _pathCreator;
    [SerializeField] private EndOfPathInstruction _endOfPathInstruction;
    [SerializeField] private float _speed = 5;
    private float _distanceTravelled;

    [SerializeField] private bool _hoverAnimationEnabled = false;
    [SerializeField] private float _hoverPeriod = 1f; //in seconds
    [SerializeField] private float _hoverYOffset = 0.2f;
    [SerializeField] private bool _moving = false;
    [SerializeField] private uint _droneID;
    [SerializeField] private float _survivedDistanceThreshold = 0.1f;

    //[SerializeField] private AudioSource _explosionAudio;
    [SerializeField] private GameObject _audioPrefab;
    private AudioPlayer _audioPlayer;
    private GameObject _audioPlayerGO;

    private Coroutine _lerpCoroutine;
    private float _lerpTimeElapsed = 0f;
    private float _lerpT = 0f;
    private float _hoverPositionY = 0f;

    private int _currentHealth = 0;
    private bool _isDead = false;

    private Vector3 _newPosition = Vector3.zero;
    private float _pathY = 0f;

    private DroneManager _otherDroneManager;
    private Coroutine _explosionCoroutine;

    public bool Moving { get => _moving; set => _moving = value; }
    public uint DroneId { get => _droneID; set => _droneID = value; }
    public PathCreator PathCreator { get => _pathCreator; set => _pathCreator = value; }
    public float Speed { get => _speed; set => _speed = value; }

    public event EventHandler<uint> DroneSurvived;

    private void Awake()
    {
        _currentHealth = _startHealth;
        if (_healthBar != null) _healthBar.OnTargetHealthReached += HealthBarReachedTarget;
        if (_audioPrefab != null) _audioPlayerGO = Instantiate(_audioPrefab, transform.position, transform.rotation);
        if (_audioPlayerGO != null) _audioPlayer = _audioPlayerGO.GetComponent<AudioPlayer>();
    }

    void Start()
    {
        if (_pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            _pathCreator.pathUpdated += OnPathChanged;
            transform.position = _pathCreator.path.GetPointAtDistance(0, _endOfPathInstruction);
            transform.rotation = _pathCreator.path.GetRotationAtDistance(0, _endOfPathInstruction);
            
        }

        _pathY = transform.position.y;

        StartStopHover();

    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GAME_STATE.PLAYING) return;
        if (_pathCreator != null && _moving)
        {
            _distanceTravelled += _speed * Time.deltaTime;
            
            transform.position = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction);
            transform.rotation = _pathCreator.path.GetRotationAtDistance(_distanceTravelled, _endOfPathInstruction);

            if(Vector3.Distance(transform.position, _pathCreator.path.GetPoint(_pathCreator.path.NumPoints-1)) <= _survivedDistanceThreshold) OnDroneSurvived();

            _pathY = _pathCreator.path.GetPointAtDistance(_distanceTravelled, _endOfPathInstruction).y;
        }
        
        

        _newPosition = transform.position;
        _newPosition.y = _hoverPositionY;
        transform.position = _newPosition;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);
        SetHealthBar();
    }

    private void SetHealthBar()
    {
        if (_healthBar == null) return;
        _healthBar.SetHealthPercent((float)((float)_currentHealth / (float)_maxHealth));
    }

    protected virtual void OnDroneSurvived()
    {
        DroneSurvived?.Invoke(this, ((uint)Mathf.RoundToInt((_points * _currentHealth) / _maxHealth)));

    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        _distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }

    private void StartStopHover()
    {
        if(_lerpCoroutine != null)
        {
            StopCoroutine(_lerpCoroutine);
            _lerpCoroutine = null;
        }
        else
        {
            _lerpCoroutine = StartCoroutine(DoHover());
        }
    }

    

    private CSV_Debug _csvDebug = new CSV_Debug(CSV_Debug.CSV_DebugState.DISABLED); //provides loging of data to CSV, if enabled (should be disabled for production)

    IEnumerator DoHover()
    {
        string[] headers = { "_lerpT", "_lerp1","_lerp2", "_hoverPositionY" };
        _csvDebug.SetFilename("DoHover");
        _csvDebug.SetHeaders(headers);

        while (_hoverAnimationEnabled)
        {
            _lerpTimeElapsed = 0;
            //do upward motion
            while (_lerpTimeElapsed < (_hoverPeriod / 2))
            {
                _lerpT = _lerpTimeElapsed / (_hoverPeriod / 2);
                _lerpT = _lerpT * _lerpT * (3f - (2f * _lerpT)); //smooth step
                _hoverPositionY = Mathf.Lerp(_pathY - _hoverYOffset, _pathY + _hoverYOffset, _lerpT);
                
                _csvDebug.WriteData(new string[] {_lerpT.ToString(), (_pathY - _hoverYOffset).ToString(),(_pathY + _hoverYOffset).ToString(), _hoverPositionY.ToString() });
                _lerpTimeElapsed += Time.deltaTime;
                yield return null;
            }
            //_hoverPosition.y = _hoverYOffset;
            _hoverPositionY = _pathY + _hoverYOffset;
            _csvDebug.WriteData(new string[] { _lerpT.ToString(), (_pathY - _hoverYOffset).ToString(), (_pathY + _hoverYOffset).ToString(), _hoverPositionY.ToString() });
            //do downward motion
            _lerpTimeElapsed = 0;
            
            while (_lerpTimeElapsed < (_hoverPeriod / 2))
            {
                _lerpT = _lerpTimeElapsed / (_hoverPeriod / 2);
                _lerpT = _lerpT * _lerpT * (3f - (2f * _lerpT)); //smooth step
                _hoverPositionY = Mathf.Lerp(_pathY + _hoverYOffset, _pathY - _hoverYOffset, _lerpT);

                _csvDebug.WriteData(new string[] { _lerpT.ToString(), (_pathY + _hoverYOffset).ToString(), (_pathY - _hoverYOffset).ToString(), _hoverPositionY.ToString() });
                _lerpTimeElapsed += Time.deltaTime;
                yield return null;
            }
            _hoverPositionY = transform.position.y - _hoverYOffset;
            _csvDebug.WriteData(new string[] { _lerpT.ToString(), (_pathY - _hoverYOffset).ToString(), (_pathY + _hoverYOffset).ToString(), _hoverPositionY.ToString() });
        }

        _hoverPositionY = 0;
        _csvDebug.CloseFile();
    }

    private void HealthBarReachedTarget(object sender, float percent)
    {
        if(Mathf.Approximately(percent, 0f))
        {
            _isDead = true;

            _audioPlayerGO.transform.position = transform.position;
            _audioPlayerGO.transform.rotation = transform.rotation;
            _audioPlayer.StartAudio();
            Destroy(this.gameObject);
        }
    }

    

    public bool GetIsDead()
    {
        return _isDead;
    }
    
    public int GetHealth()
    {
        return _currentHealth;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnDroneDestroy();
    }

    public Transform GetTransform()
    {
        return this.gameObject.transform;
    }
}

