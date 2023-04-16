using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using TMPro;

public class CameraDirector : MonoBehaviour
{
    public enum CameraList
    {
        MenuCam = 0,
        SpawnCam,
        RoamingCam,
        ShoulderCam,
        DeathCam,
        //FinishGameCam,
        NumOfCams
    }

    public enum CameraPriority
    {
        Low = 0,
        High = 1,
        NumOfPriorities
    }

    [SerializeField] private CinemachineBrain cameraBrain;
    [SerializeField] private CinemachineVirtualCamera[] cameraList = new CinemachineVirtualCamera[(int)CameraList.NumOfCams];
    [SerializeField] private bool[] cameraFollowPlayer = new bool[(int)CameraList.NumOfCams];
    [SerializeField] private bool[] cameraLookAtPlayer = new bool[(int)CameraList.NumOfCams];

    private Saboteur _saboteur;

    private static CameraDirector instance;
    

    public static CameraDirector Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<CameraDirector>();
            }
            return instance;
        }
    }

    void Awake()
    {
        SetCamera(CameraList.MenuCam);

    }

    private void Start()
    {
        cameraBrain = GameManager.Instance.MainCam;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void SetCutCam(CinemachineVirtualCamera vCam)
    //{
    //    cameraList[(int)CameraList.CutSceneCam] = vCam;
    //    SetCamera(CameraList.CutSceneCam);
    //}

    public void SetCamera(CameraList newCam)
    {
        for (int i = 0; i < (int)CameraList.NumOfCams; i++)
        {
            if(cameraList[i] != null)
            {
                //cameraList[i].Priority = (i == (int)newCam) ? (int)CameraPriority.High : (int)CameraPriority.Low;
                cameraList[i].Priority = (int)CameraPriority.Low;
            }
            else
            {
                Debug.LogError("Error: cameraList[" + i.ToString() + "] is null - should be a virtual cam");
            }
            
        }
        cameraList[(int)newCam].Priority = (int)CameraPriority.High;
    }

    public void SetNewPlayer(GameObject player)
    {
        if (player == null) return;

        for (int i = 0; i < (int)CameraList.NumOfCams; i++)
        {
            if(cameraList[i] != null)
            {
                if (cameraFollowPlayer[i]) cameraList[i].Follow = player.transform;
                if (cameraLookAtPlayer[i]) cameraList[i].LookAt = player.transform;
            }
            else
            {
                Debug.LogError("Error: cameraList[" + i.ToString() + "] is null - should be a virtual cam");
            }
        }
        if(player.TryGetComponent<Saboteur>(out _saboteur))
        {
            _saboteur.StateChanged += OnPlayerStateChanged;
            if (cameraList[(int)CameraList.DeathCam] != null)
            {
                if (cameraFollowPlayer[(int)CameraList.DeathCam]) cameraList[(int)CameraList.DeathCam].Follow = _saboteur.DeathCam;
                if (cameraLookAtPlayer[(int)CameraList.DeathCam]) cameraList[(int)CameraList.DeathCam].LookAt = _saboteur.DeathCam;
            }
        }
        
    }

    public bool GetIsLive(CameraList vCam)
    {
        return CinemachineCore.Instance.IsLive(cameraList[(int)vCam]);
        
    }

    private void OnPlayerStateChanged(object sender, SaboteurStateChangedEventArgs e)
    {
        switch (e.state)
        {
            case Saboteur.SaboteurState.INIT:
                break;
            case Saboteur.SaboteurState.MATERIALIZING:
                SetCamera(CameraList.SpawnCam);
                break;
            case Saboteur.SaboteurState.ROAMING:
                SetCamera(CameraList.RoamingCam);
                break;
            case Saboteur.SaboteurState.LEARNING:
                SetCamera(CameraList.ShoulderCam);
                break;
            case Saboteur.SaboteurState.SABOTAGING:
                SetCamera(CameraList.ShoulderCam);
                break;
            case Saboteur.SaboteurState.DYING:
                SetCamera(CameraList.DeathCam);
                break;
            case Saboteur.SaboteurState.DEAD:
                SetCamera(CameraList.RoamingCam);
                _saboteur.StateChanged -= OnPlayerStateChanged;
                break;
            case Saboteur.SaboteurState.NUM_OF_STATES:
            default:
                break;
        }
    }
}
