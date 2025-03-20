using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class CharacterControllerMovement : MonoBehaviour
{
    public CharacterController characterController;
    public InputActionProperty moveInput; // Left-hand joystick movement
    public InputActionProperty sprintButton; // "A" button for sprinting

    public float walkSpeed = 2.0f;
    public float sprintSpeed = 5.0f;
    public float gravity = -9.81f;

    private Vector3 playerVelocity;

    void Update()
    {
        // Read movement input from the left-hand joystick
        Vector2 input = moveInput.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y);

        // Convert movement direction relative to the camera
        move = Camera.main.transform.TransformDirection(move);
        move.y = 0; // Prevent moving up/down

        // Check if sprint button ("A") is pressed
        bool isSprinting = sprintButton.action.ReadValue<float>() > 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Move the character using Character Controller
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (characterController.isGrounded)
        {
            playerVelocity.y = 0f;
        }
        else
        {
            playerVelocity.y += gravity * Time.deltaTime;
            characterController.Move(playerVelocity * Time.deltaTime);
        }
    }
}