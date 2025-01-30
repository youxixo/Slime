using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private PauseMenuUI pauseMenu;
    [SerializeField] private SettingUIController settingMenu;
    public static MainUIController _instance;
    private Stack<PanelParent> panelsStack = new();

    public static MainUIController Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogWarning("already has main ui instance");
            return;
        }
        if (transform.parent)
        {
            DontDestroyOnLoad(transform.parent);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        MainMenuButtons.settingClick.AddListener(OpenSetting);
        PlayerMove.pauseGame.AddListener(OnPause);
    }

    public void CloseCurrentPanel()
    {
        PanelParent temp;
        panelsStack.TryPeek(out temp);
        if (temp != null)
        {
            panelsStack.Pop().DisableThisPanel();

            if (panelsStack.Count > 0) panelsStack.Peek().gameObject.SetActive(true);
        }
    }

    public void OpenAPanel(PanelParent openingPanel)
    {
        PanelParent temp;
        panelsStack.TryPeek(out temp);
        if (temp != null) temp.DisableThisPanel();

        panelsStack.Push(openingPanel);
        openingPanel.gameObject.SetActive(true);
        if(openingPanel)
            EventSystem.current.SetSelectedGameObject(openingPanel.selectedButtonWhenOpen);
    }

    private void OnPause()
    {
        OpenAPanel(pauseMenu);
        /*
        PanelParent temp;
        panelsStack.TryPeek(out temp);
        if(temp != null) temp.DisableThisPanel();

        panelsStack.Push(pauseMenu);
        pauseMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenu.selectedButtonOnPause);
        */
    }

    public void OpenSetting()
    {
        OpenAPanel(settingMenu);
        /*
        PanelParent temp;
        panelsStack.TryPeek(out temp);
        if (temp != null) temp.DisableThisPanel();

        panelsStack.Push(settingMenu);
        settingMenu.gameObject.SetActive(true);
        */
    }
}
