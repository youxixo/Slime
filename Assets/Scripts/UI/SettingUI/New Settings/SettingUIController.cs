using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class SettingUIController : MonoBehaviour
{
    private bool settingChanged; // 用戶是否更改了設置
    private SettingPageParent currentPage;
    private SettingPageParent prevPage;
    private PlayerSettingsData newPlayerSettings;
    private PlayerSettingsData prevPlayerSettings;
    private static PlayerSettingsData defaultSettings = 
        new PlayerSettingsData
        {
            masterVolume = 60,
            musicVolume = 60,
            sfxVolume = 60,
            screenSizeMode = 0,
            resolutionMode = 0
        };
    private GameObject selectedPanelButton;
    [SerializeField] private GraphicSetting graphicSetting;
    [SerializeField] private SoundSetting soundSetting;
    [SerializeField] private KeybindPageAutoGenerate keybindPage;

    [SerializeField] private CanvasGroup buttonList;
    [SerializeField] private Transform panelList;
    private Dictionary<string, SettingPageParent> buttonPanelPair = new();

    public static UnityEvent finishSetting = new UnityEvent();

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction navigateAction;
    private InputAction exitAction;

    [Header("Panel")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingPage;
    [SerializeField] private CanvasGroup panelsGroup;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private GameObject selectedButtonOnPause;

    [Header("Setting Menu Buttons")]
    [SerializeField] private Button keybindButton;
    [SerializeField] private GameObject selectedButtonOnSetting;

    [Header("Confirm Setting Menu")]
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private GameObject selectedButtonOnConfirm;
    [SerializeField] private Transform confirmButtonsParent;

    [Header("Events")]
    public static UnityEvent resumeGameEvent = new UnityEvent();

    private void Start()
    {
        settingChanged = true;
        Debug.LogWarning("right now setting settingChanged to true when start for testing, make sure to change later");
        DontDestroyOnLoad(this);

        newPlayerSettings = defaultSettings;
        prevPlayerSettings = defaultSettings;
        Initialize();
        InitInput();

        EventSystem.current.SetSelectedGameObject(buttonList.transform.GetChild(0).gameObject);
    }

    private void Update()
    {
        if(exitAction.triggered)
        {
            Transform tempSelectedObj = EventSystem.current.currentSelectedGameObject.transform;
            if (tempSelectedObj.IsChildOf(buttonList.transform))
            {
                ExitingSetting();
                Debug.Log("Exit setting action");
            }
            else if(tempSelectedObj.IsChildOf(confirmButtonsParent))
            {
                BackToSetting();
            }
            else
            {
                currentPage.SetPlayerSettingData(ref newPlayerSettings);
                EventSystem.current.SetSelectedGameObject(selectedPanelButton);
                buttonList.interactable = true;
            }
        }
        if(navigateAction.triggered)
        {
            ChangeSetting();
        }
    }

    private void BackToSetting()
    {
        panelsGroup.interactable = true;
        panelsGroup.alpha = 1;
        confirmMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(buttonList.transform.GetChild(0).gameObject);
    }

    private void ExitingSetting()
    {
        if(settingChanged)
        {
            panelsGroup.interactable = false;
            panelsGroup.alpha = 0.5f;
            confirmMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(selectedButtonOnConfirm);
        }
    }

    public void MenuSelect()
    {
        selectedPanelButton = EventSystem.current.currentSelectedGameObject;
        prevPage = currentPage;
        currentPage = buttonPanelPair[selectedPanelButton.name];
        if(currentPage.firstSelectObj!=null) EventSystem.current.SetSelectedGameObject(currentPage.firstSelectObj);

        if(prevPage != null) prevPage.Deactivate();
        currentPage.Activate();
        buttonList.interactable = false;
        currentPage.LoadFromPSD(newPlayerSettings);
    }

    private void Initialize()
    {
        prevPlayerSettings = newPlayerSettings;

        for (int i = 0; i < buttonList.transform.childCount; i++)
        {
            buttonPanelPair[buttonList.transform.GetChild(i).name] = panelList.transform.GetChild(i).GetComponent<SettingPageParent>();
        }
    }

    //加載設置中的面板數值 
    private void UpdateSettingVisual(PlayerSettingsData psd)
    {
        graphicSetting.LoadFromPSD(psd);
        soundSetting.LoadFromPSD(psd);
    }

    //存Input Action
    private void InitInput()
    {
        var UIActionMap = inputActions.FindActionMap("UI");
        exitAction = UIActionMap.FindAction("Exit");
        navigateAction = UIActionMap.FindAction("Navigate");
    }   

    #region 暫停
    private void OnPause()
    {
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(selectedButtonOnPause);
    }

    private void Resume()
    {
        pauseMenu.SetActive(false);
        resumeGameEvent.Invoke();
    }
    #endregion

    //左右修改設置數值
    private void ChangeSetting()
    {
        GameObject selectedSetting = EventSystem.current.currentSelectedGameObject;

        if (navigateAction.ReadValue<Vector2>().x == 1)
        {
            if (selectedSetting.TryGetComponent<Slider>(out Slider slideComponent))
            {
                slideComponent.value += 10;
            }
            else if (selectedSetting.TryGetComponent<TMP_Dropdown>(out TMP_Dropdown dropdownComponent))
            {
                dropdownComponent.value = (dropdownComponent.value + 1) % dropdownComponent.options.Count;
            }
        }
        else if (navigateAction.ReadValue<Vector2>().x == -1)
        {
            if (selectedSetting.TryGetComponent<Slider>(out Slider slideComponent))
            {
                slideComponent.value -= 10;
            }
            else if (selectedSetting.TryGetComponent<TMP_Dropdown>(out TMP_Dropdown dropdownComponent))
            {
                dropdownComponent.value = (dropdownComponent.value - 1 + dropdownComponent.options.Count) % dropdownComponent.options.Count;
            }
        }
    }

    //保存新的玩家數據
    public void SaveSetting()
    {
        prevPlayerSettings = newPlayerSettings;

        soundSetting.ApplyChanges(newPlayerSettings);
        graphicSetting.ApplyChanges(newPlayerSettings);
        keybindPage.ApplyChanges(newPlayerSettings);
    }

    public void NoSaveSetting()
    {
        newPlayerSettings = prevPlayerSettings;

        soundSetting.DoNotSaveChanges();
        graphicSetting.DoNotSaveChanges();
        keybindPage.DoNotSaveChanges();
    }
}

public struct PlayerSettingsData
{
    public float masterVolume { get; set; }
    public float musicVolume { get; set; }
    public float sfxVolume { get; set; }

    public int screenSizeMode { get; set; }
    public int resolutionMode { get; set; }

    public string keybindSet { get; set; }
}