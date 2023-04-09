//From Introduction to Audio in Unity by Brackeys - https://www.youtube.com/watch?v=6OT43pvUyfY&ab_channel=Brackeys

using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private bool _dontDestroyOnLoad = false;
    [SerializeField][Range(0.0f, 1.0f)] private float _bgMusicVolume = 0.7f;
    public Sound[] bgMusic;
    public Sound[] sounds;

    public static AudioManager Instance
    {
        get
        {
            if(instance == null) instance = FindObjectOfType<AudioManager>();
            return instance;
        }
    }

    void Awake()
    {
        if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        sounds = FindObjectsOfType<Sound>();

        //foreach(Sound sound in sounds)
        //{
            //sound.Source = gameObject.AddComponent<AudioSource>();
            //sound.Source.clip = sound.Clip;
            //sound.Source.volume = sound.Volume;
            //sound.Source.pitch = sound.Pitch;
            //sound.Source.loop = sound.Loop;
        //}
    }
    private void Start()
    {
        PlayBG();
    }

    public void Play (string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.Name == name);
        if(sound != null) sound.Source.Play();
    }

    private void PlayBG()
    {
        if (bgMusic.Length > 0) StartCoroutine(BGMusicPlayer());
    }

    IEnumerator BGMusicPlayer()
    {
        int currentTrack = 0;
        int nextTrack = 0;

        while( true)
        {
            if(bgMusic[currentTrack] != null)
            {
                bgMusic[currentTrack].Source.volume = _bgMusicVolume;
                if (!bgMusic[currentTrack].Source.isPlaying)
                {
                    bgMusic[nextTrack].Source.Play();
                    currentTrack = nextTrack;
                    nextTrack++;
                    if (nextTrack >= bgMusic.Length) nextTrack = 0;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
