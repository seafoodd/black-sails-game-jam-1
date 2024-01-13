using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIINput : MonoBehaviour
{
    [FormerlySerializedAs("UI"),SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject Main;
    [SerializeField] private GameObject Settings;
    
    [SerializeField] private Button continueButton, settingsButton, mainMenuButton;
    
    [SerializeField] private Scene mainMenuScene;
    [SerializeField] private AudioMixer audMix;
    [SerializeField] private Slider mainVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    //private float volume;

    public static UIINput instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if(PlayerPrefs.HasKey("mainVolume") && PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("effectsVolume")) LoadVolume();

        pauseMenu = transform.Find("Pause Menu").gameObject;
        Main = pauseMenu.transform.Find("Main").gameObject;
        Settings = pauseMenu.transform.Find("Settings").gameObject;
        //volumeSlider = Settings.transform.Find("Volume Slider").GetComponent<Slider>();
        
        continueButton = Main.transform.Find("Continue Button").GetComponent<Button>();
        settingsButton = Main.transform.Find("Settings Button").GetComponent<Button>();
        mainMenuButton = Main.transform.Find("Main Menu Button").GetComponent<Button>();

        pauseMenu.SetActive(false);
        
        continueButton.onClick.AddListener(OnContinueButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);

        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeSliderValueChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderValueChanged);
        effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeSliderValueChanged);
    }

    private void OnMainVolumeSliderValueChanged(float volume)
    {
        audMix.SetFloat("Main", Mathf.Log10(volume) * 20 + 10);
        PlayerPrefs.SetFloat("mainVolume", volume);
        //Debug.Log($"current main volume: {volume}");
    }
    private void OnMusicVolumeSliderValueChanged(float volume)
    {
        audMix.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
        //Debug.Log($"current music volume: {volume}");
    }
    private void OnEffectsVolumeSliderValueChanged(float volume)
    {
        audMix.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("effectsVolume", volume);
       // Debug.Log($"current effects volume: {volume}");
    }

    private void LoadVolume()
    {
        mainVolumeSlider.value = PlayerPrefs.GetFloat("mainVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        effectsVolumeSlider.value = PlayerPrefs.GetFloat("effectsVolume");

        audMix.SetFloat("Main", Mathf.Log10(mainVolumeSlider.value) * 20 + 10);
        audMix.SetFloat("Music", Mathf.Log10(musicVolumeSlider.value) * 20);
        audMix.SetFloat("SFX", Mathf.Log10(effectsVolumeSlider.value) * 20);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (ToggleUI())
            {
                Time.timeScale = 0f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
    }
    


    private void OnContinueButtonClick()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }
    private void OnSettingsButtonClick()
    {
        Main.SetActive(false);
        Settings.SetActive(true);
    }

    private void OnMainMenuButtonClick()
    {
        Application.Quit();
    }

    private bool ToggleUI()
    {
        // enable pauseMenu
        if (!pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(true);
            Main.SetActive(true);
            return true;
        }
        
        // disable settings
        if(Settings.activeSelf)
        {
            Settings.SetActive(false);
            Main.SetActive(true);
            return true;
        }
        
        // disable pauseMenu
        pauseMenu.SetActive(false);
        return false;
    }
}