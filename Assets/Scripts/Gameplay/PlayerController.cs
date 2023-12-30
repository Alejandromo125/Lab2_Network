using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class PlayerController : MonoBehaviour
{
    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;

    //public int healthPoints; <-- Not needed, takes it from bullet hit manager
    public string username;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public LayerMask hitDummyLayer;
    public float moveSpeed = 5f;
    public float maxSpeed = 7f;
    public float rotationSpeed = 10f; // Rotation speed around the Y-axis.
    public float deceleration = 5f; // Deceleration rate.
    public float acceleration = 5f; // Deceleration rate.
    public LineRenderer raycastLine;
    public float shootDelay = 0.3f; // Delay between shots.
    public GameObject explosionPrefab; // Prefab for the explosion particle system.
    public float shootRange = 7.0f;

    public Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private float lastShootTime;
    private TypesOfActions actions;

    private CinemachineVirtualCamera cam;

    public Transform aimTarget;

    private bool playerRespawn = false;
    #region TimerForData
    [SerializeField]
    private float timeForUpdate;
    private float timerUpdate;
    #endregion
    private void Awake()
    {
        characterData = new CharacterData();
        cam = FindObjectOfType<CinemachineVirtualCamera>();

        cam.LookAt = gameObject.transform;
        cam.Follow = gameObject.transform;
    }

    private void Start()
    {
        raycastLine.enabled = false;

        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        bulletHitDummyManager_ = FindObjectOfType<BulletHitDummyManager>();

        mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        UpdateCharacterData();
        HandleCharacterUpdates();
    }
 
    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        actions.run = Input.GetKey(KeyCode.LeftShift) == true ? true : false;

        if (movement != Vector3.zero) { actions.walk = true; }
        if (movement == Vector3.zero) { actions.walk = false; }
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
        if(actions.run == false)
            transform.Translate(velocity * Time.deltaTime, Space.World);
        else
            transform.Translate(velocity * 1.5f * Time.deltaTime, Space.World);

        // Rotate the character to look at the mouse pointer instantly on the Y-axis.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Vector3 lookAtPoint = hit.point;

            Vector3 lookDir = lookAtPoint - transform.position;
            lookDir.y = 0;

            transform.LookAt(transform.position + lookDir, Vector3.up); // Y-axis rotation only.
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            cam.LookAt = aimTarget;

        }
        else if(Input.GetMouseButtonUp(1))
        {
            cam.LookAt = gameObject.transform;

        }
    }
 
    void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShootTime > shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
            actions.shoot = true;
            
        }
        else
        {
            actions.shoot = false;
        }
    }
 
    void Shoot()
    {
        raycastLine.enabled = true;
        bool score = false;
        // Spawn the explosion particle at the gun's position.
        GameObject explosion = Instantiate(explosionPrefab, gunTransform.position, Quaternion.identity);

        // Get the duration of the particle system's effect.
        ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
        float duration = particleSystem.main.duration;

        // Destroy the explosion prefab after the particle effect duration.
        Destroy(explosion, duration);

        // Get the mouse position in world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Invoke("DisableRaycastLine", 0.2f);


        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Calculate the direction from player to mouse pointer in X,Z plane
            Vector3 direction = hit.point - transform.position;
            direction.y = 0f; // Make sure the direction is parallel to the ground


            // Cast a new ray from player position towards the calculated direction
            if (Physics.Raycast(transform.position, direction.normalized, out hit, shootRange, hitLayer))
            {
                //raycastLine.SetPosition(1, hit.point);
                UnityEngine.Debug.Log("Hit object: " + hit.transform.name);

                bulletHitManager_.TakeDamage(10, hit.collider.gameObject);
            }
            else if (Physics.Raycast(transform.position, direction.normalized, out hit, shootRange, hitDummyLayer))
            {
                //raycastLine.SetPosition(1, hit.point);
                UnityEngine.Debug.Log("Hit object: " + hit.transform.name);

                if(bulletHitDummyManager_.TakeDamage(10, hit.collider.gameObject) <= 0)
                {
                    characterData.GameScore++;
                }
                
            }
            else
            {
                // If no hit, set a default end point for the ray.
                Vector3 rayEnd = transform.position + direction.normalized * 100f;
                //raycastLine.SetPosition(1, rayEnd);
            }
        }
        else
        {
            // Handle case where mouse doesn't hit anything
            // You might want to adjust this behavior based on your requirements
            // For instance, you could set a default direction if no hit occurs
            UnityEngine.Debug.Log("Mouse pointer doesn't hit anything.");
        }
    }
    void DisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
    #region NetworkUpdates
    private void UpdateCharacterData()
    {
        characterData.position = gameObject.transform.position;
        characterData.rotation = gameObject.transform.rotation;
 
        characterData.HealthPoints = bulletHitManager_.entityLife;

        characterData.actions = actions;

        FindObjectOfType<HP_Bar_ForPlayer>().Set(characterData.HealthPoints);

        if (characterData.HealthPoints <= 0)
        {
            playerRespawn = true;
            Invoke("ResetUpdates", 0.5f);
            switch(characterData.team)
            {
                case Team.NONE:

                    break;
                case Team.BLUE_TEAM:
                    gameObject.transform.position = GameManager.instance.startingBluePos;

                    break;
                case Team.RED_TEAM:
                    gameObject.transform.position = GameManager.instance.startingRedPos;

                    break;
            }
            characterData.position = gameObject.transform.position;
            bulletHitManager_.entityLife = 100;
            characterData.HealthPoints = 100;
            FindObjectOfType<HP_Bar_ForPlayer>().Change(100);

        }
    }
    private void HandleCharacterUpdates()
    {
        timerUpdate += Time.deltaTime;
        if (playerRespawn)
            return;
        if(timerUpdate > timeForUpdate)
        {
            UpdateCharacterData();
            UpdateInfo();
            timerUpdate= 0;
        }
    }
    void UpdateInfo()
    {
        Message message = new Message(username, characterData, TypesOfMessage.GAMEPLAY_ROOM);
        GameManager.instance.UpdateData(message);
    }
 

    public void UpdateLocalData(CharacterData data)
    {
        characterData.HealthPoints = data.HealthPoints;
        bulletHitManager_.entityLife = data.HealthPoints;
        gameObject.transform.position = data.position;
        gameObject.transform.rotation = data.rotation;
        characterData.GameScore = data.GameScore;
    }
    private void ResetUpdates()
    {
        playerRespawn = false;
    }

    #endregion

}