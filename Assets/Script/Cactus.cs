using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Cactus : MonoBehaviour
{
    private enum State { Idle, Move, Alert, Attack }
    private State currentState = State.Idle;

    private GameObject player;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;
    private Animator animator;

    public LayerMask layerMask;
    public GameObject enemyProjectile;
    public Transform projectileSpawn;

    public float detectionRange = 13f; // Phạm vi phát hiện người chơi
    public float moveDistance = 3f;   // Khoảng cách di chuyển khi phát hiện người chơi
    public float alertDuration = 2f;  // Thời gian hiển thị DangerMarker

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>(); // Lấy LineRenderer
        animator = GetComponent<Animator>();
        lineRenderer.enabled = false; // Ẩn đường cảnh báo ban đầu


        StartCoroutine(StateMachine());
    }

    private IEnumerator StateMachine()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Idle:
                    yield return StartCoroutine(IdleState());
                    break;
                case State.Move:
                    yield return StartCoroutine(MoveState());
                    break;
                case State.Alert:
                    yield return StartCoroutine(AlertState());
                    break;
                case State.Attack:
                    yield return StartCoroutine(AttackState());
                    break;
            }
        }
    }

    // 💤 **Trạng thái Idle - Đứng yên nếu không thấy người chơi**
    private IEnumerator IdleState()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        while (!IsPlayerInDetectionRange())
        {
            yield return null;
        }
        currentState = State.Move;
    }

    // 🚶 **Trạng thái Move - Di chuyển đến một điểm ngẫu nhiên trên NavMesh**
    private IEnumerator MoveState()
    {
        agent.isStopped = false;
        animator.SetBool("isWalking", true);
        Vector3 targetPoint = GetRandomNavMeshPoint();
        agent.SetDestination(targetPoint);

        while (agent.pathPending || agent.remainingDistance > 0.1f)
        {
            yield return null;
        }

        currentState = State.Alert;
    }

    private IEnumerator AlertState()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        transform.LookAt(player.transform.position);

        lineRenderer.enabled = true; // Hiển thị đường cảnh báo

        float elapsedTime = 0f;
        while (elapsedTime < alertDuration)
        {
            ShowWarningLine(); // Cập nhật đường liên tục để theo sát Player
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        lineRenderer.enabled = false; // Ẩn đường cảnh báo trước khi bắn
        currentState = State.Attack;
    }




    // 🎯 **Trạng thái Attack - Bắn đạn từ projectileSpawn**
    private IEnumerator AttackState()
    {
        animator.SetTrigger("Attack");
        ShootProjectile();
        yield return new WaitForSeconds(1f); // Chờ 1s trước khi quay lại Idle
        currentState = State.Idle;
    }

    // 📍 **Lấy một điểm ngẫu nhiên hợp lệ trên NavMesh**
    private Vector3 GetRandomNavMeshPoint()
    {
        for (int i = 0; i < 10; i++) // Thử tối đa 10 lần để tìm điểm hợp lệ
        {
            Vector3 randomDirection = Random.insideUnitSphere * moveDistance;
            randomDirection += transform.position;
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, moveDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return transform.position; // Nếu không tìm thấy điểm hợp lệ, giữ nguyên vị trí cũ
    }
    // 🔫 **Bắn đạn từ projectileSpawn về phía Player**
    private void ShootProjectile()
    {
        if (player == null || enemyProjectile == null) return;

        Vector3 targetPosition = player.transform.position;
        targetPosition.y = projectileSpawn.position.y; // 🔹 Giữ nguyên độ cao của viên đạn

        Vector3 direction = (targetPosition - projectileSpawn.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        GameObject projectile = Instantiate(enemyProjectile, projectileSpawn.position, rotation);

        // 🔥 Gán Dame từ EnemyHP vào Damage của WebProjectile
        CactusProjectile webProjectile = projectile.GetComponent<CactusProjectile>();
        EnemyHP enemyHP = GetComponent<EnemyHP>(); // Lấy EnemyHP từ SpiderEn

        if (webProjectile != null && enemyHP != null)
        {
            webProjectile.Damage = enemyHP.Damage; // ✅ Gán Damage = Dame của EnemyHP
        }
    }



    // 🕵️ **Kiểm tra xem người chơi có trong phạm vi phát hiện không**
    private bool IsPlayerInDetectionRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) <= detectionRange;
    }

    private void ShowWarningLine()
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 4; // Tối đa 3 lần phản xạ (4 điểm)

        Vector3 start = projectileSpawn.position;
        Vector3 direction = (player.transform.position - start).normalized;
        direction.y = 0; // Giữ nguyên độ cao

        Vector3[] points = new Vector3[4];
        points[0] = start;
        int reflections = 0;
        float maxDistance = 20f;

        for (int i = 1; i < 4; i++) // Tối đa 3 lần phản xạ
        {
            if (Physics.Raycast(start, direction, out RaycastHit hit, maxDistance, layerMask))
            {
                Vector3 reflectPoint = hit.point;
                reflectPoint.y = projectileSpawn.position.y; // Giữ nguyên độ cao

                points[i] = reflectPoint;
                direction = Vector3.Reflect(direction, hit.normal); // Phản xạ tia
                start = reflectPoint;
                reflections++;
            }
            else
            {
                Vector3 endPoint = start + direction * maxDistance;
                endPoint.y = projectileSpawn.position.y; // Giữ nguyên độ cao
                points[i] = endPoint;
                break;
            }
        }

        lineRenderer.positionCount = reflections + 1;
        lineRenderer.SetPositions(points);
    }




    // 🎨 **Vẽ Gizmos để kiểm tra phạm vi phát hiện**
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}