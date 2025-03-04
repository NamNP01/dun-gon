using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BigFreeBurrow : MonoBehaviour
{
    public enum State { Idle, Move, Attack, Duplicate, Shoot }

    public State currentState;

    public float moveDistance = 3f;
    public GameObject duplicatePrefab;
    public Transform duplicatePoint;
    public GameObject projectilePrefab;
    public int projectileCount = 12;
    public float projectileSpeed = 5f;
    public float attackCooldown = 2f;
    public Transform attackPoint;
    public int hpThreshold = 2000; // Mất 2000 HP thì tạo bản sao
    public Transform PointDuplicate;

    public Transform player;  // Gán trong Inspector
    public float detectRange = 10f;


    private Animator animator;
    private NavMeshAgent agent;
    private EnemyHP enemyHP;
    //private bool isAttacking = false;
    private Vector3 targetPosition;
    private bool isMoving = false;
    public int lastHP; // Lưu HP lần cuối

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyHP = GetComponent<EnemyHP>();


        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Tìm player

        currentState = State.Idle;

        if (enemyHP != null)
        {
            lastHP = 20000; // Khởi tạo HP ban đầu
        }
        currentState = State.Idle;

        MoveToRandomPosition(); // Bắt đầu di chuyển ngay khi khởi tạo
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        {
            currentState = State.Move;
        }
        else
        {
            currentState = State.Idle;
            agent.SetDestination(transform.position); // Dừng lại
        }

        // Kiểm tra nếu đã đến nơi thì tấn công
        if (isMoving && !agent.pathPending && agent.remainingDistance <= 0.5f && distanceToPlayer <= detectRange)
        {
            isMoving = false;
            StartCoroutine(ShootProjectiles());
        }

        // Nếu mất 2000 HP so với lần cuối lưu, tạo bản sao
    if (enemyHP != null && (lastHP - enemyHP.currentHP) >= hpThreshold)
        {
            Debug.LogWarning("!");
            lastHP = enemyHP.currentHP;
            DuplicateSelf();
        }
    }

    void MoveToRandomPosition()
    {
        // Chọn hướng ngẫu nhiên (trước, sau, trái, phải)
        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // Tính toán vị trí đích cách enemy 3f
        targetPosition = transform.position + randomDirection * moveDistance;

        // Kiểm tra nếu điểm đích hợp lệ trên NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, moveDistance, NavMesh.AllAreas))
        {
            targetPosition = hit.position;
            agent.SetDestination(targetPosition);
            isMoving = true;
            currentState = State.Move;
        }
    }

    IEnumerator ShootProjectiles()
    {
        currentState = State.Shoot;
        //isAttacking = true;
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = (360f / projectileCount) * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            CoinProjectile webProjectile = projectile.GetComponent<CoinProjectile>();
            EnemyHP enemyHP = GetComponent<EnemyHP>(); // Lấy EnemyHP từ SpiderEn

            if (webProjectile != null && enemyHP != null)
            {
                webProjectile.Damage = enemyHP.Damage; // ✅ Gán Damage = Dame của EnemyHP
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * projectileSpeed;
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        //isAttacking = false;

        // Sau khi tấn công xong, di chuyển tiếp
        MoveToRandomPosition();
    }

    void DuplicateSelf()
    {
        Debug.Log("!");
        GameObject duplicate = Instantiate(duplicatePrefab, PointDuplicate.position, Quaternion.identity);

        if (transform.parent != null && transform.parent.parent != null)
        {
            duplicate.transform.SetParent(transform.parent.parent);
        }

        //StartCoroutine(SetupDuplicate(duplicate));
    }

    IEnumerator SetupDuplicate(GameObject duplicate)
    {
        yield return new WaitForEndOfFrame();

        EnemyHP duplicateHP = duplicate.GetComponentInChildren<EnemyHP>();

        if (duplicateHP != null && enemyHP != null)
        {
            int currentHP = enemyHP.currentHP;
            duplicateHP.maxHP = currentHP;
            duplicateHP.currentHP = currentHP;

            if (duplicateHP.hpBar != null)
            {
                duplicateHP.hpBar.SetMaxHP(currentHP);
                duplicateHP.hpBar.TakeDamage(0);
            }

            Debug.Log($"✅ Enemy nhân bản được tạo ra với maxHP = {duplicateHP.maxHP}, currentHP = {duplicateHP.currentHP}");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy EnemyHP trên enemy nhân bản hoặc enemy gốc!");
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
