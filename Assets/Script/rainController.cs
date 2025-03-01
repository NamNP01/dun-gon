using UnityEngine;
using UnityEngine.VFX; // Import Visual Effect Graph

public class RainController : MonoBehaviour
{
    public VisualEffect rainEffect; // Tham chiếu đến hiệu ứng VFX
    public string playerTag = "Player"; // Tag để nhận diện Player

    private void Start()
    {
        if (rainEffect != null)
        {
            rainEffect.Stop(); // Mặc định tắt hiệu ứng khi bắt đầu
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && rainEffect != null)
        {
            rainEffect.Play(); // Player vào vùng -> Hiệu ứng bắt đầu
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && rainEffect != null)
        {
            rainEffect.Stop(); // Player rời vùng -> Hiệu ứng dừng
        }
    }
}
