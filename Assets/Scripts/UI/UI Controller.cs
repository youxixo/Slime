using System;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// UI控制腳本 - 控制大部分的面板開關
/// 有些可能可以拿出來分開寫
/// 
/// 把操控聲音的直接掛到slider組件上 on Value change讓玩家可以邊調邊聽
/// </summary>
public class UIController : MonoBehaviour
{
    private bool settingChanged; // 用戶是否更改了設置
    private Menus currentOpenMenu;
    private PlayerSettings playerSettings;
    private PlayerSettings defaultSettings;

    public static UnityEvent finishSetting = new UnityEvent();

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction navigateAction;
    private InputAction exitAction;

    [Header("Panel")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingPage;
    [SerializeField] private GameObject keybindPage;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private GameObject selectedButtonOnPause;

    [Header("Setting Menu Buttons")]
    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Button keybindButton;
    [SerializeField] private GameObject selectedButtonOnSetting;

    [Header("Confirm Setting Menu")]
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private Button confirmSetting;
    [SerializeField] private Button cancelSetting;
    [SerializeField] private GameObject selectedButtonOnConfirm;

    [Header("Events")]
    public static UnityEvent resumeGameEvent = new UnityEvent();

    private void Start()
    {
        DontDestroyOnLoad(this);

        defaultSettings = new PlayerSettings
        {
            masterVolume = 60,
            musicVolume = 60,
            sfxVolume = 60,
            screenSizeMode = 0,
            resolutionMode = 0
        };
        playerSettings = defaultSettings;

        InitInput();
        PlayerMove.pauseGame.AddListener(OnPause);
        continueButton.onClick.AddListener(Resume);
        settingButton.onClick.AddListener(OpenSetting);
        exitGameButton.onClick.AddListener(() => Application.Quit());

        keybindButton.onClick.AddListener(OpenKeybind);
        confirmSetting.onClick.AddListener(ApplyChange);
        cancelSetting.onClick.AddListener(DoNotApplyChange);

        MainMenuButtons.settingClick.AddListener(OpenSetting);
    }

    void Update()
    {
        if (navigateAction.triggered && currentOpenMenu == Menus.SettingMenu)
        {
            ChangeSetting();
        }
        if(exitAction.triggered)
        {
            if(currentOpenMenu == Menus.SettingMenu)
            {
                ExitSetting();
            }
            else if(currentOpenMenu == Menus.KeybindMenu)
            {
                ExitKeybind();
            }
        }
    }

    //不同的UI介面
    private enum Menus
    {
        None,
        PauseMenu,
        SettingMenu,
        ConfirmSettingMenu,
        KeybindMenu
    }

    //加載設置中的面板數值 
    private void UpdateSettingVisual()
    {
        masterAudioSlider.value = playerSettings.masterVolume;
        musicSlider.value = playerSettings.musicVolume;
        sfxSlider.value = playerSettings.sfxVolume;

        resolutionDropdown.value = playerSettings.resolutionMode;
        screenModeDropdown.value = screenModeDropdown.value;
    }

    //存Input Action
    private void InitInput()
    {
        var UIActionMap = inputActions.FindActionMap("UI");

        exitAction = UIActionMap.FindAction("Exit");
        navigateAction = UIActionMap.FindAction("Navigate");
    }

    #region 介面開關
    //打開設置介面
    private void OpenSetting()
    {
        DisableSettingPanel();
        settingPage.SetActive(true);
        LoadSettings();
        UpdateSettingVisual();
        currentOpenMenu = Menus.SettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnSetting);
    }

