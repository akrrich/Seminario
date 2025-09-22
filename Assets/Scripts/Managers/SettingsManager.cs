using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : Singleton<SettingsManager>
{
    [Header("Audio")]
    [SerializeField] private AudioMixerGroup genralMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup SFXMixerGroup;

    private float generalVolume;
    private float musicVolume;
    private float SFXVolume;

    public float GeneralVolume { get => generalVolume; }
    public float MusicVolume { get => musicVolume; }
    public float SFXVOlume { get => SFXVolume; }

    [Header("Video")]
    private bool showFPS = false;
    private int targetFPS = 60;
    private Resolution currentResolution;
    private FullScreenMode fullscreenMode = FullScreenMode.FullScreenWindow;
    private int qualityLevel = 2;
    private bool vSync = true;

    public bool ShowFPS { get => showFPS; set => showFPS = value; }
    public int TargetFPS { get => targetFPS; set => targetFPS = value; }
    public Resolution CurrentResolution { get => currentResolution; set => currentResolution = value; }
    public FullScreenMode FullscreenMode { get => fullscreenMode; set => fullscreenMode = value; }
    public int QualityLevel { get => qualityLevel; set => qualityLevel = value; } // predeterminado Medio
    public bool VSync { get => vSync; set => vSync = value; }

    [Header("Controls")]
    private float sensitivityMouseX;
    private float sensitivityMouseY;
    private float sensitivityJoystickX;
    private float sensitivityJoystickY;

    public float SensitivityMouseX { get => sensitivityMouseX; set => sensitivityMouseX = value; }
    public float SensitivityMouseY { get => sensitivityMouseY; set => sensitivityMouseY = value; }
    public float SensitivityJoystickX { get => sensitivityJoystickX; set => sensitivityJoystickX = value; }
    public float SensitivityJoystickY { get => sensitivityJoystickY; set => sensitivityJoystickY = value; }


    void Awake()
    {
        CreateSingleton(true);
    }

    void Start()
    {
        LoadAudioValuesFromPlayerPrefs();
        LoadVideoValuesFromPlayerPrefs();
        LoadControlsValuesFromPlayerPrefs();
        ApplyAudioSettings();
        ApplyVideoSettings();
        //ApplyControlSettings();
    }


    public void SetGeneralVolume(float value)
    {
        generalVolume = value;
        ApplyVolumeToSlider(genralMixerGroup, "General", value);
        PlayerPrefs.SetFloat("GeneralVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        ApplyVolumeToSlider(musicMixerGroup, "Music", value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SFXVolume = value;
        ApplyVolumeToSlider(SFXMixerGroup, "SFX", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetShowFPS(bool value)
    {
        showFPS = value;
        PlayerPrefs.SetInt("ShowFPS", value ? 1 : 0);
    }

    public void SetTargetFPS(int fps)
    {
        targetFPS = fps;
        Application.targetFrameRate = fps;
        PlayerPrefs.SetInt("TargetFPS", fps);
    }

    public void SetResolution(int width, int height, FullScreenMode mode)
    {
        currentResolution.width = width;
        currentResolution.height = height;
        fullscreenMode = mode;
        Screen.SetResolution(width, height, mode);
        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
        PlayerPrefs.SetInt("FullscreenMode", (int)mode);
    }

    public void SetQualityLevel(int index)
    {
        qualityLevel = index;
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("QualityLevel", index);
    }

    public void SetVSync(bool value)
    {
        vSync = value;
        QualitySettings.vSyncCount = value ? 1 : 0;
        PlayerPrefs.SetInt("VSync", value ? 1 : 0);
    }

    public void SetSensitivityMouseX(float value)
    {
        sensitivityMouseX = value;
        PlayerInputs.Instance.KeyboardInputs.SensitivityX = sensitivityMouseX;
        PlayerPrefs.SetFloat("SensitivityMouseX", value);
    }

    public void SetSensitivityMouseY(float value)
    {
        sensitivityMouseY = value;
        PlayerInputs.Instance.KeyboardInputs.SensitivityY = sensitivityMouseY;
        PlayerPrefs.SetFloat("SensitivityMouseY", value);
    }

    public void SetSensitivityJoystickX(float value)
    {
        sensitivityJoystickX = value;
        PlayerInputs.Instance.JoystickInputs.SensitivityX = sensitivityJoystickX;
        PlayerPrefs.SetFloat("SensitivityJoystickX", value);
    }

    public void SetSensitivityJoystickY(float value)
    {
        sensitivityJoystickY = value;
        PlayerInputs.Instance.JoystickInputs.SensitivityY = sensitivityJoystickY;
        PlayerPrefs.SetFloat("SensitivityJoystickY", value);
    }


    private void LoadAudioValuesFromPlayerPrefs()
    {
        float defaultValue = 0.5f;

        generalVolume = PlayerPrefs.GetFloat("GeneralVolume", defaultValue);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultValue);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", defaultValue);
    }

    private void ApplyAudioSettings()
    {
        SetGeneralVolume(generalVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(SFXVolume);
    }

    private void ApplyVolumeToSlider(AudioMixerGroup group, string exposedParam, float value)
    {
        if (value <= 0.01f)
        {
            group.audioMixer.SetFloat(exposedParam, -80f);
        }

        else
        {
            group.audioMixer.SetFloat(exposedParam, Mathf.Log10(value) * 20);
        }
    }

    private void LoadVideoValuesFromPlayerPrefs()
    {
        showFPS = PlayerPrefs.GetInt("ShowFPS", 0) == 1;
        targetFPS = PlayerPrefs.GetInt("TargetFPS", 60);
        Application.targetFrameRate = TargetFPS;

        int width = PlayerPrefs.GetInt("ResolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("ResolutionHeight", Screen.currentResolution.height);
        int mode = PlayerPrefs.GetInt("FullscreenMode", (int)FullScreenMode.FullScreenWindow);
        currentResolution = new Resolution { width = width, height = height };
        fullscreenMode = (FullScreenMode)mode;
        Screen.SetResolution(width, height, fullscreenMode);

        qualityLevel = PlayerPrefs.GetInt("QualityLevel", 2);
        QualitySettings.SetQualityLevel(qualityLevel);

        vSync = PlayerPrefs.GetInt("VSync", 1) == 1;
        QualitySettings.vSyncCount = vSync ? 1 : 0;
    }

    private void ApplyVideoSettings()
    {
        SetTargetFPS(targetFPS);
        SetResolution(currentResolution.width, currentResolution.height, fullscreenMode);
        SetQualityLevel(qualityLevel);
        SetVSync(vSync);
    }

    private void LoadControlsValuesFromPlayerPrefs()
    {
        float defaultMouse = 10f;
        float defaultJoystick = 10f;

        sensitivityMouseX = PlayerPrefs.GetFloat("SensitivityMouseX", defaultMouse);
        sensitivityMouseY = PlayerPrefs.GetFloat("SensitivityMouseY", defaultMouse);
        sensitivityJoystickX = PlayerPrefs.GetFloat("SensitivityJoystickX", defaultJoystick);
        sensitivityJoystickY = PlayerPrefs.GetFloat("SensitivityJoystickY", defaultJoystick);
    }

    private void ApplyControlSettings()
    {
        SetSensitivityMouseX(sensitivityMouseX);
        SetSensitivityMouseY(sensitivityMouseY);
        SetSensitivityJoystickX(sensitivityJoystickX);
        SetSensitivityJoystickY(sensitivityJoystickY);
    }
}
