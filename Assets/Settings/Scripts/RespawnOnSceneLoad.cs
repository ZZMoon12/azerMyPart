using UnityEngine;
using System.Collections;

[DefaultExecutionOrder(-100)]
public class RespawnOnSceneLoad : MonoBehaviour
{
    [Header("Assign or leave null to auto-find by name 'SpawnPoint'")]
    public Transform spawnPoint;

    [Header("Ground snap (optional)")]
    public LayerMask groundLayer;
    public float raycastUp = 2f;
    public float raycastDown = 10f;
    public float safeLift = 0.2f;

    private Player player;
    private Rigidbody2D rb;

    void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        // If not a respawn, do nothing
        if (!RespawnState.IsRespawning) return;

        // Find spawn if not assigned
        if (spawnPoint == null)
        {
            var go = GameObject.Find("SpawnPoint");
            if (go != null) spawnPoint = go.transform;
        }

        // Fallback: use current position if no spawn found
        Vector3 target = (spawnPoint != null) ? spawnPoint.position : transform.position;

        // Snap to ground if possible
        var hit = Physics2D.Raycast((Vector2)target + Vector2.up * raycastUp, Vector2.down, raycastDown, groundLayer);
        if (hit.collider != null)
            target = hit.point + Vector2.up * safeLift;

        // Safely teleport: pause physics for a frame
        StartCoroutine(DoSafeTeleportAndHeal(target));
    }

    IEnumerator DoSafeTeleportAndHeal(Vector3 target)
    {
        // stop physics while we move
        bool hadRb = rb != null;
        bool prevSim = hadRb ? rb.simulated : false;
        if (hadRb) rb.simulated = false;

        // move & zero motion
        transform.position = target;
        if (hadRb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // FULL HEAL & refresh bar
        if (player != null)
        {
            if (player.maxHealth <= 0) player.maxHealth = 100; // safety
            player.health = player.maxHealth;
            player.health = Mathf.Clamp(player.health, 0, player.maxHealth);
            if (player.healthImage != null)
                player.healthImage.fillAmount = player.health / (float)player.maxHealth;
        }

        // clear the flag so normal flow resumes
        RespawnState.IsRespawning = false;

        // wait one frame, then resume physics
        yield return null;
        if (hadRb) rb.simulated = prevSim;
    }
}
