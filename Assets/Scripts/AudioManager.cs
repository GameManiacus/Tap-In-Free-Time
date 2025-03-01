using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource music;

    [SerializeField] private List<AudioClips> clips;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlaySfx(ClipName clipName)
    {
        sfx.PlayOneShot(clips[(int)clipName].clip);
    }
}


[Serializable]
public class AudioClips
{
    public ClipName clipName;
    public AudioClip clip;
}

public enum ClipName
{
    ButtonClick,
    Shoot
}