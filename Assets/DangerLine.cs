using UnityEngine;

public class DangerLine : MonoBehaviour
{
    public Vector3 EndPosition;
    private TrailRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<TrailRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        // Cài đặt LineRenderer để hiển thị đường cảnh báo
        //lineRenderer.SetPosition(0, transform.position); // Điểm bắt đầu
        //lineRenderer.SetPosition(1, EndPosition); // Điểm kết thúc
        //lineRenderer.startWidth = 0.1f;
        //lineRenderer.endWidth = 0.1f;
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1, 0, 0, 0.7f);
        lineRenderer.endColor = new Color(1, 0, 0, 0.7f);

        // Hủy sau 2 giây
        Destroy(gameObject, 3f);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, EndPosition, Time.deltaTime * 3.5f);
    }

}
