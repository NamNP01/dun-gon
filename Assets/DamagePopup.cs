using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    private float disappearTimer;
    private Color textColor;

    public void Setup(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
        textColor = damageText.color;
        disappearTimer = 1f; // Thời gian tồn tại
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

            if (textColor.a <= 0) Destroy(gameObject);
        }
    }
}
