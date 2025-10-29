using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    
    // Event for when damage is taken
    public System.Action OnDamaged;
    public System.Action OnDeath;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        
        // Call damage event
        OnDamaged?.Invoke();
        
        // Check for death
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnDeath?.Invoke();
            Die();
        }
    }

    private void Die()
    {
        // Handle death logic here
        Debug.Log(gameObject.name + " has died!");
    }
}