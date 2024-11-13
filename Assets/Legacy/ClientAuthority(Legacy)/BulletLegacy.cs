using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class BulletLegacy : NetworkBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private int speed = 5;
    public float direction = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        if (direction != 0f)
        {
            rb.MovePosition(new Vector2(rb.position.x + speed * direction * Time.deltaTime, rb.position.y));
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            DestroyBullet();
        }
        if(collision.gameObject.layer == 6)
        {
            if(collision.TryGetComponent(out PlayerData player))
            {
                player.ChangeHPServerRpc(10);
            }
        }
        if(collision.gameObject.layer == 7)
        {
            NetworkObject.Despawn();
        }
    }

    private void DestroyBullet()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("KillBullet");
    }

    IEnumerator KillBullet()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkObject.Despawn();
    }
}
