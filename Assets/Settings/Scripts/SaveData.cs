[System.Serializable]
public class SaveData
{
    // Scene & transform
    public string sceneName;
    public float x, y, z;
    public float rotZ;

    // Player stats (we guys need to add more as we progress)
    public int health;
    public int coins;

    // Versioning for future changes
    public int version = 1;
}
