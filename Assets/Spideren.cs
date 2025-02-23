using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SpiderEn : MonoBehaviour
{
    private enum State { Idle, Move, Alert, Attack }
    private State currentState = State.Idle;

    private GameObject player;
    private NavMeshAgent agent;
    private LineRenderer lineRenderer;

    public LayerMask layerMask;
    public GameObject enemyProjectile;
    public Transform projectileSpawn;

    public float detectionRange = 6f; // Phạm vi phát hiện người chơi
    public float moveDistance = 3f;   // Khoảng cách di chuyển khi phát hiện người chơi
    public float alertDuration = 2f;  // Thời gian hiển thị DangerMarker

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>(); // Lấy LineRenderer
        lineRenderer.enabled = false; // Ẩn đường cảnh báo ban đầu

        //// ✅ Đặt màu đỏ cho LineRenderer
        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.red;

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
        transform.LookAt(player.transform.position);

        //ShowDangerMarker();
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

    // 🛑 **Hiển thị DangerMarker**
    //private void ShowDangerMarker()
    //{
    //    Vector3 markerPosition = transform.position + Vector3.up * 0.1f;
    //    if (Physics.Raycast(markerPosition, transform.forward, out RaycastHit hit, 30f, layerMask))
    //    {
    //        if (hit.transform.CompareTag("Wall"))
    //        {
    //            GameObject markerClone = Instantiate(dangerMarker, markerPosition, transform.rotation);
    //            markerClone.GetComponent<DangerLine>().EndPosition = hit.point;
    //        }
    //    }
    //}

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
        WebProjectile webProjectile = projectile.GetComponent<WebProjectile>();
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
        lineRenderer.positionCount = 2;

        Vector3 start = projectileSpawn.position;

        // 🔹 Giữ nguyên trục Y của projectileSpawn khi tính hướng bắn
        Vector3 targetPosition = player.transform.position;
        targetPosition.y = start.y; // Giữ nguyên độ cao để bắn thẳng

        Vector3 direction = (targetPosition - start).normalized; // Hướng bắn

        RaycastHit hit;
        Vector3 endPoint;

        Debug.DrawRay(start, direction * 20f, Color.green, 0.1f); // Vẽ Raycast

        if (Physics.Raycast(start, direction, out hit, Mathf.Infinity, layerMask))
        {
            endPoint = hit.point; // Nếu gặp tường thì dừng lại
            Debug.DrawRay(start, direction * hit.distance, Color.red, 0.1f); // Raycast chạm tường
        }
        else
        {
            Vector3 extendedPoint = start + direction * 20f;
            if (Physics.Raycast(start, direction, out hit, 20f, layerMask))
            {
                endPoint = hit.point;
                Debug.DrawRay(start, direction * hit.distance, Color.blue, 0.1f);
            }
            else
            {
                endPoint = extendedPoint;
                Debug.DrawRay(start, direction * 20f, Color.yellow, 0.1f);
            }
        }

        // 🔹 Đảm bảo tia bắn không đổi trục Y
        endPoint.y = start.y;

        // Cập nhật LineRenderer
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, endPoint);
    }



    // 🎨 **Vẽ Gizmos để kiểm tra phạm vi phát hiện**
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}