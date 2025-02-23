using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    public static PlayerTargeting Instance { get; private set; }

    public bool getATarget = false;
    public LayerMask layerMask;
    public List<GameObject> MonsterList = new List<GameObject>();

    public GameObject PlayerBolt;
    public Transform AttackPoint;

    private int TargetIndex = -1;
    private float TargetDist = 100f;
    private Rigidbody rb;

    public float stopThreshold = 0.1f; // Ngưỡng tốc độ để coi là dừng
    public float attackSpeed = 1f; // Tốc độ bắn (1 phát mỗi giây)
    private float lastAttackTime = 0f; // Thời gian lần bắn trước

    public PlayerData playerData;

    public Animator animator;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (playerData != null)
        {
            attackSpeed = playerData.SpeedAtk; // 🛑 Lấy attackSpeed từ PlayerData
        }
        else
        {
            Debug.LogError("PlayerData chưa được gán!");
        }
    }

    void Update()
    {
        FindTarget();
        AttackTarget();

        if (!animator.GetBool("isMoving")) // Khi không di chuyển
        {
            RotateTowardsTarget();
        }
    }


    void FindTarget()
    {
        if (MonsterList.Count == 0)
        {
            TargetIndex = -1;
            return;
        }

        TargetDist = 100f;
        TargetIndex = -1;
        GameObject closestValidTarget = null;

        for (int i = 0; i < MonsterList.Count; i++)
        {
            if (MonsterList[i] == null) continue;

            Vector3 direction = (MonsterList[i].transform.position - transform.position).normalized;
            float currentDist = Vector3.Distance(transform.position, MonsterList[i].transform.position);
            RaycastHit hit;

            // Kiểm tra Raycast
            if (Physics.Raycast(transform.position, direction, out hit, 20f, layerMask))
            {
                if (hit.transform.CompareTag("Monster"))
                {
                    Debug.DrawRay(transform.position, direction * 20f, Color.red); // Tất cả mục tiêu

                    if (currentDist < TargetDist)
                    {
                        TargetIndex = i;
                        TargetDist = currentDist;
                        closestValidTarget = MonsterList[i];
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position, direction * 20f, Color.yellow); // Bị chặn bởi vật cản
                }
            }
        }

        // Nếu có mục tiêu gần nhất mà không bị che, vẽ màu xanh lá
        if (closestValidTarget != null)
        {
            Vector3 closestDirection = (closestValidTarget.transform.position - transform.position).normalized;
            Debug.DrawRay(transform.position, closestDirection * 20f, Color.green);
        }

        getATarget = (TargetIndex != -1);
    }


    void AttackTarget()
    {
        if (TargetIndex == -1) return;
        if (Time.time - lastAttackTime < 1f / attackSpeed) return;
        if (rb.linearVelocity.magnitude > stopThreshold) return;
        if (animator.GetBool("isMoving")) return; // Nếu đang di chuyển, không tấn công

        lastAttackTime = Time.time;

        // Cập nhật AttackSpeed theo attackRate
        animator.SetFloat("AttackSpeed", attackSpeed/2);

        // Xoay nhân vật về phía mục tiêu trước khi bắn
        GameObject target = MonsterList[TargetIndex];
        if (target != null)
        {
            Vector3 targetPosition = target.transform.position;
            targetPosition.y = transform.position.y;
            transform.LookAt(targetPosition);
        }

        // Kích hoạt animation tấn công
        animator.SetTrigger("Attack");
    }


    public void ShootArrow()
    {
        if (TargetIndex == -1) return;

        GameObject target = MonsterList[TargetIndex];
        if (target == null) return;

        // Tạo mũi tên và hướng nó về mục tiêu
        GameObject arrow = Instantiate(PlayerBolt, AttackPoint.position, Quaternion.identity);

        //// Xoay mũi tên theo hướng từ nhân vật đến mục tiêu
        //Vector3 direction = (target.transform.position - AttackPoint.position).normalized;
        //arrow.transform.forward = direction;

        // Gửi thông tin mục tiêu cho mũi tên
        arrow.GetComponent<Arrow>().SetTarget(target);
    }
    void RotateTowardsTarget()
    {
        if (TargetIndex == -1) return; // Không có mục tiêu nào

        GameObject target = MonsterList[TargetIndex];
        if (target == null) return;

        Vector3 targetPosition = target.transform.position;
        targetPosition.y = transform.position.y; // Giữ nguyên chiều cao

        // Quay từ từ về phía mục tiêu
        Quaternion lookRotation = Quaternion.LookRotation(targetPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

}
