using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAudioListener : MonoBehaviour
{
    private GameObject _player;

    private Coroutine _findPlayerCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_player == null)
        {
            if (_findPlayerCoroutine == null) _findPlayerCoroutine = StartCoroutine(FindPlayer());
        }

        if(_player != null)
        {
            transform.position = _player.transform.position;
            transform.rotation = _player.transform.rotation;
        }

    }

    //replace below with threading as coroutine runs in same frame thread anyway and cannot do a performant yield from FindWithTag
    IEnumerator FindPlayer()
    {
        _player = GameObject.FindWithTag("PlayerAudioListener");
        _findPlayerCoroutine = null;
        yield return null;

    }

}
