using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player camera setting")]
    public Transform lookAt;
    public Camera playerCamara;
    public float maxDistanceToLookAt = 8f;
    public float minDistanceToLookAt = 2f;
    public float cameraRotationSpeed = 10;
    public bool invertPitch;
    [Range(1, 179)] public float mMinPitch = 1;
    [Range(1, 179)] public float mMaxPitch = 179;

    [Header("Impulse Settings")]
    public float impulseForce = 3f;
    public float rotationForce = 1f;
    public float jumpForce = 5f;
    public float stopThreshold = 0.1f;

    private Rigidbody rb;
    private float _mYaw;
    private float _mPitch;
    private Vector2 _mLookDirection;
    private Vector3 cameraDirection;
    private float cameraDistance;
    private float scrollCameraDistance;
    private Vector2 scrollValue;
    private bool isColliding = false;
    public bool isGrounded = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraDirection = playerCamara.transform.position - lookAt.position;
        cameraDistance = Vector3.Distance(transform.position, playerCamara.transform.position);
        scrollCameraDistance = Vector3.Distance(transform.position, playerCamara.transform.position);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        PlayerCamera();
    }

    private void FixedUpdate()
    {
        isGrounded = Mathf.Abs(rb.linearVelocity.y) < stopThreshold && isColliding;

        // Lógica de desbloqueo combinada:
        // Solo recuperamos el control si está en el suelo Y casi detenido
        if (isGrounded && rb.linearVelocity.magnitude < stopThreshold && rb.angularVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void PlayerCamera()
    {
        _mYaw -= _mLookDirection.x * cameraRotationSpeed * Time.deltaTime;
        _mPitch += _mLookDirection.y * cameraRotationSpeed * Time.deltaTime;
        _mPitch = Math.Clamp(_mPitch, mMinPitch, mMaxPitch);

        cameraDirection = playerCamara.transform.position - lookAt.position;

        // Scroll para acercar y alejar camara
        scrollCameraDistance -= scrollValue.y;
        scrollCameraDistance = Math.Clamp(scrollCameraDistance, minDistanceToLookAt, maxDistanceToLookAt);
        //lookAt.localRotation = Quaternion.Euler(_mPitch * (invertPitch ? -1.0f : 1.0f), 0.0f, 0.0f);
        //lookAt.rotation = Quaternion.Euler(_mPitch * (invertPitch ? -1.0f : 1.0f), _mYaw, 0.0f);

        // Raycast desde jugador hacia camara para detectar obstaculos y mover camara mas hacia jugador
        Vector3 endPos;
        RaycastHit hit;
        if (Physics.Raycast(lookAt.transform.position, cameraDirection, out hit, scrollCameraDistance))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.layer == 6)
            {
                endPos = hit.point;
                cameraDistance = Vector3.Distance(lookAt.position, endPos);
            }
        }
        else
        {
            cameraDistance = scrollCameraDistance;
        }

        // Convertir grados a radianes
        float yawRad = _mYaw * Mathf.Deg2Rad;
        float pitchRad = _mPitch * Mathf.Deg2Rad;

        // Calculo de las coordenadas de camara cartesianas
        float x = cameraDistance * Mathf.Sin(pitchRad) * Mathf.Cos(yawRad);
        float y = cameraDistance * Mathf.Cos(pitchRad);
        float z = cameraDistance * Mathf.Sin(pitchRad) * Mathf.Sin(yawRad);
        Vector3 offset = new Vector3(x, y, z);

        playerCamara.transform.position = lookAt.position + offset;
        playerCamara.transform.LookAt(lookAt.position);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mLookDirection = context.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        scrollValue = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Vector2 input = context.ReadValue<Vector2>();
        if (input == Vector2.zero) return;

        if (input.x != 0 && input.y == 0)
        {
            ApplyRotationImpulse(input.x);
        }
        else if (input.y != 0)
        {
            ApplyForwardImpulse(input.y);
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.started || !isGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyForwardImpulse(float forwardInput)
    {
        Vector3 forward = playerCamara.transform.forward;
        forward.y = 0;
        forward.Normalize();

        rb.AddForce(forward * forwardInput * impulseForce, ForceMode.Impulse);
    }

    private void ApplyRotationImpulse(float sideInput)
    {
        Vector3 torque = Vector3.up * sideInput * rotationForce;
        rb.AddTorque(torque, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}