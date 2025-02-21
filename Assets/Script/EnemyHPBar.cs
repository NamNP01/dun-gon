using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    public Slider hpSlider;
    public Transform enemy;
    private float maxHP;
    private float currentHP;
    private float displayedHP;
    public float smoothSpeed = 2f;

    public Vector3 hpBarOffset = new Vector3(0, 2f, 0);
    private EnemyHP enemyHP;

    void Start()
    {
        if (enemy != null)
        {
            enemyHP = enemy.GetComponent<EnemyHP>();
        }

        if (enemyHP != null)
        {
            SetMaxHP(enemyHP.maxHP); // Nhận maxHP từ EnemyHP
            currentHP = enemyHP.maxHP;
        }

        displayedHP = currentHP;
        UpdateHPBar();
    }

    void Update()
    {
        if (enemy == null)
        {
            DestroyParent();
            return;
        }

        transform.position = enemy.position + hpBarOffset;

        if (displayedHP > currentHP)
        {
            displayedHP = Mathf.Lerp(displayedHP, currentHP, Time.deltaTime * smoothSpeed);
            UpdateHPBar();
        }
    }

    public void SetMaxHP(int maxHP)
    {
        this.maxHP = maxHP;
        this.currentHP = maxHP;
        displayedHP = maxHP;
        UpdateHPBar();
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        if (enemyHP != null)
        {
            enemyHP.currentHP = (int)currentHP; // Cập nhật máu về EnemyHP
        }

        if (currentHP <= 0)
        {
            DestroyParent();
        }
    }

    private void UpdateHPBar()
    {
        hpSlider.value = displayedHP / maxHP;
    }

    public void DestroyParent()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }
}
