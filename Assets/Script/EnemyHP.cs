using System.Collections;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public EnemyHPBar hpBar;

    public int maxHP = 1200;
    public int currentHP;
    public int Dame=120;

    private bool isKnockedBack = false;

    void Start()
    {
        currentHP = maxHP; // Khởi tạo máu ban đầu
        if (hpBar != null)
        {
            hpBar.SetMaxHP(maxHP); // Gửi maxHP cho hpBar
        }
    }

    public void TakeDamage(int damage)
    {
        if (hpBar != null)
        {
            hpBar.TakeDamage(damage);
            currentHP = (int)hpBar.GetCurrentHP(); // Cập nhật máu từ hpBar về EnemyHP
        }
    }

    public float GetCurrentHP()
    {
        return currentHP;
    }

    public void ApplyKnockback(Vector3 direction, float distance, float speed)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackRoutine(direction, distance, speed));
        }
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float distance, float speed)
    {
        isKnockedBack = true;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + (direction * distance);

        float elapsedTime = 0f;
        while (elapsedTime < distance / speed)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / (distance / speed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isKnockedBack = false;
    }
}
