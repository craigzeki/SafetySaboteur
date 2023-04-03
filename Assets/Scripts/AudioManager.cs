//From Introduction to Audio in Unity by Brackeys - https://www.youtube.com/watch?v=6OT43pvUyfY&ab_channel=Brackeys

using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    [SerializeField] private bool _dontDestroyOnLoad = false;
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

    public void Play (string name)
    {
        Sound sound = Array.Find(sounds, sound => sound.Name == name);
        if(sound != null) sound.Source.Play();
    }
}
