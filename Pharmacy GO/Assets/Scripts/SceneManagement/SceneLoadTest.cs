using UnityEngine;
using UnityEngine.SceneManagement;

public class AsyncLoadTest : MonoBehaviour
{
    [SerializeField] private int sceneBuildIndexToTest = 1; // Replace with Route1's index

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) // Press 'L' to test async load
        {
            Debug.Log($"Starting async load for scene index: {sceneBuildIndexToTest}");
            StartCoroutine(TestLoadSceneAsync(sceneBuildIndexToTest));
        }
    }

    private System.Collections.IEnumerator TestLoadSceneAsync(int buildIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(buildIndex);
        Debug.Log("Async operation started.");

        while (!operation.isDone)
        {
            Debug.Log($"Loading progress: {operation.progress * 100}%");
            yield return null;
        }

        Debug.Log("Async operation completed!");
    }
}
