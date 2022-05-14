using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour {

    [SerializeField] float playerSpeed;
    [SerializeField] float gravity;
    [SerializeField] float controllerDeadzone;
    [SerializeField] float gamepadSmoothing;
    [SerializeField] float jumpHeight;


    CharacterController controller;
    PlayerController playerController;
    PlayerInput playerInput;

    Vector3 movementDirection;
    Vector3 aimDirection;

    Vector3 playerVelocity;

    bool isGamepad;

    Transform camera;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletPoint;

    float stepOffset;

    void Awake() {
        controller = GetComponent<CharacterController>();
        stepOffset = controller.stepOffset;
        playerController = new PlayerController();
        playerInput = GetComponent<PlayerInput>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update() {
        Input();
        Movement();
        Aim();
        Melee();
        Ranged();
    }

    void Input() {
        Vector2 movementInput = playerController.Controls.Movement.ReadValue<Vector2>();
        Vector2 aimInput = playerController.Controls.Aim.ReadValue<Vector2>();
        Vector3 forward = camera.forward;
        Vector3 right = camera.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        movementDirection = forward * movementInput.y + right * movementInput.x;
        if (isGamepad) aimDirection = forward * aimInput.y + right * aimInput.x;
        else aimDirection = aimInput;
    }

    void Movement() {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0) {
            playerVelocity.y = 0f;
        }

        controller.stepOffset = isGrounded ? stepOffset : 0;

        Vector3 move = new Vector3(movementDirection.x, 0, movementDirection.z);
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (playerController.Controls.Jump.WasPressedThisFrame() && isGrounded) {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void Aim() {
        if (isGamepad) {
            if (Mathf.Abs(aimDirection.x) > controllerDeadzone || Mathf.Abs(aimDirection.y) > controllerDeadzone) {
                Vector3 playerDirection = Vector3.right * aimDirection.x + Vector3.forward * aimDirection.z;

                if (playerDirection.sqrMagnitude > 0.0f) {
                    Quaternion newRotation = Quaternion.LookRotation(playerDirection, Vector3.up);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, newRotation, gamepadSmoothing * Time.deltaTime);
                }
            }
        } else {
            Ray ray = Camera.main.ScreenPointToRay(aimDirection);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance)) {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }
    }

    void LookAt(Vector3 lookPoint) {
        Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }


    void Melee() {
        if (playerController.Controls.Melee.WasPressedThisFrame()) {

        }
    }

    void Ranged() {
        if (playerController.Controls.Ranged.WasPressedThisFrame()) {
            Instantiate(bulletPrefab, bulletPoint.position, transform.rotation);
        }
    }

    public void OnDeviceChance(PlayerInput playerInput) {
        isGamepad = playerInput.currentControlScheme.Equals("Gamepad") ? true : false;
    }

    void OnEnable() {
        playerController.Enable();
    }

    void OnDisable() {
        playerController.Disable();
    }
}
