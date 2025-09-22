using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SettingsManagerUI : Singleton<SettingsManagerUI>
{
    [Header("General:")]
    [SerializeField] private GameObject panelAudio;
    [SerializeField] private GameObject panelVideo;
    [SerializeField] private GameObject panelControls;
    [SerializeField] private Button buttonAudio;
    [SerializeField] private Button buttonVideo;
    [SerializeField] private Button buttonControls;

    [Header("Audio Options:")]
    [SerializeField] private Slider generalSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private TMP_Text generalPorcentageText;
    [SerializeField] private TMP_Text musicGeneralText;
    [SerializeField] private TMP_Text SFXGeneralText;

    [Header("Video Options:")]
    [SerializeField] private TMP_Dropdown dropdownResolution;
    [SerializeField] private TMP_Dropdown dropdownQuality;
    [SerializeField] private TMP_Dropdown dropdownFPS;
    [SerializeField] private Toggle toggleFullscreen;
    [SerializeField] private Toggle toggleVSync;
    [SerializeField] private Toggle toggleShowFPS;

    [Header("Controls Options:")]
    [SerializeField] private Slider sensitivityMouseXSlider;
    [SerializeField] private Slider sensitivityMouseYSlider;
    [SerializeField] private Slider sensitivityJoystickXSlider;
    [SerializeField] private Slider sensitivityJoystickYSlider;
    [SerializeField] private TMP_Text sensitivityMouseXText;
    [SerializeField] private TMP_Text sensitivityMouseYText;
    [SerializeField] private TMP_Text sensitivityJoystickXText;
    [SerializeField] private TMP_Text sensitivityJoystickYText;


    void Awake()
    {
        CreateSingleton(false);
        SuscribeToMainMenuEvent();
        SuscribeToUpdateManagerEvent();
    }

    void Start()
    {
        InitializeAudioOptions();
        InitializeVideoOptions();
        InitializeControlsOptions();
    }

    // Simulacion de Update
    void UpdateSettingsManagerUI()
    {
        //CheckJoystickInputsToInteractWithPanels();
    }

    void OnDestroy()
    {
        UnsuscribeToMainMenuEvent();
        UnsuscribeToUpdateEvent();
    }


    // Funciones asignadas a OnPointerEnter para el mouse y combinadas con los Inputs del joystick
    // El objetivo es que se abran los paneles correctamente y se cierren correctamente
    public void SetPanelAudio()
    {
        // Color blanco
        ColorBlock color = buttonAudio.colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (buttonAudio.colors == color)
        {
            //buttonSelected.Play();

            SetButtonNormalColorInWhite(buttonVideo);
            SetButtonNormalColorInWhite(buttonControls);

            SetButtonNormalColorInGreen(buttonAudio);
            DisableAllPanels();
            panelAudio.SetActive(true);
        }
    }

    public void SetPanelVideo()
    {
        // Color blanco
        ColorBlock color = buttonAudio.colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (buttonVideo.colors == color)
        {
            //buttonSelected.Play();

            SetButtonNormalColorInWhite(buttonAudio);
            SetButtonNormalColorInWhite(buttonControls);

            SetButtonNormalColorInGreen(buttonVideo);
            DisableAllPanels();
            panelVideo.SetActive(true);
        }
    }

    public void SetPanelControls()
    {
        // Color blanco
        ColorBlock color = buttonAudio.colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        if (buttonControls.colors == color)
        {
            //buttonSelected.Play();

            SetButtonNormalColorInWhite(buttonAudio);
            SetButtonNormalColorInWhite(buttonVideo);

            SetButtonNormalColorInGreen(buttonControls);
            DisableAllPanels();
            panelControls.SetActive(true);
        }
    }


    private void SuscribeToMainMenuEvent()
    {
        MainMenu.OnButtonSettingsClickToShowCorrectPanel += SetPanelAudio;
    }

    private void UnsuscribeToMainMenuEvent()
    {
        MainMenu.OnButtonSettingsClickToShowCorrectPanel -= SetPanelAudio;
    }

    private void SuscribeToUpdateManagerEvent()
    {
        UpdateManager.OnUpdate += UpdateSettingsManagerUI;
    }

    private void UnsuscribeToUpdateEvent()
    {
        UpdateManager.OnUpdate -= UpdateSettingsManagerUI;
    }

    private void InitializeAudioOptions()
    {
        generalSlider.value = SettingsManager.Instance.GeneralVolume;
        musicSlider.value = SettingsManager.Instance.MusicVolume;
        SFXSlider.value = SettingsManager.Instance.SFXVOlume;

        generalSlider.onValueChanged.AddListener(SettingsManager.Instance.SetGeneralVolume);
        musicSlider.onValueChanged.AddListener(SettingsManager.Instance.SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSFXVolume);

        UpdateTextAudio(generalPorcentageText, generalSlider.value);
        UpdateTextAudio(musicGeneralText, musicSlider.value);
        UpdateTextAudio(SFXGeneralText, SFXSlider.value);

        generalSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetGeneralVolume(value);
            UpdateTextAudio(generalPorcentageText, value);
        });

        musicSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetMusicVolume(value);
            UpdateTextAudio(musicGeneralText, value);
        });

        SFXSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SetSFXVolume(value);
            UpdateTextAudio(SFXGeneralText, value);
        });
    }

    private void UpdateTextAudio(TMP_Text currentText, float value)
    {
        currentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    private void InitializeVideoOptions()
    {
        // ---- Resoluciones ----
        dropdownResolution.ClearOptions();
        List<string> resOptions = new List<string>();
        Resolution[] resolutions = Screen.resolutions;
        foreach (var res in resolutions)
            resOptions.Add(res.width + " x " + res.height);
        dropdownResolution.AddOptions(resOptions);

        // ---- Calidad ----
        dropdownQuality.ClearOptions();
        dropdownQuality.AddOptions(new List<string>(QualitySettings.names));

        // ---- FPS ----
        dropdownFPS.ClearOptions();
        dropdownFPS.AddOptions(new List<string> { "30", "60", "120", "144", "Unlimited" });

        // ---- Toggles ----
        toggleFullscreen.isOn = SettingsManager.Instance.FullscreenMode != FullScreenMode.Windowed;
        toggleVSync.isOn = SettingsManager.Instance.VSync;
        toggleShowFPS.isOn = SettingsManager.Instance.ShowFPS;

        // ---- Listeners ----
        dropdownResolution.onValueChanged.AddListener(OnResolutionChanged);
        dropdownQuality.onValueChanged.AddListener(SettingsManager.Instance.SetQualityLevel);
        dropdownFPS.onValueChanged.AddListener(OnFPSChanged);
        toggleFullscreen.onValueChanged.AddListener(OnFullscreenChanged);
        toggleVSync.onValueChanged.AddListener(SettingsManager.Instance.SetVSync);
        toggleShowFPS.onValueChanged.AddListener(SettingsManager.Instance.SetShowFPS);
    }

    private void OnResolutionChanged(int index)
    {
        Resolution res = Screen.resolutions[index];
        FullScreenMode mode = toggleFullscreen.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        SettingsManager.Instance.SetResolution(res.width, res.height, mode);
    }

    private void OnFPSChanged(int index)
    {
        int fps = index switch
        {
            0 => 30,
            1 => 60,
            2 => 120,
            3 => 144,
            4 => -1, // Unlimited
            _ => 60
        };
        SettingsManager.Instance.SetTargetFPS(fps);
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        Resolution res = SettingsManager.Instance.CurrentResolution;
        FullScreenMode mode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        SettingsManager.Instance.SetResolution(res.width, res.height, mode);
    }

    private void InitializeControlsOptions()
    {
        sensitivityMouseXSlider.minValue = 1f;
        sensitivityMouseXSlider.maxValue = 20f;
        sensitivityMouseYSlider.minValue = 1f;
        sensitivityMouseYSlider.maxValue = 20f;
        sensitivityJoystickXSlider.minValue = 1f;
        sensitivityJoystickXSlider.maxValue = 20f;
        sensitivityJoystickYSlider.minValue = 1f;
        sensitivityJoystickYSlider.maxValue = 20f;

        sensitivityMouseXSlider.value = SettingsManager.Instance.SensitivityMouseX;
        sensitivityMouseYSlider.value = SettingsManager.Instance.SensitivityMouseY;
        sensitivityJoystickXSlider.value = SettingsManager.Instance.SensitivityJoystickX;
        sensitivityJoystickYSlider.value = SettingsManager.Instance.SensitivityJoystickY;

        sensitivityMouseXSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSensitivityMouseX);
        sensitivityMouseYSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSensitivityMouseY);
        sensitivityJoystickXSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSensitivityJoystickX);
        sensitivityJoystickYSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSensitivityJoystickY);

        UpdateTextControls(sensitivityMouseXText, sensitivityMouseXSlider.value);
        UpdateTextControls(sensitivityMouseYText, sensitivityMouseYSlider.value);
        UpdateTextControls(sensitivityJoystickXText, sensitivityJoystickXSlider.value);
        UpdateTextControls(sensitivityJoystickYText, sensitivityJoystickYSlider.value);

        sensitivityMouseXSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SensitivityMouseX = value;
            UpdateTextControls(sensitivityMouseXText, value);
        });
        sensitivityMouseYSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SensitivityMouseY = value;
            UpdateTextControls(sensitivityMouseYText, value);
        });
        sensitivityJoystickXSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SensitivityJoystickX = value;
            UpdateTextControls(sensitivityJoystickXText, value);
        });
        sensitivityJoystickYSlider.onValueChanged.AddListener(value =>
        {
            SettingsManager.Instance.SensitivityJoystickY = value;
            UpdateTextControls(sensitivityJoystickYText, value);
        });
    }

    private void UpdateTextControls(TMP_Text currentText, float value)
    {
        currentText.text = Mathf.RoundToInt(value).ToString();
    }

    private void DisableAllPanels()
    {
        panelAudio.SetActive(false);
        panelVideo.SetActive(false);
        panelControls.SetActive(false);
    }

    private void SetButtonNormalColorInWhite(Button currentButton)
    {
        ColorBlock color = currentButton.colors;
        color.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        currentButton.colors = color;
    }

    private void SetButtonNormalColorInGreen(Button currentButton)
    {
        ColorBlock color = currentButton.colors;
        color.normalColor = new Color32(0x28, 0xFF, 0x00, 0xFF);
        currentButton.colors = color;
    }

    /*private void CheckJoystickInputsToInteractWithPanels()
    {
        if (panelAudio.activeSelf || panelVideo.activeSelf || panelControls.activeSelf)
        {
            if (PlayerInputs.Instance != null)
            {
                if (PlayerInputs.Instance.R1())
                {
                    SetNexPanelUsingJoystickR1();
                }

                if (PlayerInputs.Instance.L1())
                {
                    SetNexPanelUsingJoystickL1();
                }
            }
        }
    }

    private void SetNexPanelUsingJoystickR1()
    {
        if (panelAudio.activeSelf)
        {
            SetPanelVideo();
            return;
        }

        if (panelVideo.activeSelf)
        {
            SetPanelControls();
            return;
        }

        if (panelControls.activeSelf)
        {
            SetPanelAudio();
            return;
        }
    }

    private void SetNexPanelUsingJoystickL1()
    {
        if (panelAudio.activeSelf)
        {
            SetPanelControls();
            return;
        }

        if (panelVideo.activeSelf)
        {
            SetPanelAudio();
            return;
        }

        if (panelControls.activeSelf)
        {
            SetPanelVideo();
            return;
        }
    }*/
}
