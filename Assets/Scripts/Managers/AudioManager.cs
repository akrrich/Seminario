using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private AudioMixerGroup _sfxMixerGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource; 
    [SerializeField] private AudioSource _sfxSource;

    [Header("Clips")]
    [SerializeField] private Sound[] _musicClips; // Lista de música con nombre
    [SerializeField] private Sound[] _sfxClips;   // Lista de SFX con nombre

    private Dictionary<string, AudioClip> _musicDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _sfxDictionary = new Dictionary<string, AudioClip>();


    void Awake()
    {
        CreateSingleton();
        InitializeAudiosDictionaries(_musicDictionary, _musicClips);
        InitializeAudiosDictionaries(_sfxDictionary, _sfxClips);
    }

    public void PlayMusic(string musicName)
    {
        if (!_musicDictionary.ContainsKey(musicName))
        {
            Debug.LogWarning("Música no encontrada: " + musicName);
            return;
        }

        _musicSource.clip = _musicDictionary[musicName];
        _musicSource.loop = true;
        _musicSource.Play();
    }

    public void PlaySFX(string sfxName)
    {
        if (!_sfxDictionary.ContainsKey(sfxName))
        {
            Debug.LogWarning("SFX no encontrado: " + sfxName);
            return;
        }

        _sfxSource.clip = _sfxDictionary[sfxName];
        _sfxSource.PlayOneShot(_sfxSource.clip);
    }
    public void PlaySFXAtPosition(string sfxName, Vector3 position, float spatialBlend = 1f, float volume = 1f, float minDistance = 1f, float maxDistance = 20f)
    {
        if (!_sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            Debug.LogWarning("SFX no encontrado: " + sfxName);
            return;
        }

        GameObject tempGO = new GameObject("TempSFX_" + sfxName);
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.outputAudioMixerGroup = _sfxMixerGroup;
        aSource.clip = clip;
        aSource.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
        aSource.volume = volume;
        aSource.minDistance = minDistance;
        aSource.maxDistance = maxDistance;
        aSource.rolloffMode = AudioRolloffMode.Linear;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
    public float returnAudioLength(string sfxName)
    {
        if (_sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            return clip.length;
        }

        Debug.LogWarning("SFX no encontrado: " + sfxName);
        return 0f;
    }

    public void SetMusicVolume(float volume)
    {
        _musicMixerGroup.audioMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxMixerGroup.audioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }


    private void CreateSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudiosDictionaries(Dictionary<string, AudioClip> audioDic, Sound[] soundType)
    {
        foreach (var sounds in soundType)
        {
            audioDic[sounds.name] = sounds.clip;
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}