using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    [SerializeField] public Animator transitionAnim;
    public static SceneController instance;    
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            FindTransitionAnimator();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Small delay to ensure scene is fully loaded before finding animator
        Invoke(nameof(FindTransitionAnimator), 0.1f);
    }

    void FindTransitionAnimator()
    {
        // Method 1: Find by name "Scene Transition"
        GameObject transitionObject = GameObject.Find("Scene Transition");
        
        // Method 2: If not found by name, try finding any object with the animator
        if (transitionObject == null)
        {
            Animator[] allAnimators = FindObjectsOfType<Animator>();
            foreach (Animator anim in allAnimators)
            {
                if (anim.runtimeAnimatorController != null && 
                    anim.runtimeAnimatorController.name == "Scene Transition")
                {
                    transitionObject = anim.gameObject;
                    break;
                }
            }
        }

        if (transitionObject != null)
        {
            transitionAnim = transitionObject.GetComponent<Animator>();
            Debug.Log("Found transition animator: " + transitionObject.name);
            
            // Verify the parameters exist
            if (transitionAnim != null)
            {
                VerifyAnimatorParameters();
            }
        }
        else
        {
            Debug.LogError("No 'Scene Transition' object found in the scene!");
            transitionAnim = null;
        }
    }

    void VerifyAnimatorParameters()
    {
        bool hasEnd = false;
        bool hasStart = false;
        
        foreach (AnimatorControllerParameter param in transitionAnim.parameters)
        {
            if (param.name == "End") hasEnd = true;
            if (param.name == "Start") hasStart = true;
        }

        if (!hasEnd || !hasStart)
        {
            Debug.LogError($"Animator missing parameters! End: {hasEnd}, Start: {hasStart}");
            transitionAnim = null; // Don't use invalid animator
        }
        else
        {
            Debug.Log("Animator parameters verified: End and Start found");
        }
    }

    // Original method - loads next scene by build index
    public void NextLevel()
    {
        StartCoroutine(LoadLevelByIndex());
    }

    // NEW method - loads scene by name
    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadLevelByName(sceneName));
    }

    IEnumerator LoadLevelByIndex()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        // Double-check we have a valid animator with the right parameters
        if (transitionAnim != null && VerifyAnimatorHasParameters())
        {
            Debug.Log("Starting transition with animator: " + transitionAnim.gameObject.name);
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(1);
        }
        else
        {
            Debug.LogWarning("No valid transition animator, using fallback");
            yield return new WaitForSeconds(0.5f);
        }
        
        // Load the next scene by index
        SceneManager.LoadSceneAsync(nextSceneIndex);
        
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("Start");
        }
    }

    IEnumerator LoadLevelByName(string sceneName)
    {
        Debug.Log($"Loading scene by name: {sceneName}");
        
        // Check if scene exists
        if (!DoesSceneExist(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' does not exist in Build Settings!");
            yield break;
        }
        
        // Double-check we have a valid animator with the right parameters
        if (transitionAnim != null && VerifyAnimatorHasParameters())
        {
            Debug.Log("Starting transition with animator: " + transitionAnim.gameObject.name);
            transitionAnim.SetTrigger("End");
            yield return new WaitForSeconds(1);
        }
        else
        {
            Debug.LogWarning("No valid transition animator, using fallback");
            yield return new WaitForSeconds(0.5f);
        }
        
        // Load the scene by name
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        if (transitionAnim != null)
        {
            transitionAnim.SetTrigger("Start");
        }
    }

    bool VerifyAnimatorHasParameters()
    {
        if (transitionAnim == null) return false;
        
        bool hasEnd = false;
        bool hasStart = false;
        
        foreach (AnimatorControllerParameter param in transitionAnim.parameters)
        {
            if (param.name == "End") hasEnd = true;
            if (param.name == "Start") hasStart = true;
        }
        
        return hasEnd && hasStart;
    }
    
    // Helper method to check if a scene exists in build settings
    private bool DoesSceneExist(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}