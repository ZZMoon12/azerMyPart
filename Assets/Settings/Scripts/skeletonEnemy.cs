using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton_Enemy : MonoBehaviour 
{
    #region Public Variables
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance; //Minimum distance for attack
    public float moveSpeed;
    public float timer; //Timer for cooldown between attacks
    public Health health; // Add Health component reference
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private GameObject target;
    private Animator anim;
    private float distance; //Store the distance b/w enemy and player
    private bool attackMode;
    private bool inRange; //Check if Player is in range
    private bool cooling; //Check if Enemy is cooling after attack
    private float intTimer;
    private bool isDead = false;
    #endregion

    void Awake()
    {
        intTimer = timer; //Store the initial value of timer
        anim = GetComponent<Animator>();
        health = GetComponent<Health>(); // Get Health component
    }

    void OnEnable()
    {
        // Subscribe to damage events like the bat enemy
        if (health != null)
        {
            health.OnDamaged += HandleDamage;
        }
    }

    void OnDisable()
    {
        // Unsubscribe from events
        if (health != null)
        {
            health.OnDamaged -= HandleDamage;
        }
    }

    void Update () 
    {
        if (isDead) return; // Stop everything if dead

        if (inRange && target != null)
        {
            // Check which direction the enemy is facing
            Vector2 rayDirection = (target.transform.position.x < transform.position.x) ? Vector2.left : Vector2.right;
            hit = Physics2D.Raycast(rayCast.position, rayDirection, rayCastLength, raycastMask);
            RaycastDebugger(rayDirection);
        }

        //When Player is detected
        if(hit.collider != null && hit.collider.CompareTag("Player"))
        {
            EnemyLogic();
        }
        else if(hit.collider == null)
        {
            inRange = false;
        }

        if(inRange == false)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
        }

        // Only run cooldown if we're actually cooling
        if (cooling)
        {
            Cooldown();
        }
    }

    void OnTriggerEnter2D(Collider2D trig)
    {
        if(trig.gameObject.CompareTag("Player"))
        {
            target = trig.gameObject;
            inRange = true;
            Debug.Log("Player detected in range!");
        }
    }

    void OnTriggerExit2D(Collider2D trig)
    {
        if(trig.gameObject.CompareTag("Player"))
        {
            inRange = false;
            target = null;
            Debug.Log("Player left range!");
        }
    }

    void EnemyLogic()
    {
        if (target == null) return;
        
        distance = Vector2.Distance(transform.position, target.transform.position);

        if(distance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if(attackDistance >= distance && cooling == false)
        {
            Attack();
        }
    }

    void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("skelly_attack"))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);
            
            // Flip sprite based on player position
            if (target.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        timer = intTimer; //Reset Timer when Player enter Attack Range
        attackMode = true; //To check if Enemy can still attack or not

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
        Debug.Log("Attacking player!");
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;
        Debug.Log("Cooldown timer: " + timer);

        if(timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            attackMode = false;
            timer = intTimer;
            Debug.Log("Cooldown finished - ready to attack again!");
        }
    }

    void StopAttack()
    {
        // Only stop attack animation if not in attack mode
        if (!attackMode)
        {
            anim.SetBool("Attack", false);
        }
    }

    void RaycastDebugger(Vector2 direction)
    {
        if(distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.red);
        }
        else if(attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, direction * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
        Debug.Log("Cooldown triggered!");
    }
    
    // Add this method to actually damage the player
    public void DealDamage()
    {
        if (target != null && distance <= attackDistance)
        {
            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(10); 
                Debug.Log("Dealt damage to player!");
            }
        }
    }

    // Health damage handler like the bat enemy
    void HandleDamage()
    {
        // Check if health is 0 or below and handle death
        if (health.currentHealth <= 0 && !isDead)
        {
            isDead = true;
            anim.SetBool("canWalk", false);
            anim.SetBool("Attack", false);
            anim.Play("skelly_death"); 
            Destroy(gameObject, 1f); // Destroy after 1 second to let animation play
        }
    }
}