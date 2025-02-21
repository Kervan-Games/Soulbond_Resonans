using System.Collections;
using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public float afterImageLifetime = 0.5f; 
    public float spawnInterval = 0.05f; 
    public Color afterImageColor = new Color(1f, 1f, 1f, 0.5f); 

    private Transform player;
    private SpriteRenderer playerSprite;
    private bool isDashing = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopDash();
        }
    }*/

    public void Dash()
    {
        if (!isDashing)
        {
            isDashing = true;
            StartCoroutine(CreateAfterImages());
        }
    }

    private IEnumerator CreateAfterImages()
    {
        while (isDashing)
        {
            CreateGhost();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void CreateGhost()
    {
        GameObject ghost = new GameObject("AfterImage");
        SpriteRenderer ghostSprite = ghost.AddComponent<SpriteRenderer>();

        ghost.transform.position = player.position;
        ghost.transform.localScale = player.localScale;
        ghostSprite.sprite = playerSprite.sprite;  
        ghostSprite.flipX = playerSprite.flipX;  
        ghostSprite.color = afterImageColor; 

        ghostSprite.sortingLayerID = playerSprite.sortingLayerID;
        ghostSprite.sortingOrder = playerSprite.sortingOrder - 1; 

        StartCoroutine(FadeOutAndDestroy(ghostSprite, afterImageLifetime));
    }

    private IEnumerator FadeOutAndDestroy(SpriteRenderer ghostSprite, float lifetime)
    {
        float elapsedTime = 0f;
        Color startColor = ghostSprite.color;

        while (elapsedTime < lifetime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / lifetime);
            ghostSprite.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(ghostSprite.gameObject);
    }

    public void StopDash()
    {
        isDashing = false;
    }
}
