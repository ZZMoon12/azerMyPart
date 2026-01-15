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

    public void NextLevel()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
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
        
        // Load the next scene
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        transitionAnim.SetTrigger("Start");

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
}