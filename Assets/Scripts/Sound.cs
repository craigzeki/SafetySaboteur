//From Introduction to Audio in Unity by Brackeys - https://www.youtube.com/watch?v=6OT43pvUyfY&ab_channel=Brackeys

using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    public string Name;
    //public AudioClip Clip;
    //public bool Loop = false;
    //[Range(0f, 1f)]
    //public float Volume = 1.0f;
    //[Range (0.1f, 3f)]
    //public float Pitch = 1.0f;
    //[Range(0f, 1f)]
    //public float SpatialBlend = 0f;
    //public float MinDistance = 1;
    //public float MaxDistance = 500;
    

    [HideInInspector]
    public AudioSource Source;

    private void Awake()
    {
        Source = GetComponent<AudioSource>();
    }
}
