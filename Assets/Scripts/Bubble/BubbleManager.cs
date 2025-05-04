using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [Header("泡泡设置")]
    public GameObject bubblePrefab;          // 泡泡预制体
    public float spawnInterval = 2f;         // 生成间隔
    public float launchForce = 10f;         // 发射力度
    public Vector2 launchDirection = Vector2.right; // 发射方向
    
    [Header("生成位置")]
    public Vector3 spawnPosition;           // 生成位置
    
    private float timer;                    // 计时器
    private bool canSpawn = true;          // 是否可以生成

    void Start()
    {
        timer = 0f;
        if (bubblePrefab == null)
        {
            Debug.LogError("请设置泡泡预制体!");
        }
    }

    void Update()
    {
        if (!canSpawn || bubblePrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnBubble();
            timer = 0f;
        }
    }

    private void SpawnBubble()
    {
        // 在指定位置生成泡泡
        GameObject bubble = Instantiate(bubblePrefab, transform.position + spawnPosition, Quaternion.identity);
        
        // 获取泡泡的Rigidbody2D组件
        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 设置初始速度
            rb.velocity = launchDirection.normalized * launchForce;
        }

        

        // 销毁超时的泡泡（可选）
        Destroy(bubble, 5f);
    }

    // 提供启动/停止生成的公共方法
    public void StartSpawning()
    {
        canSpawn = true;
    }

    public void StopSpawning()
    {
        canSpawn = false;
    }
}
