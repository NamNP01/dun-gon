using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BigMushroomAngry : MonoBehaviour
{
    public enum EnemyState { Idle, Dash, Wait, Shoot }
    private EnemyState currentState;

    public Transform player;
    public float detectRange = 13f;
    public float dashSpeed = 10f;
    public float shootDuration = 6f;
    public int bulletsPerWave = 7;
    public float bulletSpreadAngle = 720f;
    public float attackCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float lastAttackTime;
    private NavMeshAgent agent;
    private Animator animator;
    private bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentState = EnemyState.Idle;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState(distanceToPlayer);
                break;
            case EnemyState.Dash:
                animator.SetTrigger("JumpAttack");
                break;
            case EnemyState.Wait:
            case EnemyState.Shoot:
                RotateTowardsPlayer(); // 🔄 Chỉ xoay khi ở trạng thái Wait hoặc Shoot
                break;
        }
    }
    void RotateTowardsPlayer()
    {
        if (player == null) return;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0; // Giữ trục Y không thay đổi để tránh nghiêng
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
    }

    void IdleState(float distanceToPlayer)
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);

        if (distanceToPlayer <= detectRange && Time.time - lastAttackTime >= attackCooldown)
        {
            currentState = EnemyState.Dash;
        }
    }

    void DashState()
    {
        if (isAttacking) return;
        isAttacking = true;

        Vector3 dashTarget = player.position;

        // Phát animation "JumpAttack"
        animator.SetTrigger("JumpAttack");

        // Lướt ngay lập tức
        StartCoroutine(DashToTarget(dashTarget));
    }

    IEnumerator DashToTarget(Vector3 targetPosition)
    {
        agent.isStopped = true;
        Vector3 startPosition = transform.position;
        float dashTime = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < dashTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;

        // 🛑 Đứng yên 4 giây trước khi bắn
        currentState = EnemyState.Wait;
        yield return new WaitForSeconds(4f);

        // 🎯 Chuyển sang trạng thái bắn
        StartCoroutine(ShootState());
    }

    IEnumerator ShootState()
    {
        currentState = EnemyState.Shoot;

        int shootCount = 5; // Số lần phát animation
        float shootInterval = shootDuration / shootCount; // Khoảng thời gian giữa mỗi lần

        for (int i = 0; i < shootCount; i++)
        {
            animator.SetTrigger("Shoot"); // Kích hoạt animation "Shoot"
            yield return new WaitForSeconds(shootInterval); // Đợi trước khi phát lần tiếp theo
        }

        lastAttackTime = Time.time;
        isAttacking = false;
        currentState = EnemyState.Idle;
    }


    // 📌 Gọi từ event trong animation Shoot (5 lần)
    public void ShootBullets()
    {
        if (bulletPrefab == null || firePoint == null || player == null) return;

        // ✅ Xác định hướng bắn dựa trên hướng nhìn của enemy (không cần vị trí player)
        Vector3 baseDirection = transform.forward; // Hướng của enemy thay vì vị trí player

        for (int i = 0; i < bulletsPerWave; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            CoinProjectile webProjectile = bullet.GetComponent<CoinProjectile>();
            EnemyHP enemyHP = GetComponent<EnemyHP>();

            if (webProjectile != null && enemyHP != null)
            {
                webProjectile.Damage = enemyHP.Damage;
            }
            Destroy(bullet, 5f);

            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ✅ Tính góc lệch cho từng viên đạn
                float angleStep = bulletSpreadAngle / (bulletsPerWave - 1);
                float angleOffset = -bulletSpreadAngle / 2 + i * angleStep;

                // ✅ Xoay hướng bắn theo góc lệch
                Vector3 bulletDirection = Quaternion.Euler(0, angleOffset, 0) * baseDirection;

                // ✅ Đạn bay theo hướng enemy đang nhìn
                rb.linearVelocity = bulletDirection * 10f; // Điều chỉnh tốc độ đạn
            }
        }
    }




    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
