using UnityEngine;

public class RespawnHotkey : MonoBehaviour
{
    [Header("Assign your starting spawn point")]
    public Transform spawnPoint;

    [Header("Optional settings")]
    public KeyCode respawnKey = KeyCode.R;   // Press R to respawn
    public bool resetVelocity = true;        // stops all motion when respawning

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(respawnKey))
        {
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.position;
                if (resetVelocity && rb != null)
                    rb.linearVelocity = Vector2.zero;

                Debug.Log("[RespawnHotkey] Teleported to starting position.");
            }
            else
            {
                Debug.LogWarning("[RespawnHotkey] No spawn point assigned!");
            }
        }
    }
}
