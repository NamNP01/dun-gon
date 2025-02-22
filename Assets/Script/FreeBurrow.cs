using UnityEngine;
using UnityEngine.AI;

public class FreeBurrow : MonoBehaviour
{
    public enum State { Move, Duplicate }

    public State currentState;

    public float detectRange = 13f;  // Phạm vi phát hiện player
    public float duplicateCooldown = 5f; // Thời gian chờ để nhân đôi
    private float duplicateTimer;

    public GameObject duplicatePrefab; // Prefab nhân đôi
    public Transform player; // Player để enemy di chuyển đến
    private Animator animator;
    private NavMeshAgent agent;
    public ParticleSystem duplicateVFX; // Hiệu ứng nhân đôi

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        currentState = State.Move;
        duplicateTimer = duplicateCooldown;
    }

    void Update()
    {
        duplicateTimer -= Time.deltaTime;

        if (duplicateTimer <= 0)
        {
            PlayDuplicateEffect();
            duplicateTimer = duplicateCooldown; // Reset thời gian nhân đôi
        }

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void PlayDuplicateEffect()
    {
        animator.SetTrigger("Duplicate"); // Chạy animation nhân đôi

        if (duplicateVFX != null)
        {
            duplicateVFX.Play(); // Chạy hiệu ứng
        }
    }

    // Gọi từ Animation Event để tạo bản sao
    public void CreateDuplicate()
    {
        Instantiate(duplicatePrefab, transform.position + Vector3.right * 1f, Quaternion.identity);
    }
}
