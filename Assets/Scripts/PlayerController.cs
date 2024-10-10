using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 1f;
    [SerializeField] float jumpForce = 5f;
    private Rigidbody2D rb;
    [SerializeField] private GameObject bulletPrefab;
    private SpriteRenderer sr;
    float horizontalMovement;
    private NetworkVariable<Vector3> playerScale = new NetworkVariable<Vector3>(writePerm: NetworkVariableWritePermission.Owner);

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();    
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if(!IsOwner)
        {
            Debug.Log("not owner");
            //enabled = false;
            return;
        }
        if (IsOwner)
        {
            playerScale.Value = new Vector3(1, 1, 1);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            HandleAttack();
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            horizontalMovement = Input.GetAxis("Horizontal");
            Vector2 velocity = new Vector2(horizontalMovement, 0);
            if (Mathf.Abs(velocity.x) > 0 || Input.GetAxis("Jump") != 0)
            {
                if(Input.GetAxis("Jump") != 0)
                {
                    velocity.y = jumpForce;
                }
                SendMoveRequest(velocity);
            }
        }
    }

    #region Move
    private void SendMoveRequest(Vector2 velocity)
    {
        MoveServerRpc(velocity);
    }

    [Rpc(SendTo.Server)]
    private void MoveServerRpc(Vector2 velocity)
    {
        MoveClientRpc(velocity);
    }


    [Rpc(SendTo.ClientsAndHost)]
    private void MoveClientRpc(Vector2 velocity)
    {
        RespondToMove(velocity);
    }

    private void RespondToMove(Vector2 velocity)
    {
        if (horizontalMovement > 0)
        {
            playerScale.Value = new Vector3(1, transform.localScale.y, transform.localScale.z);
            //transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalMovement < 0)
        {
            playerScale.Value = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            //transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        transform.localScale = playerScale.Value;
        if (velocity.y == 0)
        {
            rb.linearVelocityX = velocity.x * moveSpeed;
            return;
        }
        rb.linearVelocity = new Vector2(velocity.x * moveSpeed, velocity.y);
    }
    #endregion

    #region attack
    private void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button for attack
        {
            //SendBasicAttackRequest();
            BasicAttackServerRpc((int)transform.localScale.x);
        }
    }

    private void SendBasicAttackRequest(int direction)
    {
        BasicAttackServerRpc(direction); // Send attack request to the server
    }

    //[ServerRpc]
    [Rpc(SendTo.Server)]
    private void BasicAttackServerRpc(int direction)
    {
        SpawnBullet(direction);
    }

    private void SpawnBullet(int direction)
    {
        GameObject bulletInstance = Instantiate(bulletPrefab, rb.transform.position, Quaternion.identity);
        bulletInstance.GetComponent<Bullet>().direction = direction;
        bulletInstance.GetComponent<NetworkObject>().Spawn(true); // Spawn with ownership of server
    }
    #endregion
}