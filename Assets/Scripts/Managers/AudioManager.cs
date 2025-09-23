using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource SFXSource;

    [Header("Clips")]
    [SerializeField] private Sound[] musicClips; // Lista de m�sica con nombre
    [SerializeField] private Sound[] SFXClips;   // Lista de SFX con nombre

    private Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> SFXDictionary = new Dictionary<string, AudioClip>();


    void Awake()
    {
        CreateSingleton(true);
        InitializeAudiosDictionaries(musicDictionary, musicClips);
        InitializeAudiosDictionaries(SFXDictionary, SFXClips);
    }


    public void PlayMusic(string musicName)
    {
        if (!musicDictionary.ContainsKey(musicName))
        {
            Debug.LogWarning("M�sica no encontrada: " + musicName);
            return;
        }

        musicSource.clip = musicDictionary[musicName];
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        if (!SFXDictionary.ContainsKey(sfxName))
        {
            Debug.LogWarning("SFX no encontrado: " + sfxName);
            return;
        }

        SFXSource.clip = SFXDictionary[sfxName];
        SFXSource.PlayOneShot(SFXSource.clip);
    }

    /*public float returnAudioLength(string sfxName)
    {
        if (SFXDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            return clip.length;
        }

        Debug.LogWarning("SFX no encontrado: " + sfxName);
        return 0f;
    }*/

    private void InitializeAudiosDictionaries(Dictionary<string, AudioClip> audioDic, Sound[] soundType)
    {
        foreach (var sounds in soundType)
        {
            audioDic[sounds.name] = sounds.clip;
        }
    }
}


[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}
