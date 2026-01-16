using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Movement
    public float speed = 2f;
    public Transform[] points;
    
    // Health and Combat
    public Health health;
    public Animator animator;
    
    private int i;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDead = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Disable gravity for flying enemies
        rb.gravityScale = 0f;
        
        transform.position = points[0].position;
    }

    private void OnEnable()
    {
        // Subscribe to damage events - THIS IS WHAT WAS MISSING
        if (health != null)
        {
            health.OnDamaged += HandleDamage;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from events
        if (health != null)
        {
            health.OnDamaged -= HandleDamage;
        }
    }

    void Update()
    {
        if (isDead) return; // Stop movement if dead
        
        if(Vector2.Distance(transform.position, points[i].position) < 0.01f)
        {
            i++;
            if(i == points.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
        
        spriteRenderer.flipX = (points[i].position.x - transform.position.x) < 0f;
        
        // Set animation based on state
        SetAnimation();
    }

    void SetAnimation()
    {
        if (animator == null) return;

        if (isDead)
        {
            animator.Play("bat_death");
        }
        else
        {
            // Play normal bat movement animation
            animator.Play("bat_fly"); // Change this to your bat's normal animation name
        }
    }

    void HandleDamage()
    {
        // Check if health is 0 or below and handle death
        if (health.currentHealth <= 0 && !isDead)
        {
            isDead = true;
            SetAnimation(); // Play death animation
            Destroy(gameObject, 1f); // Destroy after 1 second to let animation play
        }
    }
}