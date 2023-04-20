using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private bool _started = false;
    private AudioSource _audioSource;
    public void StartAudio()
    {
        _audioSource.Play();
        _started = true;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(_started)
        {
            if(!_audioSource.isPlaying)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
