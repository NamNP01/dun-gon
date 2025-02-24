using UnityEngine;

public class CoinProjectile : MonoBehaviour
{
    public int Damage;
    public float rotationSpeed = 500f;
    void Update()
    {
        // Xoay liên tục quanh trục Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
