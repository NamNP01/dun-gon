using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public VariableJoystick joystick;
    public float movementSpeed;
    public Canvas inputCanvas;
    public bool isJoystick;
    public Animator animator;
    public StageManager stageManager;
    public CameraController cameraController;



    private Rigidbody rb; // Thay CharacterController bằng Rigidbody

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        EnableJoystickInput();
        stageManager = Object.FindFirstObjectByType<StageManager>();

        if (cameraController == null)
        {
            cameraController = FindAnyObjectByType<CameraController>();
            if (cameraController == null)
            {
                Debug.LogError("Không tìm thấy CameraController trong Scene!");
            }
        }
    }

    public void EnableJoystickInput()
    {
        isJoystick = true;
        inputCanvas.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isJoystick)
        {
            var movementDirection = new Vector3(joystick.Direction.x, 0.0f, joystick.Direction.y);
            bool isMoving = movementDirection.magnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);

            if (isMoving)
            {

                // Quay nhân vật theo hướng di chuyển
                Quaternion toRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
            }
        }
    }


    private void FixedUpdate()
    {
        if (isJoystick)
        {
            var movementDirection = new Vector3(joystick.Direction.x, 0.0f, joystick.Direction.y);
            rb.linearVelocity = movementDirection * movementSpeed; // Cập nhật vận tốc

            rb.MovePosition(rb.position + movementDirection * movementSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Đảm bảo vận tốc bằng 0 khi không di chuyển
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NextStage"))
        {
            // Kiểm tra xem còn quái trong phòng không
            if (PlayerTargeting.Instance.MonsterList.Count > 0)
            {
                Debug.Log("Còn quái vật, không thể qua màn!");
                return;
            }

            // Nếu không còn quái vật, cho phép qua màn
            if (stageManager != null)
            {
                Debug.Log("Chuyển sang màn tiếp theo!");
                stageManager.NextStage();
            }

            // Di chuyển camera
            cameraController.MoveToNearestStage();
        }
    }

}
