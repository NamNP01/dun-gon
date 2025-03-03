using System.Collections;
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
    private int currentBoltIndex = 0; // Vị trí hiện tại trong danh sách Prefab
    public List<GameObject> boltPrefabs = new List<GameObject>(); // Danh sách Prefab đạn có thể đổi
    public GameObject diagonalArrowPrefab; // Prefab của mũi tên phụ (được đặt trong Inspector)
    public List<GameObject> diagonalArrowPrefabs = new List<GameObject>(); // Danh sách các Prefab mũi tên chéo
    private int currentDiagonalArrowIndex = 0; // Vị trí hiện tại
    public GameObject sideArrowPrefab;
    public List<GameObject> sideArrowPrefabs = new List<GameObject>();
    private int currentSideArrowIndex = 0;
    public GameObject rearArrowPrefab;
    public List<GameObject> rearArrowPrefabs = new List<GameObject>();
    private int currentRearArrowIndex = 0;





    private int TargetIndex = -1;
    private float TargetDist = 100f;
    private Rigidbody rb;

    public float stopThreshold = 0.1f; // Ngưỡng tốc độ để coi là dừng
    //public float attackSpeed = 1f; // Tốc độ bắn (1 phát mỗi giây)
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
            //attackSpeed = playerData.SpeedAtk; // 🛑 Lấy attackSpeed từ PlayerData
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

            Vector3 direction = (MonsterList[i].transform.position - transform.position);
            direction.y = 0; // Giữ trục Y cố định
            direction = direction.normalized; // Chuẩn hóa vector sau khi đặt y = 0

            float currentDist = Vector3.Distance(transform.position, MonsterList[i].transform.position);
            RaycastHit hit;

            // Kiểm tra Raycast
            if (Physics.Raycast(transform.position + Vector3.up * 1f, direction, out hit, 20f, layerMask))
            {
                if (hit.transform.CompareTag("Monster"))
                {
                    Debug.DrawRay(transform.position + Vector3.up * 1f, direction * 20f, Color.red); // Tất cả mục tiêu

                    if (currentDist < TargetDist)
                    {
                        TargetIndex = i;
                        TargetDist = currentDist;
                        closestValidTarget = MonsterList[i];
                    }
                }
                else
                {
                    Debug.DrawRay(transform.position + Vector3.up * 1f, direction * 20f, Color.yellow); // Bị chặn bởi vật cản
                }
            }
        }

        // Nếu có mục tiêu gần nhất mà không bị che, vẽ màu xanh lá
        if (closestValidTarget != null)
        {
            Vector3 closestDirection = (closestValidTarget.transform.position - transform.position);
            closestDirection.y = 0; // Giữ trục Y cố định
            closestDirection = closestDirection.normalized;
            Debug.DrawRay(transform.position + Vector3.up * 1f, closestDirection * 20f, Color.green);
        }

        getATarget = (TargetIndex != -1);
    }


    void AttackTarget()
    {
        if (TargetIndex == -1) return;
        if (Time.time - lastAttackTime < 1f / playerData.SpeedAtk) return;
        if (rb.linearVelocity.magnitude > stopThreshold) return;
        if (animator.GetBool("isMoving")) return; // Nếu đang di chuyển, không tấn công

        lastAttackTime = Time.time;

        // Cập nhật AttackSpeed theo attackRate
        animator.SetFloat("AttackSpeed", playerData.SpeedAtk / 2);

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
    public void ShootArrowFromAnimation()
    {
        ShootArrow(false); // Gọi hàm chính và đánh dấu đây không phải Multishot
    }

    public void ShootArrow(bool isFromMultishot = false)
    {
        if (TargetIndex == -1) return;

        GameObject target = MonsterList[TargetIndex];
        if (target == null) return;



        // Bắn mũi tên chính (có script `Arrow`)
        FireArrow(PlayerBolt, 0,  target);
        // Nếu có Multishot và đây không phải là lần bắn từ Multishot, bắn thêm một lần nữa
        if (playerData.hasMultishot && !isFromMultishot)
        {
            StartCoroutine(MultishotDelay()); // Thay vì gọi ngay, ta sẽ delay rồi mới bắn
        }
        // Nếu có Diagonal Arrows, bắn thêm 2 mũi tên phụ
        if (playerData.hasDiagonalArrows && diagonalArrowPrefab != null)
        {
            FireArrow(diagonalArrowPrefab, 45,  target);
            FireArrow(diagonalArrowPrefab, -45,  target);
        }
        if (playerData.hasSideArrows && sideArrowPrefab != null)
        {
            FireArrow(sideArrowPrefab, 90, target);
            FireArrow(sideArrowPrefab, -90, target);
        }        
        if (playerData.hasSideArrows && sideArrowPrefab != null)
        {
            FireArrow(sideArrowPrefab, 90, target);
            FireArrow(sideArrowPrefab, -90, target);
        }
        if (playerData.hasRearArrow && rearArrowPrefab != null)
        {
            FireArrow(rearArrowPrefab, 180, target);
        }

    }

    // Hàm bắn mũi tên (chính & phụ)
    private void FireArrow(GameObject arrowPrefab, float angleOffset, GameObject target)
    {
        if (arrowPrefab == null)
        {
            Debug.LogWarning("⚠ Prefab mũi tên không hợp lệ!");
            return;
        }

        GameObject arrow = Instantiate(arrowPrefab, AttackPoint.position, Quaternion.identity);
        Vector3 direction = (target.transform.position - AttackPoint.position).normalized;
        direction = Quaternion.Euler(0, angleOffset, 0) * direction; // Xoay góc 45 độ

        if (arrow.GetComponent<Arrow>() != null)
        {
            // Nếu là mũi tên chính, dùng script Arrow
            arrow.GetComponent<Arrow>().SetTarget(target);
        }
        else if (arrow.GetComponent<DiagonalArrow>() != null)
        {
            // Nếu là mũi tên phụ, dùng script mới
            arrow.GetComponent<DiagonalArrow>().SetDirection(direction);
        }
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
    public void ChangeBoltPrefab()
    {
        if (boltPrefabs.Count == 0)
        {
            Debug.LogWarning("⚠ Danh sách Prefab đạn rỗng! Không thể đổi.");
            return;
        }

        currentBoltIndex = (currentBoltIndex + 1) % boltPrefabs.Count; // Đổi sang Prefab tiếp theo
        PlayerBolt = boltPrefabs[currentBoltIndex]; // Cập nhật Prefab đạn

        Debug.Log($"🔄 Đạn đã được thay đổi! Hiện tại: {PlayerBolt.name}");
    }

    public void ChangeDiagonalArrowPrefab()
    {
        if (diagonalArrowPrefabs.Count == 0)
        {
            Debug.LogWarning("⚠ Không có Prefab nào cho Diagonal Arrows!");
            return;
        }

        currentDiagonalArrowIndex = (currentDiagonalArrowIndex + 1) % diagonalArrowPrefabs.Count;
        diagonalArrowPrefab = diagonalArrowPrefabs[currentDiagonalArrowIndex];

        Debug.Log($"🔄 Mũi tên chéo đã đổi sang: {diagonalArrowPrefab.name}");
    }
    public void ChangeSideArrowPrefab()
    {
        if (sideArrowPrefabs.Count == 0)
        {
            Debug.LogWarning("⚠ Không có Prefab nào cho Side Arrows!");
            return;
        }

        currentSideArrowIndex = (currentSideArrowIndex + 1) % sideArrowPrefabs.Count;
        sideArrowPrefab = sideArrowPrefabs[currentSideArrowIndex];

        Debug.Log($"🔄 Mũi tên ngang đã đổi sang: {sideArrowPrefab.name}");
    }
    public void ChangeRearArrowPrefab()
    {
        if (rearArrowPrefabs.Count == 0)
        {
            Debug.LogWarning("⚠ Không có Prefab nào cho Rear Arrows!");
            return;
        }

        currentRearArrowIndex = (currentRearArrowIndex + 1) % rearArrowPrefabs.Count;
        rearArrowPrefab = rearArrowPrefabs[currentRearArrowIndex];

        Debug.Log($"🔄 Mũi tên phía sau đã đổi sang: {rearArrowPrefab.name}");
    }
    // 🏹 Bắn thêm một lần nữa sau 0.2 giây (chỉ bắn lại một lần)
    private IEnumerator MultishotDelay()
    {
        yield return new WaitForSeconds(0.2f);
        ShootArrow(true); // Bắn lần thứ hai nhưng không gọi lại Multishot
    }
}