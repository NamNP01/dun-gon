using System.Collections;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public PlayerData playerData;

    public EnemyHPBar hpBar;
    public GameObject damagePopupPrefab; // Prefab của popup sát thương

    public int maxHP = 1200;
    public int currentHP;
    public int Damage = 120;

    public int expReward = 1; // 🔥 EXP rơi ra khi chết

    private bool isKnockedBack = false;
    private bool isDead = false;

    private BigFreeBurrow bigFreeBurrow;

    void Start()
    {
        PlayerData.OnLevelUp += BoostStats; // Đăng ký sự kiện level up
        bigFreeBurrow = GetComponent<BigFreeBurrow>();

        currentHP = maxHP; // Khởi tạo máu ban đầu
        if (hpBar != null)
        {
            hpBar.SetMaxHP(maxHP); // Gửi maxHP cho hpBar
        }
    }
    private void OnDestroy()
    {
        PlayerData.OnLevelUp -= BoostStats; // Hủy đăng ký khi enemy bị xóa
    }
    private void BoostStats()
    {
        maxHP = Mathf.RoundToInt(maxHP * 1.3f);  // Tăng 30% máu
        currentHP = Mathf.RoundToInt(currentHP * 1.3f);
        Damage = Mathf.RoundToInt(Damage * 1.3f); // Tăng 30% sát thương

        if (hpBar != null)
        {
            hpBar.SetMaxHP(maxHP);
        }

        Debug.Log($"🔥 Enemy {gameObject.name} boosted! New HP: {maxHP}, Damage: {Damage}");
    }

    public void TakeDamage(int damage, bool isCriticalHit)
    {
        if (hpBar != null)
        {
            hpBar.TakeDamage(damage);
            currentHP = (int)hpBar.GetCurrentHP(); // Cập nhật máu từ hpBar về EnemyHP
        }

        // 🔥 Hiển thị Damage Popup
        if (DamagePopupPool.Instance != null)
        {
            GameObject popup = DamagePopupPool.Instance.GetPopup();
            popup.transform.position = transform.position + Vector3.up * 2;
            popup.transform.rotation = Camera.main.transform.rotation;
            popup.GetComponent<DamagePopup>().Setup(damage, isCriticalHit);
        }

        if (bigFreeBurrow != null)
        {
            //bigFreeBurrow.OnTakeDamage();
        }

        if (currentHP <= 0)
        {
            Die();
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
    private void Die()
    {
        if (isDead) return; 

        isDead = true; 
        // 🔥 Cộng EXP cho người chơi khi enemy chết
        if (playerData != null)
        {
            playerData.GainExp(expReward);
        }

        Destroy(gameObject); // Xóa enemy khỏi game
    }

}
