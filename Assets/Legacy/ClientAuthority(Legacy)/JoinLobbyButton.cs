using UnityEngine;

public class JoinLobbyButton : MonoBehaviour
{

    public bool needPassword;
    public string lobbyId;
    
    public void JoinLobbyButtonPressed()
    {
        LobbyManager.Instance.JoinLobby(lobbyId, needPassword);
    }

}
