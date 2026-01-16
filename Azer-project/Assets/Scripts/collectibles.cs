using UnityEngine;

public class Collectibles : MonoBehaviour
{
    public AudioClip coinCollect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if this collectible is a coin
            if (gameObject.CompareTag("coin"))
            {
                Player player = collision.gameObject.GetComponent<Player>();
                if (player != null)
                {
                    player.coins += 1;
                    player.PlaySFX(coinCollect, 0.4f,1.75f);
                    Destroy(gameObject);
                }
            }
        }
    }
}