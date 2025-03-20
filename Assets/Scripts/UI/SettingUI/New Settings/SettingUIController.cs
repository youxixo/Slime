using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class SettingUIController : PanelParent
{
    private bool settingChanged; // 用戶是否更改了設置
    private SettingPageParent currentPage;
    private SettingPageParent prevPage;
    private PlayerSettingsData newPlayerSettings;
    private PlayerSettingsData prevPlayerSettings;
    private static PlayerSettingsData defaultSettings = 
        new PlayerSettingsData
        {
            masterVolume = 0,
            musicVolume = 0,
            sfxVolume = 0,
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


    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;
    private InputAction navigateAction;
    private InputAction exitAction;

    [Header("Panel")]
    [SerializeField] private CanvasGroup panelsGroup;

    [Header("Confirm Setting Menu")]
    [SerializeField] private GameObject confirmMenu;
    [SerializeField] private GameObject selectedButtonOnConfirm;
    [SerializeField] private Transform confirmButtonsParent;

    [Header("Events")]
    public static UnityEvent resumeGameEvent = new UnityEvent();
    public static UnityEvent finishSetting = new UnityEvent();

    private void Start()
    {
        newPlayerSettings = defaultSettings;
        prevPlayerSettings = defaultSettings;

        Initialize();
        InitInput();
    }

    public void OnEnable()
    {
        settingChanged = true;
        Debug.LogWarning("right now setting settingChanged to true when start for testing, make sure to change later");
        buttonList.interactable = true;
        panelsGroup.alpha = 1;
        panelsGroup.interactable = true;
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
            Debug.Log("mmsd");
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
        print("click select");
        selectedPanelButton = EventSystem.current.currentSelectedGameObject;
        prevPage = currentPage;
        currentPage = buttonPanelPair[selectedPanelButton.name];

        if(prevPage != null) prevPage.Deactivate();
        currentPage.Activate();
        if (currentPage.firstSelectObj != null) EventSystem.current.SetSelectedGameObject(currentPage.firstSelectObj);
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

    //左右修改設置數值
    private void ChangeSetting()
    {
        GameObject selectedSetting = EventSystem.current.currentSelectedGameObject;

        if (navigateAction.ReadValue<Vector2>().x == 1)
        {
            if (selectedSetting.TryGetComponent<Slider>(out Slider slideComponent))
            {
                slideComponent.value += 10;
                slideComponent.targetGraphic.transform.localScale = new Vector3(-1, 1, 1);
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
                slideComponent.targetGraphic.transform.localScale = new Vector3(1, 1, 1);
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

        CloseAllPanel();
    }

    public void NoSaveSetting()
    {
        newPlayerSettings = prevPlayerSettings;

        soundSetting.DoNotSaveChanges();
        graphicSetting.DoNotSaveChanges();
        keybindPage.DoNotSaveChanges();

        CloseAllPanel();
    }

    //***Todo - Add a Close to the interface and let each page deal their own dispose
    private void CloseAllPanel()
    {
        soundSetting.gameObject.SetActive(false);
        graphicSetting.gameObject.SetActive(false);
        keybindPage.gameObject.SetActive(false);
        confirmMenu.SetActive(false);
        MainUIController.Instance.CloseCurrentPanel();
        finishSetting.Invoke();
        this.gameObject.SetActive(false);
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