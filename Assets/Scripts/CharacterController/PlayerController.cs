using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{    
    [Header("References")]
       
  //[SerializeField] private Animator animator = null;
        
    [SerializeField] private PlayerInput         playerInput = null;
    [SerializeField] private Transform           playerBody = null;
    [SerializeField] private CharacterController controller = null;
    [SerializeField] private HealthUI healthUI;


    [Header("Settings")]
    public int health = 300;
    public float movementSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float maxAirControlAmount = 90f;

    private Vector3 inputMovement;
    private Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    public bool isDead = false;

    private float currentMoveSpeed;
    private bool keepMomentum = false;
    private Vector3 airMomentum = Vector3.zero;
    public PlayerInput PlayerInput => playerInput;


    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0 && !keepMomentum)
        {
            velocity.y = -2f; // Small push down to keep grounded
        }
        if (isGrounded && velocity.y <= 0 && keepMomentum)
        {
            keepMomentum = false;
            airMomentum = Vector3.zero;
        }
        float usedSpeed = keepMomentum ? currentMoveSpeed : movementSpeed;
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Calculate final movement
        Vector3 inputDir = playerBody.right * inputMovement.x + playerBody.forward * inputMovement.z;
        inputDir.y = 0f;
        inputDir.Normalize();

        Vector3 move;
        if (keepMomentum)
        {
            // While airborne
            if (inputDir.magnitude > 0f)
            {
                // Adjust air momentum gradually toward input direction
                airMomentum = Vector3.RotateTowards(
                airMomentum,
                inputDir,
                maxAirControlAmount * Mathf.Deg2Rad * Time.deltaTime,
                float.MaxValue
                );
            }

            move = airMomentum * currentMoveSpeed;
        }
        else
        {
            // Grounded movement, use input directly
            move = inputDir * movementSpeed;
        }


        // Combine horizontal movement and vertical velocity
        Vector3 finalMovement = (move * usedSpeed + new Vector3(0, velocity.y, 0)) * Time.deltaTime;

        controller.Move(finalMovement);
    }
        
    public void Move(InputAction.CallbackContext ctx)
    {  
        Vector2 inputValue = ctx.ReadValue<Vector2>();
            
        inputMovement = new Vector3(inputValue.x, 0f, inputValue.y);
    }

    public void Jump(InputAction.CallbackContext ctx)
    { 
        if (!ctx.performed) { return; }
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            currentMoveSpeed = movementSpeed;
            keepMomentum = true;
        }
        //animator.SetTrigger("Jump");
    }

    public void Sprint(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
        {
            movementSpeed = 2.5f;
        }
        else 
            movementSpeed = 3.5f;
        //animator.SetTrigger("Jump");
    }
    public void TakeDamage(int damage)
    {
        if (isDead)
            return;
        health -= damage;
        healthUI.UpdateHealthDisplay();
        if (health <= 0)
            Die();
    }
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        //OnDeath?.Invoke(this);
        gameObject.SetActive(false);
    }
}

