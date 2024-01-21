using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class PlayerController : MonoBehaviour
{
    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;

    AudioSource audioSource;

    public string username;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public LayerMask hitDummyLayer;
    public LayerMask Obstacle;

    public float moveSpeed = 5f;
    public float maxSpeed = 7f;
    public float rotationSpeed = 10f; // Rotation speed around the Y-axis.
    public float deceleration = 5f; // Deceleration rate.
    public float acceleration = 5f; // Deceleration rate.
    public LineRenderer raycastLine;
    public float shootDelay = 0.5f; // Delay between shots.
    private GameObject bulletPrefab; 
    public GameObject explosionPrefab; // Prefab for the explosion particle system.
    public GameObject explosionSparksPrefab;
    public GameObject hitParticlesPrefab;
    public GameObject deathParticlesPrefab;
    public GameObject respawnParticlesPrefab;
    public Transform particleSpawnerTr;
    public float shootRange = 7.0f;

    public Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private float lastShootTime;
    public float lastShieldTime;
    public float shieldDelay = 15f;
    public float shieldTimer = 5f;
    
    private TypesOfActions actions;

    private CinemachineVirtualCamera cam;

    public Transform aimTarget;

    public AudioClip shootSound;

    HP_Bar_ForPlayer hp_Bar_Manager_ForPlayer_;
    TextHP_Set textHP_Set_;

    public float dashDistance = 4f;
    public float dashDuration = 0.15f;
    private bool isDashing = false;
    private bool recievedDamage = false;
    private bool justHealed = false;
    public GameObject blueTeamBullet, redTeamBullet, shockBullet;
    private Quaternion LastRotation;
    private float timerShoots = 0f;

    #region TimerForData
    [SerializeField]
    private float timeForUpdate;
    public float lastShootTimeShotgun;
    public float shootDelayshotgun = 5f;
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

        actions.heal = false;

        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        bulletHitDummyManager_ = FindObjectOfType<BulletHitDummyManager>();

        mainCamera = FindObjectOfType<Camera>();

        audioSource = GetComponent<AudioSource>();

        hp_Bar_Manager_ForPlayer_ = FindObjectOfType<HP_Bar_ForPlayer>();
        textHP_Set_ = FindObjectOfType<TextHP_Set>();

        switch (characterData.team)
        {
            case Team.NONE:
                break;
            case Team.BLUE_TEAM:
                bulletPrefab = blueTeamBullet;
                break;
            case Team.RED_TEAM:
                bulletPrefab = redTeamBullet;
                break;
        }

    }

    void Update()
    {
        
        HandleMovement();
        HandleActions();
        UpdateCharacterData();
        HandleCharacterUpdates();
        
    }
 
    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        actions.run = Input.GetKey(KeyCode.LeftShift) == true ? true : false;

        if (movement != Vector3.zero) {
            actions.walk = true; 
        }
        if (movement == Vector3.zero) {
            actions.walk = false; 
        }
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
            UpdateInfo();
        }
        else
        {
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }
 
    void HandleActions()
    {
        if (Input.GetMouseButton(0) && Time.time - lastShootTime > shootDelay)
        {
            Shoot();
            lastShootTime = Time.time;
            actions.shoot = true;
        }
        else if(Time.time - lastShootTime < shootDelay)
        {
            actions.shoot = false;
        }

        if (Input.GetMouseButton(1) && Time.time - lastShootTimeShotgun > shootDelayshotgun)
        {
            
            Shotgun();
            lastShootTimeShotgun = Time.time;
            actions.shotgun = true;
        }
        else if (Time.time - lastShootTimeShotgun < shootDelayshotgun)
        {
            actions.shotgun = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastShieldTime > shieldDelay)
        {
            lastShieldTime = Time.time;
            actions.shield = true;
        }
        else if (Time.time - lastShieldTime > shieldTimer && actions.shield)
        {
            actions.shield = false;
        }
    }

    void Shoot()
    {
        audioSource.PlayOneShot(shootSound);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation);
    }

    public void FiveShoots_1()
    {
        //Quaternion rotation = Quaternion.Euler(0, degree, 0);
        //GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
        //for (int i = 0; i <= 300; i++)
        //{
        //    if (i == 300)
        //    {
        //        if (degree <= 30)
        //        {
        //            if (timerShoots >= 0.2f)
        //            {
        //                timerShoots = 0f;
        //                FiveShoots(degree + 15);
        //            }   
        //        }

        //    }
        //}

        Quaternion rotation = Quaternion.Euler(0, -30, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    public void FiveShoots_2()
    {
        Quaternion rotation = Quaternion.Euler(0, -15, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    public void FiveShoots_3()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    public void FiveShoots_4()
    {
        Quaternion rotation = Quaternion.Euler(0, 15, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    public void FiveShoots_5()
    {
        Quaternion rotation = Quaternion.Euler(0, 30, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    void Shotgun()
    {
        Invoke("FiveShoots_1", 0.0f);
        Invoke("FiveShoots_2", 0.1f);
        Invoke("FiveShoots_3", 0.2f);
        Invoke("FiveShoots_4", 0.3f);
        Invoke("FiveShoots_5", 0.4f);
    }

    #region NetworkUpdates
    private void UpdateCharacterData()
    {
        characterData.position = gameObject.transform.position;
        characterData.rotation = gameObject.transform.rotation;
    
        characterData.HealthPoints = bulletHitManager_.entityLife;
        actions.dash = isDashing;
        characterData.actions = actions;

        hp_Bar_Manager_ForPlayer_.SetWidth_v2(characterData.HealthPoints);
        textHP_Set_.playerHP = bulletHitManager_.entityLife;

        if (characterData.HealthPoints <= 0)
        {
            switch(characterData.team)
            {
                case Team.NONE:

                    break;
                case Team.BLUE_TEAM:
                    GameObject deathParticles = Instantiate(deathParticlesPrefab, gameObject.transform.position, transform.rotation);
                    Destroy(deathParticles, 1f);

                    gameObject.transform.position = GameManager.instance.startingBluePos;

                    GameObject respawnParticles = Instantiate(respawnParticlesPrefab, gameObject.transform.position, transform.rotation);
                    Destroy(respawnParticles, 1.5f);

                    break;
                case Team.RED_TEAM:
                    GameObject deathParticles2 = Instantiate(deathParticlesPrefab, gameObject.transform.position, transform.rotation);
                    Destroy(deathParticles2, 1f);

                    gameObject.transform.position = GameManager.instance.startingRedPos;

                    GameObject respawnParticles2 = Instantiate(respawnParticlesPrefab, gameObject.transform.position, transform.rotation);
                    Destroy(respawnParticles2, 1.5f);

                    break;
            }
            characterData.position = gameObject.transform.position;
            bulletHitManager_.entityLife = 100;
            characterData.HealthPoints = 100;
            hp_Bar_Manager_ForPlayer_.SetWidth_v2(characterData.HealthPoints);
            hp_Bar_Manager_ForPlayer_.Change(100);
            hp_Bar_Manager_ForPlayer_.Change(-1);
            int teamValue = (int)characterData.team;
            Message message = new Message(teamValue.ToString(), characterData, TypesOfMessage.DUMMY_SHOOT);
            ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
            ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
            if(client)
            {
                client.SendStartMessage(message);
            }
            else if(server)
            {
                server.HandleSendingMessages(message);
            }
        }
    }
    private void HandleCharacterUpdates()
    {
        if(actions.walk || actions.run || actions.dash || actions.shoot || actions.shield || recievedDamage || actions.heal)
        {
            UpdateInfo();
            recievedDamage = false;
            actions.heal = false;
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
    #endregion

    void OnTriggerStay(Collider trigger)
    {
        if (!actions.heal)
        {
            if (trigger.gameObject.CompareTag("HealPad"))
            {
                bulletHitManager_.entityLife += 1;

                if (bulletHitManager_.entityLife >= 100)
                {
                    bulletHitManager_.entityLife = 100;
                }

                actions.heal = true;

            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Current collision: " + collision.gameObject);

        if (!actions.shield)
        {
            switch (characterData.team)
            {
                case Team.NONE:
                    break;
                case Team.BLUE_TEAM:
                    if (collision.gameObject.CompareTag("RedTeamBullet"))
                    {
                        bulletHitManager_.entityLife -= 10;
                        recievedDamage = true;
                    }
                    break;
                case Team.RED_TEAM:
                    if (collision.gameObject.CompareTag("BlueTeamBullet"))
                    {
                        bulletHitManager_.entityLife -= 10;
                        recievedDamage = true;
                    }
                    break;
            }
            
        }
    }
    IEnumerator Dash()
    {
        isDashing = true;

        // Store the initial position
        Vector3 startPosition = transform.position;

        // Calculate the end position based on the dash distance
        Vector3 endPosition = startPosition + transform.forward * dashDistance;

        RaycastHit hit;
        if (Physics.Raycast(startPosition, transform.forward, out hit, dashDistance))
        {
            // Adjust the end position based on the obstacle hit
            endPosition = hit.point;
        }

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            // Move the player towards the end position
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / dashDuration);

            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure the player is exactly at the end position
        transform.position = endPosition;

        isDashing = false;
    }
}