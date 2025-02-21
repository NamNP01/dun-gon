using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MushroomAngry : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState;

    public Transform player;
    public float detectRange = 10f;  // Phạm vi phát hiện player
    public float attackRange = 2f;   // Phạm vi để nhảy tấn công
    public float jumpDistance = 3f;  // Khoảng cách nhảy
    public float attackCooldown = 1.5f; // Thời gian chờ giữa các đợt tấn công

    private float lastAttackTime = 0f;
    private bool isAttacking = false;
    private NavMeshAgent agent;
    private Animator animator;


    public GameObject attackVFX; // Hiệu ứng khi tấn công


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState(distanceToPlayer);
                break;
            case EnemyState.Chase:
                ChaseState(distanceToPlayer);
                break;
            case EnemyState.Attack:
                AttackState(distanceToPlayer);
                break;
        }
    }

    void IdleState(float distanceToPlayer)
    {
        animator.SetBool("isWalking", false);
        agent.isStopped = true;

        if (distanceToPlayer <= detectRange)
        {
            currentState = EnemyState.Chase;
        }
    }

    void ChaseState(float distanceToPlayer)
    {
        animator.SetBool("isWalking", true);
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer > detectRange)
        {
            currentState = EnemyState.Idle;
        }
    }

    void AttackState(float distanceToPlayer)
    {
        if (isAttacking) return; // Nếu đang tấn công thì không làm gì cả

        agent.isStopped = true;
        animator.SetBool("isWalking", false);

        if (Time.time - lastAttackTime > attackCooldown)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetTrigger("JumpAttack"); // Chạy animation tấn công

        // Animation sẽ gọi OnJumpAttack() qua Animation Event
    }

    // 🛠 Hàm này sẽ được gọi từ Animation Event khi nhân vật nhảy
    public void OnJumpAttack()
    {
        Vector3 jumpTarget = transform.position + transform.forward * jumpDistance;
        StartCoroutine(JumpToTarget(jumpTarget));

        // Trì hoãn tạo hiệu ứng tấn công sau 0.5 giây
        Invoke(nameof(SpawnAttackVFX), 0.3f);
    }

    // 🛠 Hàm tạo hiệu ứng VFX tại chỗ
    void SpawnAttackVFX()
    {
        if (attackVFX != null)
        {
            GameObject vfx = Instantiate(attackVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 1f);
        }
    }



    IEnumerator JumpToTarget(Vector3 targetPosition)
    {
        float jumpTime = 0.3f; // Thời gian lướt (có thể điều chỉnh)
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;

        while (elapsedTime < jumpTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / jumpTime);
            elapsedTime += Time.deltaTime;
            yield return null; // Đợi frame tiếp theo
        }

        transform.position = targetPosition; // Đảm bảo nhân vật đúng vị trí sau khi lướt xong
    }


    public void OnAttackEnd()
    {
        Debug.Log("OnAttackEnd được gọi!");
        isAttacking = false;

        // Mở lại NavMeshAgent để có thể di chuyển
        agent.isStopped = false;

        // Reset trạng thái animation
        animator.SetTrigger("AttackEnd");

        // Kiểm tra khoảng cách để quyết định trạng thái tiếp theo
        float newDistanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (newDistanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
        }
        else if (newDistanceToPlayer <= detectRange)
        {
            currentState = EnemyState.Chase;
            animator.SetBool("isWalking", true); // Chạy animation di chuyển
            agent.SetDestination(player.position); // Tiếp tục đuổi theo player
        }
        else
        {
            currentState = EnemyState.Idle;
            animator.SetBool("isWalking", false);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
