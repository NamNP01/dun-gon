using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    public PlayerHpBar hpBar;
    public PlayerData playerData; // 🛑 Thêm biến tham chiếu PlayerData
    public int currentHP;
    public GameObject damagePopupPrefab;

    void Start()
    {
        if (playerData != null)
        {
            currentHP = playerData.HP; // 🛑 Lấy maxHP từ PlayerData
        }
        else
        {
            Debug.LogError("PlayerData chưa được gán vào PlayerHP!");
        }

        if (hpBar != null)
        {
            playerData.HP = currentHP;
            hpBar.currentHp = currentHP;
            hpBar.UpdateHpText();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            WebProjectile enemy = other.GetComponent<WebProjectile>();
            if (enemy != null)
            {
                TakeDamage(enemy.Damage);
            }
            CactusProjectile enemy2 = other.GetComponent<CactusProjectile>();
            if(enemy2 != null)
            {
                TakeDamage(enemy2.Damage);
            }            
            CoinProjectile enemy3 = other.GetComponent<CoinProjectile>();
            if(enemy3 != null)
            {
                TakeDamage(enemy3.Damage);
                Destroy(enemy3);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Monster"))
        {
            EnemyHP enemy = collision.gameObject.GetComponent<EnemyHP>();
            if (enemy != null)
            {
                TakeDamage(enemy.Damage);
            }
        }
    }

    public void TakeDamage(int damage, bool isCriticalHit = false)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        if (hpBar != null)
        {
            hpBar.currentHp = currentHP;
            hpBar.UpdateHpText();
        }

        Debug.Log(isCriticalHit ? $"🔥 Player nhận sát thương chí mạng! {damage} HP" : $"💥 Player nhận {damage} sát thương thường, còn lại {currentHP} HP");

        ShowDamagePopup(damage, isCriticalHit);

        if (currentHP <= 0)
        {
            Die();
        }
    }


    private void ShowDamagePopup(float damage, bool isCriticalHit)
    {
        if (damagePopupPrefab != null)
        {
            GameObject popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 2, Quaternion.identity);
            popup.transform.rotation = Camera.main.transform.rotation;
            popup.transform.localScale *= 0.8f;
            popup.GetComponent<DamagePopup>().Setup((int)damage, isCriticalHit);
        }
    }


    private void Die()
    {
        Debug.Log("Player đã chết!");
    }
}
