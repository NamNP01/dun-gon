using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyHPBar hpBar;

    void Start()
    {
    }

    public void TakeDamage(int damage)
    {
        if (hpBar != null)
        {
            Debug.LogWarning("1");
            hpBar.TakeDamage(damage);
            
        }
    }

    public float GetCurrentHP()
    {
        return hpBar != null ? hpBar.GetCurrentHP() : 0;
    }
}
