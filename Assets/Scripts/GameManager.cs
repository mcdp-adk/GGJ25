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

    public GameObject GetPlayer()
    {
        return player;
    }

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
        var playerObj = GetPlayer();
        if (playerObj != null)
        {
            if (cameraShakeTimer > 0)
            {
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
    }

    #endregion

    #region Private Methods

    void SpawnAtPoint()
    {
        if (spawnPoint != null && spawnPlayer != null)
        {
            player = Instantiate(spawnPlayer, spawnPoint.position, spawnPoint.rotation);
            virtualCamera.Follow = player.transform; 
        }
    }

    #endregion

}
