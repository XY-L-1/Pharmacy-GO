using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { FreeRoam, Battle, Dialogue }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControl playerControl;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    public static GameController Instance { get; private set; }

    private bool isInitialLoad = true; // Flag to track initial scene load

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        playerControl.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialogue;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialogue)
                state = GameState.FreeRoam;
        };

        if (CoinManager.Instance == null)
        {
            GameObject coinManager = new GameObject("CoinManager");
            coinManager.AddComponent<CoinManager>();
        }
    }

    void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        playerControl.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle();
    }

    void EndBattle(bool playerWin)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        playerControl.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControl.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialogue)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find the Main Camera in the new scene
        var mainCamera = Camera.main;

        if (mainCamera != null)
        {
            // Ensure FollowCam is attached to the Main Camera
            var followCam = mainCamera.GetComponent<FollowCam>();
            if (followCam == null)
            {
                followCam = mainCamera.gameObject.AddComponent<FollowCam>();
                Debug.Log("FollowCam script added to Main Camera.");
            }

            // Assign the player as the follow target
            var player = PlayerControl.Instance;
            if (player != null)
            {
                followCam.player = player.gameObject;
                Debug.Log("FollowCam assigned to follow the player.");
            }
            else
            {
                Debug.LogError("Player instance not found!");
            }
        }
        else
        {
            Debug.LogError("Main Camera not found in the scene!");
        }

        // Only update player spawn position if it's not the initial load
        if (!isInitialLoad)
        {
            UpdatePlayerSpawnPosition();
        }
        else
        {
            isInitialLoad = false; 
        }
    }

    // private void UpdatePlayerSpawnPosition()
    // {
    //     var player = PlayerControl.Instance;
    //     if (player != null)
    //     {
    //         var portal = FindFirstObjectByType<Portal>();
    //         if (portal != null)
    //         {
    //             var spawnPoint = portal.FindTargetSpawnPoint();
    //             if (spawnPoint != null)
    //             {
    //                 player.transform.position = spawnPoint.transform.position;
    //                 Debug.Log($"Player repositioned to spawn point: {spawnPoint.transform.position}");
    //             }
    //             else
    //             {
    //                 Debug.LogError("Spawn point not found! Player position will not be updated.");
    //             }
    //         }
    //         else
    //         {
    //             Debug.LogWarning("No active portal found in the scene. Player position will not be updated.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogError("Player instance not found!");
    //     }
    // }

    private void UpdatePlayerSpawnPosition()
    {
        var player = PlayerControl.Instance;
        if (player != null)
        {
            var portal = FindFirstObjectByType<Portal>();
            if (portal != null)
            {
                var spawnPoint = portal.FindTargetSpawnPoint();
                if (spawnPoint != null)
                {
                    player.transform.position = spawnPoint.transform.position;
                }
            }
        }
    }

}