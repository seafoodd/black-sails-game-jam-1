using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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

    private void Awake()
    {
        pauseMenu = transform.Find("Pause Menu").gameObject;
        Main = pauseMenu.transform.Find("Main").gameObject;
        Settings = pauseMenu.transform.Find("Settings").gameObject;
        
        continueButton = Main.transform.Find("Continue Button").GetComponent<Button>();
        settingsButton = Main.transform.Find("Settings Button").GetComponent<Button>();
        mainMenuButton = Main.transform.Find("Main Menu Button").GetComponent<Button>();

        pauseMenu.SetActive(false);
        
        continueButton.onClick.AddListener(OnContinueButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        mainMenuButton.onClick.AddListener(OnMainMenuButtonClick);
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
    
    public void OnContinueButtonClick()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }
    public void OnSettingsButtonClick()
    {
        Main.SetActive(false);
        Settings.SetActive(true);
    }

    public void OnMainMenuButtonClick()
    {
        Debug.Log("load main menu scene");
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