    //關閉設置介面
    private void ExitSetting()
    {
        confirmMenu.SetActive(true);
        currentOpenMenu = Menus.ConfirmSettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnConfirm);
    }

    //關閉鍵位設置介面
    private void ExitKeybind()
    {
        keybindPage.SetActive(false);
        settingPage.SetActive(true);
        currentOpenMenu = Menus.SettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnSetting);
    }

    //打開鍵位設置
    private void OpenKeybind()
    {
        DisableSettingPanel();
        currentOpenMenu = Menus.KeybindMenu;
        keybindPage.SetActive(true);
    }

    //關閉所有設置介面
    private void DisableSettingPanel()
    {
        //pauseMenu.SetActive(false);
        settingPage.SetActive(false);
        confirmMenu.SetActive(false);
        keybindPage.SetActive(false);
    }
    #endregion

    #region 暫停
    private void OnPause()
    {
        currentOpenMenu = Menus.PauseMenu;
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
    }

    private void Resume()
    {
        currentOpenMenu = Menus.None;
        pauseMenu.SetActive(false);
        resumeGameEvent.Invoke();
    }
    #endregion

    #region 設置介面的東西

    //左右修改設置數值
    private void ChangeSetting()
    {
        GameObject selectedSettingChild;
        if (EventSystem.current.currentSelectedGameObject.transform.childCount > 0)
        {
            selectedSettingChild = EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject;
        }
        else
        {
            return;
        }

        if (navigateAction.ReadValue<Vector2>().x == 1)
        {
            if (selectedSettingChild.TryGetComponent<Slider>(out Slider slideComponent))
            {
                slideComponent.value += 10;
            }
            else if (selectedSettingChild.TryGetComponent<TMP_Dropdown>(out TMP_Dropdown dropdownComponent))
            {
                dropdownComponent.value = (dropdownComponent.value + 1) % dropdownComponent.options.Count;
            }
        }
        else if (navigateAction.ReadValue<Vector2>().x == -1)
        {
            if (selectedSettingChild.TryGetComponent<Slider>(out Slider slideComponent))
            {
                slideComponent.value -= 10;
            }
            else if (selectedSettingChild.TryGetComponent<TMP_Dropdown>(out TMP_Dropdown dropdownComponent))
            {
                dropdownComponent.value = (dropdownComponent.value - 1 + dropdownComponent.options.Count) % dropdownComponent.options.Count;
            }
        }
    }

    //應用並保存數據
    private void ApplyChange()
    {
        string[] resolutionList = resolutionDropdown.options[resolutionDropdown.value].text.Split("x");
        Screen.SetResolution(int.Parse(resolutionList[0].Trim()), int.Parse(resolutionList[1].Trim()), GetScreenMode());

        SaveSetting();
        DisableSettingPanel();
        //DisableSettingMenu();
        //pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
        finishSetting.Invoke();
    }

    //玩家畫面模式設置
    private FullScreenMode GetScreenMode()
    {
        string screenModeText = screenModeDropdown.options[screenModeDropdown.value].text;
        if (screenModeText == "Fullscreen")
        {
            return FullScreenMode.FullScreenWindow;
        }
        else if(screenModeText == "Windowed")
        {
            return FullScreenMode.Windowed;
        }
        else if(screenModeText == "Borderless Windowed")
        {
            return FullScreenMode.MaximizedWindow;
        }
        Debug.LogWarning("Screen Mode有問題, 默認全屏");
        return FullScreenMode.FullScreenWindow;
    }

    //退出設置 不應用數據
    private void DoNotApplyChange()
    {
        DisableSettingPanel();
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
        finishSetting.Invoke();
    }
    #endregion

    //加載玩家數據
    private void LoadSettings()
    {
        masterAudioSlider.value = playerSettings.masterVolume;
        musicSlider.value = playerSettings.musicVolume;
        sfxSlider.value = playerSettings.sfxVolume;
        screenModeDropdown.value = playerSettings.screenSizeMode;
        resolutionDropdown.value = playerSettings.resolutionMode;
    }

    //保存新的玩家數據
    private void SaveSetting()
    {
        playerSettings = new PlayerSettings()
        {
            masterVolume = masterAudioSlider.value,
            musicVolume = musicSlider.value,
            sfxVolume = sfxSlider.value,

            screenSizeMode = screenModeDropdown.value,
            resolutionMode = resolutionDropdown.value
        };
    }
}

public struct PlayerSettings
{
    public float masterVolume { get; set; }
    public float musicVolume { get; set; }
    public float sfxVolume { get; set; }

    public int screenSizeMode { get; set; }
    public int resolutionMode { get; set; }
}
