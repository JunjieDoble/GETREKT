using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Salto")]
    public float jumpForce = 7f;
    public float groundDistance = 0.2f; // Qué tan cerca del suelo debe estar

    private Rigidbody rb;
    private Vector2 moveInput;
    private Transform camTransform;
    private bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        camTransform = Camera.main.transform;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            // Aplicamos impulso hacia arriba
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        CheckGround();
        MoveSausage();
    }

    void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundDistance + 0.1f);

        Debug.Log("Tocando suelo: " + isGrounded);
    }

    void MoveSausage()
    {
        // Calcular dirección relativa a la cámara
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredDirection = forward * moveInput.y + right * moveInput.x;

        if (desiredDirection.magnitude > 0.1f)
        {
            // Movimiento
            rb.MovePosition(rb.position + desiredDirection * moveSpeed * Time.fixedDeltaTime);

            // Rotación suave hacia la dirección del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    /*void MoveSausage()
    {
        if (desiredDirection.magnitude > 0.1f)
        {
            // Solo aplicamos velocidad horizontal para no interferir con la gravedad/salto
            Vector3 currentVelocity = rb.linearVelocity; // En Unity 6 'velocity' se recomienda como 'linearVelocity'
            Vector3 targetVel = desiredDirection * moveSpeed;

            rb.linearVelocity = new Vector3(targetVel.x, currentVelocity.y, targetVel.z);

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }*/
}