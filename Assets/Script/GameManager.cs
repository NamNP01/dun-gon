using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerData playerData; // Kéo ScriptableObject của nhân vật vào Inspector

    private void OnApplicationQuit()
    {
        playerData.ResetStats();
    }
}
