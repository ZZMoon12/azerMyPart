using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public float speed = 5f;
    public int facingDirection = 1;
    public Vector2 moveInput;
    
    // Jump additions
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public int extraJumpsValue = 1;
    public int coins;

    public Image healthImage;

    public int damage = 10;
    public float attackRadius = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayer;
    
    private bool isGrounded;
    private int extraJumps;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isAttacking = false;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;
    private AudioSource audioSource;
    public float baseScale = 4f; // Change this in Inspector per level


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
        audioSource = GetComponent<AudioSource>();

        if (RespawnState.IsRespawning)
        {
            // FULL HEAL on respawn
            health = maxHealth;
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthUI();

            // DO NOT clear the flag immediately — wait one frame
            StartCoroutine(ClearRespawnFlagNextFrame());
        }
        else
        {
            // Fresh session / normal start
            health = Mathf.Clamp(health, 0, maxHealth);
            UpdateHealthUI();
        }

        Debug.Log("Player initialized - waiting for attack input");
    }

    private System.Collections.IEnumerator ClearRespawnFlagNextFrame()
    {
        // Let all Start()s (including PlayerSaveBridge) run while the flag is still true
        yield return null;
        RespawnState.IsRespawning = false;
    }

    void Update()
    {
        Flip();
        
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Reset extra jumps when grounded
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }
        
        // Update animations
        SetAnimation(moveInput.x);

        // Check if attack animation is finished
        if (isAttacking && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = false;
            animator.SetBool("isAttacking", false);
            Debug.Log("Attack finished - can attack again");
        }

    }
    
    void FixedUpdate()
    {
        // Don't move horizontally while attacking
        if (!isAttacking)
        {
            float targetSpeed = moveInput.x * speed;
            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Stop horizontal movement during attack
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

void Flip()
    {
        if (moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        else if (moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }
        
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            transform.localScale = new Vector3(baseScale * facingDirection, baseScale, baseScale);
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move input: " + moveInput);
    }

    // Jump input method
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Jump button pressed");
            if (!isAttacking) // Can't jump while attacking
            {
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    Debug.Log("Jumped - grounded");
                }
                else if (extraJumps > 0)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                    extraJumps--;
                    Debug.Log("Jumped - extra jump. Remaining: " + extraJumps);
                }
            }
            else
            {
                Debug.Log("Jump blocked - currently attacking");
            }
        }
    }

    public void OnAttack(InputValue value)
    {
        Debug.Log("Attack button pressed - isPressed: " + value.isPressed);
        
        if (value.isPressed)
        {
            Debug.Log("Attack input detected");
            Debug.Log("Can attack? isAttacking: " + isAttacking + ", Cooldown ready: " + (Time.time >= lastAttackTime + attackCooldown));
            
            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
            else
            {
                Debug.Log("Attack blocked - either already attacking or on cooldown");
            }
        }
    }

    void StartAttack()
    {
        Debug.Log("Starting attack!");
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.SetBool("isAttacking", true);
        Debug.Log("Attack animation triggered");
        
        // Perform attack logic
        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);
        if (enemy != null)
        {
            Debug.Log("Enemy hit: " + enemy.gameObject.name);
            Health enemyHealth = enemy.gameObject.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.ChangeHealth(-damage);
                Debug.Log("Enemy damaged: " + damage + " damage dealt");
            }
            else
            {
                Debug.Log("No Health component found on enemy");
            }
        }
        else
        {
            Debug.Log("No enemy in attack range");
        }
    }

    private void SetAnimation(float moveInput)
    {
        if (animator == null) return;

        if (isAttacking)
        {
            // Attack animation takes priority
            return;
        }

        if (isGrounded)
        {
            if (moveInput == 0)
                animator.Play("player_idle");
            else
                animator.Play("player_run");
        }
        else
        {
            if (rb.linearVelocity.y > 0)
                animator.Play("player_jump");
            else
                animator.Play("player_fall");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            TakeDamage(25); // <-- centralize damage here so UI & clamping always happen
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f); 
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        RespawnState.IsRespawning = true;  // tell the next scene it's a respawn
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f, float pitch = 1.5f)
    {
        if (audioSource == null || audioClip == null) return;

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.pitch = pitch; // Higher pitch = faster playback
        audioSource.Play();
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();                 // single source of truth for UI

        StartCoroutine(BlinkRed());
        if (health <= 0) Die();
    }

    private void UpdateHealthUI()
    {
        if (healthImage != null)
            healthImage.fillAmount = Mathf.Clamp01(health / (float)maxHealth);
    }

}
