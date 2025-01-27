using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MainUIController : MonoBehaviour
{
    [SerializeField] private PauseMenuUI pauseMenu;
    [SerializeField] private SettingUIController settingMenu;
    public static MainUIController _instance;

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

    private void OnPause()
    {
        pauseMenu.gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(pauseMenu.selectedButtonOnPause);
    }

    public void OpenSetting()
    {
        settingMenu.gameObject.SetActive(true);
    }
}
