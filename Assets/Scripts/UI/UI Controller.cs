using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.LightProbeProxyVolume;

public class UIController : MonoBehaviour
{
    private bool settingChanged; // 用戶是否更改了設置
    private Menus currentOpenMenu;
    private PlayerSettings playerSettings;
    private PlayerSettings defaultSettings;

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

        keybindButton.onClick.AddListener(KeybindPress);
        confirmSetting.onClick.AddListener(ApplyChange);
        cancelSetting.onClick.AddListener(DoNotApplyChange);
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

    private void InitInput()
    {
        var UIActionMap = inputActions.FindActionMap("UI");

        exitAction = UIActionMap.FindAction("Exit");
        navigateAction = UIActionMap.FindAction("Navigate");
    }

    //關閉所有介面
    private void DisableAllPanel()
    {
        pauseMenu.SetActive(false);
        settingPage.SetActive(false);
        confirmMenu.SetActive(false);
        keybindPage.SetActive(false);
    }

    //關閉所有設置介面
    private void DisableSettingMenu()
    {
        settingPage.SetActive(false);
        confirmMenu.SetActive(false);
        keybindPage.SetActive(false);
    }

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

    #region 設置介面Navigate
    //打開設置介面
    private void OpenSetting()
    {
        DisableAllPanel();
        settingPage.SetActive(true);
        currentOpenMenu = Menus.SettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnSetting);
        LoadSettings();
    }

    //左右修改設置
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

    private void ExitSetting()
    {
        confirmMenu.SetActive(true);
        currentOpenMenu = Menus.ConfirmSettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnConfirm);
    }

    private void ExitKeybind()
    {
        keybindPage.SetActive(false);
        settingPage.SetActive(true);
        currentOpenMenu = Menus.SettingMenu;
        EventSystem.current.SetSelectedGameObject(selectedButtonOnSetting);
    }

    private void KeybindPress()
    {
        DisableAllPanel();
        currentOpenMenu = Menus.KeybindMenu;
        keybindPage.SetActive(true);
    }

    private void ApplyChange()
    {
        SaveSetting();
        DisableSettingMenu();
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
    }

    private void DoNotApplyChange()
    {
        DisableSettingMenu();
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
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
