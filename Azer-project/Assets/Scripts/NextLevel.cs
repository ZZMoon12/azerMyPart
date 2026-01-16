using UnityEngine.SceneManagement;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private string targetSceneName; // Scene to load by name
    [SerializeField] private string exitID; // Optional identifier for this exit
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Player entered exit: {exitID}, loading scene: {targetSceneName}");
            
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                // Call SceneController to load scene by name
                SceneController.instance.LoadSceneByName(targetSceneName);
            }
            else
            {
                Debug.LogError("No target scene name specified for this exit!");
            }
        }
    }}
    