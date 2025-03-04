using UnityEngine;

public class Angel : MonoBehaviour
{
    private bool hasTriggered = false;
    public Light angelLight;
    public AngelType angelType; // 🆕 Xác định Angel nào (1, 2, 3)

    public enum AngelType
    {
        CritAngel, // Heal + Crit Master Minor
        DamageAngel, // Heal + Attack Boost Minor
        SpeedAngel // Heal + Attack Speed Minor
    }

    private void Start()
    {
        angelLight = GetComponentInChildren<Light>();

        if (angelLight == null)
        {
            Debug.LogWarning("⚠ Angel không có Light con!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            Debug.Log($"👼 Angel xuất hiện ({angelType})! Mở bảng chọn kỹ năng.");

            if (angelLight != null)
            {
                angelLight.intensity = 0;
            }

            if (AngleAbilityManager.Instance != null)
            {
                AngleAbilityManager.Instance.ShowAngleAbilitySelection(this);
            }
            else
            {
                Debug.LogWarning("⚠ AbilityManager chưa được khởi tạo!");
            }
        }
    }
}
