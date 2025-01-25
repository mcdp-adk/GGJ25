using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject spawnPlayer;

    public static GameManager Instance { get; private set; }
    private GameObject player;

    #region Public Methods

    // 获取 Player 对象
    public GameObject GetPlayer()
    {
        return player;
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
        }
    }

    #endregion

}
