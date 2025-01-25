using UnityEngine;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject spawnPlayer;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public static GameManager Instance { get; private set; }
    private GameObject player;

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

    // 重生玩家
    public void RespawnPlayer()
    {
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
