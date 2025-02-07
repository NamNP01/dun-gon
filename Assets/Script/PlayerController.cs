using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public VariableJoystick joystick;
    public CharacterController controller;
    public float movementSpeed;
    public Canvas inputCanvas;
    public bool isJoystick;
    public Animator animator;

    private void Start()
    {
        EnableJoystickInput();
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
            controller.SimpleMove(movementDirection * movementSpeed);

            // Kiểm tra nhân vật có di chuyển không
            bool isMoving = movementDirection.magnitude > 0.1f;
            animator.SetBool("isMoving", isMoving);

            // Xoay nhân vật theo hướng di chuyển
            if (isMoving)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * 10f);
            }
        }
    }
}
