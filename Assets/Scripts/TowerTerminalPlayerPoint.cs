using RPGCharacterAnims;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class TowerTerminalPlayerPoint : MonoBehaviour
{
    [SerializeField] private Outline _outline;
    [SerializeField] private Transform _playerRotation;
    private RPGCharacterController _rpgCharacterController;
    private bool _playerLocked = false;

    private void Start()
    {
        _outline.enabled = false;
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
}
