using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YBubbleManager : MonoBehaviour
{
    public GameObject yellowBubblePrefab;
    public Vector3 spawnPosition;
    public float respawnInterval = 60f;
    private GameObject currentBubble;

    void Start()
    {
        Debug.Log("[YBubbleManager] Start called, will spawn yellow bubble.");
        SpawnYBubble();
    }

    void SpawnYBubble()
    {
        if (yellowBubblePrefab == null)
        {
            return;
        }
        Debug.Log("[YBubbleManager] Spawning yellow bubble at " + spawnPosition);
        currentBubble = Instantiate(yellowBubblePrefab, spawnPosition, Quaternion.identity);
        var trigger = currentBubble.AddComponent<YBubbleTrigger>();
        trigger.manager = this;
    }

    public void OnBubbleCollected()
    {
        Debug.Log("[YBubbleManager] Yellow bubble collected!");
        if (currentBubble != null)
        {
            Destroy(currentBubble);
            currentBubble = null;
        }
        StartCoroutine(RespawnCoroutine());
    }
    public void ResetYBubble()
    {
        if (currentBubble != null)
        {
            Destroy(currentBubble);
            currentBubble = null;
        }
        StopAllCoroutines(); 
        SpawnYBubble();
    }

    IEnumerator RespawnCoroutine()
    {
        Debug.Log("[YBubbleManager] Respawn coroutine started, waiting " + respawnInterval + " seconds.");
        yield return new WaitForSeconds(respawnInterval);
        SpawnYBubble();
    }
}