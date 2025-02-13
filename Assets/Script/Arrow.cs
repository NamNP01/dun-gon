using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f; // Tốc độ bay của mũi tên
    private GameObject target;

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Tính hướng di chuyển đến mục tiêu
        Vector3 direction = target.transform.position - transform.position;
        if (direction.sqrMagnitude > 0.001f) // Kiểm tra xem vector có khác (0,0,0) không
        {
            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-90, 0, 0);
        }

        // Di chuyển về phía mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Mũi tên chạm vào: " + other.gameObject.name); // In ra tên đối tượng va chạm

        if (target != null && other.gameObject == target)
        {
            //Debug.Log("Mũi tên trúng mục tiêu!");
            Destroy(gameObject);
        }
    }

}
