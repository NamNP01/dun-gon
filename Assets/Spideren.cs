using UnityEngine;
using UnityEngine.AI;

public class Spideren : MonoBehaviour
{
    public enum State { Patrol, Attack }
    private State currentState;

    private Transform player;
    public float detectionRange = 13f; // Phạm vi phát hiện Player
    public float patrolRadius = 10f; // Khoảng cách tối đa cho mỗi lần di chuyển ngẫu nhiên
    public GameObject webProjectile; // Dự án tơ
    public Transform firePoint; // Vị trí bắn
    public float fireRate = 2f; // Tốc độ bắn tơ

    private NavMeshAgent agent;
    private Vector3 patrolTarget;
    private float nextActionTime;
    private float attackCooldown = 2f; // Thời gian giữa các đợt tấn công

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentState = State.Patrol;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SetNewPatrolPoint();
    }

    void Update()
    {
        Debug.Log($"Trạng thái hiện tại: {currentState}"); // Hiển thị trạng thái lên Console

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (Time.time >= nextActionTime)
            {
                if (currentState == State.Patrol)
                {
                    currentState = State.Attack;
                    Debug.Log("Chuyển sang trạng thái ATTACK");
                    agent.isStopped = true; // Đứng yên khi tấn công
                    nextActionTime = Time.time + attackCooldown;
                    Attack();
                }
                else
                {
                    currentState = State.Patrol;
                    Debug.Log("Chuyển sang trạng thái PATROL");
                    agent.isStopped = false;
                    SetNewPatrolPoint();
                    nextActionTime = Time.time + attackCooldown;
                }
            }
        }

        if (currentState == State.Patrol)
        {
            agent.SetDestination(patrolTarget);
            if (Vector3.Distance(transform.position, patrolTarget) < 1f)
            {
                SetNewPatrolPoint();
            }
        }
    }

    void Attack()
    {
        Debug.Log("Nhện đang tấn công!");

        // Xoay nhện hướng về người chơi
        Vector3 targetPosition = player.position;
        targetPosition.y += 1.5f; // Nhắm cao hơn một chút để tránh mặt đất

        transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

        // Bắn tơ nhện theo hướng người chơi
        GameObject web = Instantiate(webProjectile, firePoint.position, Quaternion.identity);
        Rigidbody rb = web.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (targetPosition - firePoint.position).normalized;
            rb.linearVelocity = direction * 10f; // Tơ bay thẳng về phía player
        }
    }

    void SetNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
        }
        else
        {
            patrolTarget = transform.position;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange); // Phạm vi phát hiện Player
    }
}
