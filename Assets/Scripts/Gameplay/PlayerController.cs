using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    BulletHitManager bulletHitManager_;
    JsonStorageManager jsonStorageManager_;

    public Transform gunTransform;
    public LayerMask hitLayer;
    public float maxSpeed = 7f;
    public float rotationSpeed = 10f; // Rotation speed around the Y-axis.
    public float deceleration = 5f; // Deceleration rate.
    public float acceleration = 5f; // Deceleration rate.
    public LineRenderer raycastLine;
    public float shootDelay = 0.3f; // Delay between shots.
    public GameObject explosionPrefab; // Prefab for the explosion particle system.
    public float serializationDelay = 0.5f; // Delay for sending data to server via json serialization
    public bool disableDataSend = false; // Disable sending data for testing or other purposes

    public Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private float lastShootTime;
    private float lastSerializationTime;
    private string json;

    float rotationAngle = 0f;

    PlayerStruct player;

    void Start()
    {
        //mainCamera = Camera.main;
        raycastLine.enabled = true;
        
        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        jsonStorageManager_ = FindObjectOfType<JsonStorageManager>();

        
        player.position = transform.position;
        player.rotation = transform.rotation;

        player.isMoving = false;
        player.shooting= false;
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();

        if ((Time.time - lastSerializationTime > serializationDelay) && player.isMoving == true && disableDataSend == false)
        {
            json = JsonUtility.ToJson(player);

            jsonStorageManager_.SaveJsonFile("playerData.json", json, "/Assets/Scripts/Jsons/");

            lastSerializationTime = Time.time;

            //UnityEngine.Debug.Log("Saving player Json");
        }

        if(player.shooting == true && disableDataSend == false)
        {
            json = JsonUtility.ToJson(player);
        }
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

        if (player.position == transform.position && player.rotation == transform.rotation)
        {
            player.isMoving = false;
            
        }
        else if (player.position != transform.position || player.rotation != transform.rotation)
        {
            player.isMoving = true;
            player.position = transform.position;
            player.rotation = transform.rotation;
        }
        
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShootTime > shootDelay)
        {
            player.shooting = true;

            Shoot();
            lastShootTime = Time.time;
        }
        else
        {
            player.shooting = false;
        }
    }

    void Shoot()
    {
        raycastLine.enabled = true;
        //raycastLine.SetPosition(0, gunTransform.position);

        // Spawn the explosion particle at the gun's position.
        GameObject explosion = Instantiate(explosionPrefab, gunTransform.position, Quaternion.identity);
        
        // Get the duration of the particle system's effect.
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;

        // Destroy the explosion prefab after the particle effect duration.
        Destroy(explosion, duration);

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //ray.origin = raycastLine.transform.position;
        //ray.direction = raycastLine.transform.position;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        ray.origin.Equals(raycastLine.transform.forward);
        RaycastHit hit;

        

        if (Physics.Raycast(ray, out hit, 10, hitLayer))
        {
            //raycastLine.SetPosition(1, hit.point);
            UnityEngine.Debug.Log("Hit object: " + hit.transform.name);

            bulletHitManager_.TakeDamage(100, GameObject.Find(hit.collider.gameObject.name));
            
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
