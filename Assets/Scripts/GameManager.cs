using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;
    
    private GameObject _player;

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



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            if ((_playerSpawnPoint != null) && (_player == null)) _player = Instantiate(_playerPrefab, _playerSpawnPoint);
            if (_player != null) CameraDirector.Instance.SetNewPlayer(_player);
        }
    }
}
