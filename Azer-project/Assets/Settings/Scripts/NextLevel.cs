using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Load by name
            SceneController.instance.NextLevel();            
            // OR load by build index (uncomment one):
            // int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            // SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }
}