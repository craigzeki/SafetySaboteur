using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZekstersLab.Helpers;
using PathCreation;

public class DroneArmyManager : MonoBehaviour
{
    public enum DroneOrderOption
    {
        SEQUENTIAL = 0,
        RANDOM = 1,
        NUM_OF_OPTIONS
    }

    [SerializeField] GameObject[] _dronePrefabs;
    [SerializeField] uint[] _droneQtys;
    [SerializeField] PathCreator _pathCreator;
    [SerializeField] float _timeBetweenSpawns = 5f; //seconds
    [SerializeField] DroneOrderOption _droneOrderOption = DroneOrderOption.SEQUENTIAL;
    [SerializeField] uint _score = 0;


    private List<GameObject> _droneOrder = new List<GameObject>();
    private uint _droneId;
    private Coroutine _SpawnDrones;

    private void Awake()
    {
        CreateDroneList(_droneOrderOption);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            StartSpawning();
        }

        if(Input.GetKeyUp(KeyCode.Escape))
        {
            StopSpawning();
        }
    }


    private void CreateDroneList(DroneOrderOption orderOption)
    {
        if (_dronePrefabs.Length != _droneQtys.Length) return;
        if (_droneQtys == null) return;
        if (_dronePrefabs == null) return;
        if (_droneOrderOption >= DroneOrderOption.NUM_OF_OPTIONS) return;

        for (uint i = 0; i < _dronePrefabs.Length; i++)
        {
            for (uint y = 0; y < _droneQtys[i]; y++)
            {
                _droneOrder.Add(_dronePrefabs[i]);
            }
        }

        if(orderOption == DroneOrderOption.RANDOM) _droneOrder.Shuffle();
    }

    public void StartSpawning()
    {
        if(_SpawnDrones == null) _SpawnDrones = StartCoroutine(SpawnDrones());
    }

    public void StopSpawning()
    {
        if (_SpawnDrones != null)
        {
            StopCoroutine(_SpawnDrones);
            _SpawnDrones = null;
        }

    }

    IEnumerator SpawnDrones()
    {
        DroneManager droneMovement = null;
        _droneId = 0;

        foreach(GameObject dronePrefab in _droneOrder)
        {
            GameObject drone = Instantiate(dronePrefab, this.transform);
            if(drone != null)
            {
                
                if(drone.TryGetComponent<DroneManager>(out droneMovement))
                {
                    droneMovement.DroneId = ++_droneId;
                    droneMovement.Moving = true;
                    droneMovement.PathCreator = _pathCreator;
                    droneMovement.DroneSurvived += OnDroneSurvived;
                }
                else
                {
                    //drone did not contain correct components - cannot be moved on the path - destroy it
                    Destroy(drone);
                }
            }
            yield return new WaitForSeconds(_timeBetweenSpawns);
        }
    }

    private void OnDroneSurvived(object sender, uint points)
    {
        DroneManager droneMovement = (DroneManager)sender;
        droneMovement.DroneSurvived -= OnDroneSurvived;
        Destroy(droneMovement.gameObject);
        _score += points;
    }
}
