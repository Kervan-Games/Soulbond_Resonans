using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float bossSpeed = 7.5f;
    public GameObject player;
    public float playerSpeed = 7.5f;
    public float distanceThreshold = 10f;
    public float raycastDistance = 10f;

    private PlayerMovement playerMovement;

    public float followSpeed = 5f; // Boss'un player'ý takip etme hýzý
    public float verticalOffset = 0f; // Boss'un player'a göre dikey pozisyon ofseti

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerMovement = player.GetComponent<PlayerMovement>();
        gameObject.SetActive(false);
    }

    void Update()
    {
        FollowPlayerVertically();

        //Debug.Log(playerSpeed);
        rb.velocity = new Vector2(bossSpeed, rb.velocity.y);

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if(rb.velocity.x > 0.5f)
        {
            if (distanceToPlayer < distanceThreshold)
            {
                playerSpeed = Mathf.Max(7.5f, bossSpeed + (distanceThreshold - distanceToPlayer));
                playerMovement.SetMoveSpeed(playerSpeed);
            }
            else
            {
                playerSpeed = bossSpeed;
                playerMovement.SetMoveSpeed(playerSpeed);
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, raycastDistance);
        Debug.DrawRay(transform.position, Vector2.right * raycastDistance, Color.red);
    }

    private void FollowPlayerVertically()
    {
        // Player'ýn dikey pozisyonunu al
        float targetY = player.transform.position.y + verticalOffset;
        float currentY = rb.position.y;

        // Dikey yönde hedefe doðru hýz ayarla
        float newYVelocity = (targetY - currentY) * followSpeed;

        // Rigidbody'nin dikey hýzýný güncelle
        rb.velocity = new Vector2(rb.velocity.x, newYVelocity);
    }

    public void SetBossSpeed(float spd)
    {
        bossSpeed = spd;
    }
}
