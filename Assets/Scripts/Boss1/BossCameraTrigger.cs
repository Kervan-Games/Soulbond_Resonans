using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public GameObject boss; 
    public GameObject player;
    public GameObject umbrellaThrow;
    public GameObject umbrella;
    public CinemachineVirtualCamera virtualCamera; 
    public float zoomDuration = 2f; //add new zoom duration for offset change if needs
    public float targetOrthoSize = 12f; 
    public Vector3 bossOffset = new Vector3(16, 1, 0); 
    public Vector3 playerOffset = new Vector3(0, 1, 0); 
    private float originalOrthoSize; 
    private Vector3 originalOffset;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRB;
    private bool isInBoss = false;
    private bool canEnter = true;

    public BossMovement bossMovement;
    public Umbrella umbrellaScript;

    private void Update()
    {
        //Debug.Log(bossOffset);
    }
    private void Start()
    {
        originalOrthoSize = virtualCamera.m_Lens.OrthographicSize;
        originalOffset = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;
        playerMovement = player.GetComponent<PlayerMovement>();
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canEnter)
        {
            virtualCamera.Follow = boss.transform;
            playerMovement.SetInLanes(true);
            playerRB.gravityScale = 0f;
            umbrellaScript.SetIsFlying(true);

            StartCoroutine(SmoothZoom(targetOrthoSize));
            StartCoroutine(SmoothOffset(bossOffset));
            umbrella.SetActive(true);
            playerMovement.SetCanUmbrellaShot(false);
            if (umbrellaThrow.activeSelf)
            {
                umbrellaThrow.SetActive(false);
            }
            isInBoss = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canEnter)
        {
            isInBoss = false;
            umbrellaScript.SetIsFlying(false);
            virtualCamera.Follow = player.transform;
            playerMovement.SetInLanes(false);
            playerRB.gravityScale = 2f;
            playerRB.velocity = Vector3.zero;
            StartCoroutine(SmoothZoom(originalOrthoSize));
            StartCoroutine(SmoothOffset(playerOffset));
            umbrella.SetActive(false);
            playerMovement.SetCanUmbrellaShot(true);
            //Debug.Log("OFF");
            //boss.SetActive(false);
            bossMovement.SetBossSpeed(0);
            canEnter = false;
        }
    }

    IEnumerator SmoothZoom(float targetSize)
    {
        float elapsedTime = 0f;
        float startingSize = virtualCamera.m_Lens.OrthographicSize;

        while (elapsedTime < zoomDuration)
        {
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startingSize, targetSize, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        virtualCamera.m_Lens.OrthographicSize = targetSize;
    }

    IEnumerator SmoothOffset(Vector3 targetOffset)
    {
        float elapsedTime = 0f;
        Vector3 startingOffset = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset;

        while (elapsedTime < zoomDuration)
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = Vector3.Lerp(startingOffset, targetOffset, elapsedTime / zoomDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        while (isInBoss)
        {
            virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_TrackedObjectOffset = targetOffset;
            yield return null;
        }
        
    }
}
