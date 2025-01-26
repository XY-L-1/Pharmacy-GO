using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public int sceneBuildIndex;
     

    // If collider is a play, load the next scene
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Trigger Entered");

        // Could use other.GerComponent<PlayerControl>() to see if the game object is a Player component
        // Tags work too. Maybe some players have different script components
        if (other.tag == "Player")
        {
            print("Player Entered" + sceneBuildIndex);
            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
    }

}
