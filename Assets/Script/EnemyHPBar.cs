using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Slider hpSlider;
    public Transform enemy;
    public float MaxHP = 1000f;
    private float currentHP;

    public Vector3 hpBarOffset = new Vector3(0, 2f, 0);

    void Start()
    {
        currentHP = MaxHP;
        UpdateHPBar();
    }

    void Update()
    {
        if (enemy == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = enemy.position + hpBarOffset;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHPBar();

        if (currentHP <= 0)
        {
            Destroy(enemy.gameObject);
        }
    }

    private void UpdateHPBar()
    {
        hpSlider.value = currentHP / MaxHP;
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
