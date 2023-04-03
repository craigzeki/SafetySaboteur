using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _playerPrefab;

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<GameManager>();
            return instance;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        if (_playerSpawnPoint != null) Instantiate(_playerPrefab, _playerSpawnPoint);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
