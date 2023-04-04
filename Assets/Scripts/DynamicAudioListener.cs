using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAudioListener : MonoBehaviour
{
    [SerializeField] float _rotationSpeed = .5f;


    private Transform _playerAudioListenerPoint;

    private Coroutine _findPlayerCoroutine;
    private float _duration = 0;
    private Quaternion _target;

    // Update is called once per frame
    void Update()
    {
        if((_playerAudioListenerPoint == null) && (GameManager.Instance.Player != null))
        {
            _playerAudioListenerPoint = GameManager.Instance.Player.GetComponent<Saboteur>().PlayerAudioListener;
        }

        if(_playerAudioListenerPoint != null)
        {
            transform.position = _playerAudioListenerPoint.transform.position;
            _target = _playerAudioListenerPoint.transform.rotation;
            transform.rotation = Quaternion.Lerp(transform.rotation, _target, _rotationSpeed * Time.deltaTime); ;
        }

    }

}
