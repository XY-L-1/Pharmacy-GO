using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int sceneBuildIndex; // Target scene index
    public string targetSpawnPointID; // Target spawn point ID
    public Direction entryDirection; // Direction for positioning
    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning) return;

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Portal triggered by: {other.name}");
            StartCoroutine(TransitionToScene());
        }
    }

    private IEnumerator TransitionToScene()
    {
        isTransitioning = true;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        yield return new WaitUntil(() => operation.isDone);
        isTransitioning = false;
        Debug.Log("Scene load complete. Looking for spawn point...");

        // Reposition the player
        var player = PlayerControl.Instance;
        if (player != null)
        {
            SpawnPoint targetSpawnPoint = FindTargetSpawnPoint();
            if (targetSpawnPoint != null)
            {
                player.transform.position = targetSpawnPoint.transform.position;
                Debug.Log($"Player moved to spawn point: {targetSpawnPoint.transform.position}");

                // Reset input and movement
                player.ResetInput();
                player.DisableMovementTemporarily();
            }
            else
            {
                Debug.LogError("Spawn point not found!");
            }
        }
        else
        {
            Debug.LogError("Player instance not found!");
        }

        isTransitioning = false;
    }

    public SpawnPoint FindTargetSpawnPoint()
    {
        SpawnPoint[] spawnPoints = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnPointID == targetSpawnPointID)
            {
                return spawnPoint;
            }
        }

        Debug.LogError($"No SpawnPoint found with ID: {targetSpawnPointID}");
        return null;
    }

    private void UpdatePlayerPosition(GameObject player, Direction direction)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += 5;
                break;
            case Direction.Down:
                newPos.y -= 5;
                break;
            case Direction.Left:
                newPos.x -= 5;
                break;
            case Direction.Right:
                newPos.x += 5;
                break;
        }

        player.transform.position = newPos;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
