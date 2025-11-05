using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-50)]

public class PlayerSaveBridge : MonoBehaviour
{
    [Header("Assign Player in Inspector")]
    public Player player;                 // drag your Player here

    [Header("Health persistence toggles")]
    public bool loadHealth = true;        // load HP at fresh session start
    public bool saveHealth = true;        // save HP on quit

    void Start()
    {
        // If we just died, respawn fresh; don't load a save now
        if (RespawnState.IsRespawning)
            return;

        if (SaveManager.I != null && SaveManager.I.TryLoad(out var data))
            Apply(data);
    }

    public void SaveNow()
    {
        if (SaveManager.I == null || player == null) return;

        var data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            x = player.transform.position.x,
            y = player.transform.position.y,
            z = player.transform.position.z,
            rotZ = player.transform.eulerAngles.z,

            coins = player.coins
        };

        if (saveHealth)
            data.health = Mathf.Clamp(player.health, 0, player.maxHealth);

        SaveManager.I.Save(data);
    }


    public void Apply(SaveData data)
    {
        if (player == null || data == null) return;

        // Position / rotation
        player.transform.position = new Vector3(data.x, data.y, data.z);
        player.transform.eulerAngles = new Vector3(
            player.transform.eulerAngles.x,
            player.transform.eulerAngles.y,
            data.rotZ
        );

        // HP: only load at fresh session start (not during respawn)
        if (loadHealth)
            player.health = Mathf.Clamp(data.health, 0, player.maxHealth);
        else
            player.health = player.maxHealth;

        // Other stats
        player.coins = data.coins;

        // Refresh bar once
        var updateUi = player.GetType().GetMethod(
            "UpdateHealthUI",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        updateUi?.Invoke(player, null);
    }

    void OnApplicationQuit()
    {
        
        if (!RespawnState.IsRespawning)
            SaveNow();
    }

}
