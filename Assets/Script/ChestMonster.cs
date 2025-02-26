using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ChestMonsterFSM : MonoBehaviour
{
    private enum State { Idle, Attack }
    private State currentState = State.Idle;
    private Animator animator;


    public float detectRange = 5f;
    public float attackInterval = 6f;
    public float randomJumpDistance = 3f;
    public GameObject attackPrefab;

    private Transform player;
    private NavMeshAgent agent;
    private bool isAttacking = false;

    public ParticleSystem landingEffect; // Hiệu ứng khi đáp đất


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>(); // Lấy Animator
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Tìm player theo tag
        if (player == null)
        {
            Debug.LogError("Player not found! Ensure the player has the 'Player' tag.");
        }

        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (currentState == State.Idle && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= detectRange && !isAttacking)
            {
                StartCoroutine(AttackState());
            }
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (!isAttacking && player != null)
            {
                float distance = (player.position - transform.position).sqrMagnitude;
                if (distance <= detectRange * detectRange) // Kiểm tra lại khoảng cách
                {
                    StartCoroutine(AttackState());
                }
            }
        }
    }


    IEnumerator AttackState()
    {
        isAttacking = true;
        currentState = State.Attack;

        animator.SetBool("isAttacking", true); // Chuyển sang animation Attack

        yield return new WaitForSeconds(1f); // Chờ trước khi nhảy

        Vector3 jumpTarget = CalculateJumpTarget();

        if (NavMesh.SamplePosition(jumpTarget, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            jumpTarget = hit.position;
        }

        if (agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        yield return JumpToPosition(jumpTarget, 3f, 0.8f);

        SpawnAttacks(jumpTarget);

        yield return new WaitForSeconds(2f); // Chờ sau khi tấn công

        agent.enabled = true;
        if (agent.enabled)
        {
            agent.isStopped = false;
        }

        isAttacking = false;
        currentState = State.Idle;

        animator.SetBool("isAttacking", false); // Quay lại Idle

        yield return new WaitForSeconds(attackInterval);
    }
    Vector3 CalculateJumpTarget()
    {
        Vector3 playerPos = player.position;
        Vector3 currentPos = transform.position;

        float maxOffset = randomJumpDistance;
        float minJumpDistance = 1.5f; // Khoảng cách tối thiểu để không nhảy trúng player
        Vector3 jumpTarget = currentPos;

        // Xác định khoảng cách tối đa có thể nhảy trên trục X và Z
        float deltaX = Mathf.Abs(playerPos.x - currentPos.x);
        float deltaZ = Mathf.Abs(playerPos.z - currentPos.z);

        if (deltaX < deltaZ)
        {
            // Player gần theo trục X hơn -> Nhảy theo Z
            float jumpDistance = Mathf.Min(deltaZ, maxOffset);
            jumpTarget += new Vector3(0, 0, (playerPos.z > currentPos.z) ? jumpDistance : -jumpDistance);
        }
        else
        {
            // Player gần theo trục Z hơn -> Nhảy theo X
            float jumpDistance = Mathf.Min(deltaX, maxOffset);
            jumpTarget += new Vector3((playerPos.x > currentPos.x) ? jumpDistance : -jumpDistance, 0, 0);
        }

        // Kiểm tra nếu vị trí nhảy quá gần player, đẩy ra xa hơn
        if (Vector3.Distance(jumpTarget, playerPos) < minJumpDistance)
        {
            Vector3 direction = (jumpTarget - playerPos).normalized;
            jumpTarget = playerPos + direction * minJumpDistance;
        }

        return jumpTarget;
    }




    IEnumerator JumpToPosition(Vector3 target, float jumpHeight, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float height = Mathf.Sin(t * Mathf.PI) * jumpHeight; // Quỹ đạo nhảy parabol

            transform.position = Vector3.Lerp(startPos, target, t) + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = target; // Đáp đất chính xác

        // 🔹 Play Particle System nếu có
        if (landingEffect != null)
        {
            landingEffect.transform.position = target; // Đặt vị trí hiệu ứng ngay tại chỗ rơi
            landingEffect.Play(); // Kích hoạt hiệu ứng
        }
    }



    void SpawnAttacks(Vector3 position)
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

        foreach (Vector3 dir in directions)
        {
            GameObject projectile = Instantiate(attackPrefab, position, Quaternion.identity);
            // 🔥 Gán Dame từ EnemyHP vào Damage của WebProjectile
            CoinProjectile webProjectile = projectile.GetComponent<CoinProjectile>();
            EnemyHP enemyHP = GetComponent<EnemyHP>(); // Lấy EnemyHP từ SpiderEn

            if (webProjectile != null && enemyHP != null)
            {
                webProjectile.Damage = enemyHP.Damage; // ✅ Gán Damage = Dame của EnemyHP
            }
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            if (rb != null)
            {
                float projectileSpeed = 5f; // Tốc độ bay của đạn
                rb.linearVelocity = dir * projectileSpeed;
            }
        }
    }


    IEnumerator MoveToPosition(Vector3 from, Vector3 to, float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    // 🔹 Vẽ phạm vi detectRange trên Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
