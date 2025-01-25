using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject spawnPlayer;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin virtualCameraNoise;
    public static GameManager Instance { get; private set; }
    private GameObject player; 
    public enum CameraTrackingMode { FollowPlayer, FollowPlayerShake } 
    public CameraTrackingMode cameraTrackingMode = CameraTrackingMode.FollowPlayer;
    private float cameraShakeIntensity = 0.1f;
    private float cameraShakeTime = 0f;
    private float cameraShakeTimer = -1f;

    #region Public Methods

    // 获取 Player 对象
    public GameObject GetPlayer()
    {
        return player;
    }

    // 销毁传入的 GameObject 对象
    public void DestroyGameObject(GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    public void ShakeCamera(float intensity = 10f, float time = 0.1f)
    {
        virtualCameraNoise = virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        virtualCameraNoise.m_AmplitudeGain = intensity; 
        virtualCameraNoise.m_FrequencyGain = 100f;
        cameraShakeIntensity = intensity;
        cameraShakeTime = time;
        cameraShakeTimer = time;
    }

    public void RespawnPlayer()
    {
        ShakeCamera(10f, 0.1f);
        // 销毁所有带有“GeneratedBubble”标签的物体
        GameObject[] bubbles = GameObject.FindGameObjectsWithTag("GeneratedBubble");
        foreach (GameObject bubble in bubbles)
        {
            DestroyGameObject(bubble);
        }
        if (player != null)
        {
            DestroyGameObject(player);
        }
        SpawnAtPoint();
    }

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnAtPoint();
    }

    private void Update()
    {
        if (cameraShakeTimer > 0)
        {
            // The first 20% of the shake time is linear increase
            // The last 80% of the shake time is linear decrease
            cameraShakeTimer -= Time.deltaTime;
            if (cameraShakeTimer > cameraShakeTime * 0.2f)
            {
                float shakeIntensity = (float)(cameraShakeIntensity * (0.4 + 0.6 * (cameraShakeTimer / cameraShakeTime)));
                virtualCameraNoise.m_AmplitudeGain = shakeIntensity;
            }
            else
            {
                float shakeIntensity = (float)(cameraShakeIntensity * (1 - (cameraShakeTimer / cameraShakeTime)));
                virtualCameraNoise.m_AmplitudeGain = shakeIntensity;
            }
            if (cameraShakeTimer < 0)
            {
                virtualCameraNoise.m_AmplitudeGain = 0;
                virtualCameraNoise.m_FrequencyGain = 0f; 
            }
        }
    }

    #endregion

    #region Private Methods

    private void SpawnAtPoint()
    {
        if (spawnPoint != null && spawnPlayer != null)
        {
            player = Instantiate(spawnPlayer, spawnPoint.position, spawnPoint.rotation);
            virtualCamera.Follow = player.transform; // 设置虚拟相机追踪角色
        }
    }

    #endregion

}
