using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float maxSpeed = 7f;
    public float rotationSpeed = 10f; // Rotation speed around the Y-axis.
    public float deceleration = 5f; // Deceleration rate.
    public float acceleration = 5f; // Deceleration rate.
    public LineRenderer raycastLine;
    public float shootDelay = 0.3f; // Delay between shots.
    public GameObject explosionPrefab; // Prefab for the explosion particle system.

    //public CinemachineFreeLook freeLookCamera; // Reference to your CinemachineFreeLook component.
    //public CinemachineImpulseSource impulseSource; // Reference to CinemachineImpulseSource for camera shake.

    public Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private float lastShootTime;

    float rotationAngle = 0f;

    void Start()
    {
        //mainCamera = Camera.main;
        raycastLine.enabled = true;

        // Initialize references to the Cinemachine components.
        //freeLookCamera = GetComponentInChildren<CinemachineFreeLook>();
        //impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

    // Call this method to trigger camera shake.
    //void ShakeCamera(float shakeDuration, float shakeAmplitude, float shakeFrequency)
    //{
    //    impulseSource.GenerateImpulse(Vector3.one); // Basic impulse, can be customized.
    //}

    //// Call this method to zoom the camera.
    //void ZoomCamera(float zoomAmount)
    //{
    //    freeLookCamera.m_Orbits[1].m_Radius += zoomAmount;
    //}

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 forward = transform.forward;

        // Apply acceleration and deceleration for smoother movement, limited to maxSpeed.
        if (velocity.magnitude < maxSpeed)
        {
            // Deceleration when no input is detected.
            if (movement == Vector3.zero)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.deltaTime);
            }
            else
            {
                velocity += movement * maxSpeed * Time.deltaTime * acceleration;
            }
            
        }
        else
        {
            // Deceleration when no input is detected.
            if (movement == Vector3.zero)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, deceleration * Time.deltaTime);
            }
            else
            {
                velocity = movement * maxSpeed;
            }
            
        }

        // Apply the movement in the player's forward direction.
        transform.Translate(velocity * Time.deltaTime, Space.World);



        // Rotate the character to look at the mouse pointer instantly on the Y-axis.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 lookAtPoint = hit.point;

            Vector3 lookDir = lookAtPoint - transform.position;
            lookDir.y = 0;

            transform.LookAt(transform.position + lookDir, Vector3.up); // Y-axis rotation only.
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShootTime > shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        raycastLine.enabled = true;
        raycastLine.SetPosition(0, gunTransform.position);

        // Spawn the explosion particle at the gun's position.
        GameObject explosion = Instantiate(explosionPrefab, gunTransform.position, Quaternion.identity);
        
        // Get the duration of the particle system's effect.
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;

        // Destroy the explosion prefab after the particle effect duration.
        Destroy(explosion, duration);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        {
            raycastLine.SetPosition(1, hit.point);
            Debug.Log("Hit object: " + hit.transform.name);
        }
        else
        {
            Vector3 rayEnd = ray.GetPoint(100f);
            raycastLine.SetPosition(1, rayEnd);
        }

        //Invoke("DisableRaycastLine", 0.2f);
    }

    void DisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
