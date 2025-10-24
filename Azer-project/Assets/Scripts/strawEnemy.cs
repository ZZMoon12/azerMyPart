using UnityEngine;

public class strawEnemy : MonoBehaviour
{
    public Animator animator;
    public Health health;

    private void OnEnable()
    {
        health.OnDamaged += HandleDamage;
    }
    private void OnDisable()
    {
        health.OnDamaged -= HandleDamage;
    }
    
    void HandleDamage()
    {
        animator.SetTrigger("isDamaged");
    }
}
