using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuButtons : MonoBehaviour
{
    public static UnityEvent settingClick = new UnityEvent();

    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (root != null)
        {
            root.Q<Button>("Start").clicked += () => StartGame();
            root.Q<Button>("Setting").clicked += () => Setting();
            root.Q<Button>("Quit").clicked += () => QuitGame();

            //root.Query<Button>().ForEach(button => button.RegisterCallback<FocusInEvent>(evt => OnSelected(button)));
            //root.Query<Button>().ForEach(button => button.RegisterCallback<FocusOutEvent>(evt => OnDeSelected(button)));
        }
        root.Q<Button>("Start").Focus();

        UIController.finishSetting.AddListener(BackFromSetting);
    }

    private void OnSelected(Button b)
    {
        b.style.backgroundColor = Color.gray;
    }

    private void OnDeSelected(Button b)
    {
        b.style.backgroundColor = new StyleColor(new Color32(188, 188, 188, 255));
        //b.style.backgroundColor = Color.white;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    //可以換成New Game / Load Game
    private void StartGame()
    {
        SceneManager.LoadScene("完整关卡");
        GameManager.ActivateActionMap("Player");
    }

    private void Setting()
    {
        settingClick.Invoke();
    }

    private void BackFromSetting()
    {
        GetComponent<UIDocument>().rootVisualElement.Q<Button>("Setting").Focus();
    }
}
