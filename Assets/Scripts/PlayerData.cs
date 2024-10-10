using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : NetworkBehaviour
{
    [SerializeField] private int MAXHP = 100;
    [SerializeField] public NetworkVariable<int> playerHp = new NetworkVariable<int>(writePerm: NetworkVariableWritePermission.Server);
    [SerializeField] private Image hpDisplay;
    //[SerializeField] private NetworkVariable<string> playerName = new NetworkVariable<string>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            playerHp.Value = MAXHP;
        }
        playerHp.OnValueChanged += OnHpChange;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeHPServerRpc(int damage)
    {
        if (IsServer)
        {
            playerHp.Value -= damage;
        }
    }

    private void OnHpChange(int previous, int current)
    {
        hpDisplay.fillAmount = (float)playerHp.Value / MAXHP;
    }
}
