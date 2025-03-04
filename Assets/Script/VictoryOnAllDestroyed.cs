using UnityEngine;
using System.Collections;

public class VictoryOnAnyDestroyed : MonoBehaviour
{
    public GameObject[] targetObjects; // 🌟 Danh sách các GameObject cần kiểm tra
    public GameObject victoryScreen; // 🏆 Màn hình chiến thắng
    public float delayBeforeVictory = 3f; // ⏳ Độ trễ trước khi hiển thị Victory

    private bool isVictoryTriggered = false;

    private void Update()
    {
        if (!isVictoryTriggered)
        {
            foreach (GameObject obj in targetObjects)
            {
                if (obj == null) // Nếu bất kỳ object nào bị destroy
                {
                    isVictoryTriggered = true;
                    StartCoroutine(ShowVictoryScreen());
                    break;
                }
            }
        }
    }

    private IEnumerator ShowVictoryScreen()
    {
        Debug.Log("🎉 Một trong các đối tượng đã bị hủy! Đợi " + delayBeforeVictory + "s trước khi chiến thắng!");
        yield return new WaitForSeconds(delayBeforeVictory); // ⏳ Đợi vài giây

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true); // 🏆 Hiển thị Victory Screen
            Debug.Log("🏆 Chiến thắng!");
            Time.timeScale = 0f; // 🛑 Dừng game khi thắng
        }
        else
        {
            Debug.LogWarning("⚠ Chưa gán Victory Screen vào script!");
        }
    }
}
