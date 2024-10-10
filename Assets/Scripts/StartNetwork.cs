using Unity.Netcode;
using UnityEngine;

public class StartNetwork : MonoBehaviour
{
    public GameObject playerPrefab;

    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();


        //var instance = Instantiate(playerPrefab);
        //var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        //instanceNetworkObject.Spawn();
    }
}
