using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    private float disappearTimer;
    private Color textColor;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale; // Lưu lại kích thước gốc
    }

    public void Setup(int damageAmount, bool isCriticalHit)
    {
        // 🔥 Đặt lại giá trị ban đầu
        ResetPopup();

        damageText.text = "-" + damageAmount.ToString();
        textColor = damageText.color;
        disappearTimer = 1f; // Thời gian tồn tại

        if (isCriticalHit)
        {
            damageText.color = Color.red; // Màu đỏ nếu chí mạng
            transform.localScale = originalScale * 1.2f; // To hơn 1.2 lần
        }
    }

    void Update()
    {
        float moveSpeed = 1f;
        float fadeSpeed = 3f;

        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        disappearTimer -= Time.deltaTime;

        if (disappearTimer < 0)
        {
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            if (textColor.a <= 0) ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        DamagePopupPool.Instance.ReturnPopup(gameObject);
    }

    private void ResetPopup()
    {
        // 🔥 Reset trạng thái ban đầu
        damageText.color = Color.white; // Reset màu về trắng
        transform.localScale = originalScale; // Reset kích thước
        textColor.a = 1f; // Đặt lại độ trong suốt
    }
}
