using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FreeBurrow : MonoBehaviour
{
    public enum State { Idle, Move, Duplicate }

    public State currentState;

    public float detectRange = 13f;  // Phạm vi phát hiện player
    public GameObject duplicatePrefab; // Prefab nhân đôi
    public Transform player; // Player để enemy di chuyển đến
    private Animator animator;
    private NavMeshAgent agent;
    public ParticleSystem duplicateVFX; // Hiệu ứng nhân đôi
    public Transform PointDuplicate;

    private EnemyHP enemyHP; // Lấy máu từ EnemyHP
    private bool hasDuplicated = false; // Tránh nhân đôi nhiều lần

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemyHP = GetComponent<EnemyHP>();

        player = GameObject.FindGameObjectWithTag("Player")?.transform; // Tìm player

        currentState = State.Idle;
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

        if (currentState == State.Move)
        {
            MoveTowardsPlayer();
        }

        // Kiểm tra điều kiện nhân đôi: máu < 60% nhưng > 200 và chưa nhân đôi
        if (enemyHP != null && enemyHP.GetCurrentHP() < enemyHP.maxHP * 0.6f && enemyHP.GetCurrentHP() > 199 && !hasDuplicated)
        {
            duplicateVFX.Play();
            DuplicateSelf();
            hasDuplicated = true; // Đánh dấu đã nhân đôi để không lặp lại
        }
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    void DuplicateSelf()
    {
        GameObject duplicate = Instantiate(duplicatePrefab, PointDuplicate.position, Quaternion.identity);

        if (transform.parent != null && transform.parent.parent != null)
        {
            duplicate.transform.SetParent(transform.parent.parent);
        }

        StartCoroutine(SetupDuplicate(duplicate));
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

    // 🔹 Vẽ phạm vi phát hiện bằng Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
