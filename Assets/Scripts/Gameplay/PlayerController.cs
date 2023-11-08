using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float maxSpeed = 7f;
    public float rotationSpeed = 10f; // Rotation speed around the Y-axis.
    public float deceleration = 5f; // Deceleration rate.
    public float acceleration = 5f; // Deceleration rate.
    public LineRenderer raycastLine;

    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    float rotationAngle = 0f;

    public GameObject explosionPrefab; // Prefab for the explosion particle system.

    void Start()
    {
        mainCamera = Camera.main;
        raycastLine.enabled = false;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
    }

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
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        raycastLine.enabled = true;
        raycastLine.SetPosition(0, gunTransform.position);

        // Spawn the explosion particle at the gun's position.
        Instantiate(explosionPrefab, gunTransform.position, Quaternion.identity, this.transform);

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

        Invoke("DisableRaycastLine", 0.2f);
    }

    void DisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
