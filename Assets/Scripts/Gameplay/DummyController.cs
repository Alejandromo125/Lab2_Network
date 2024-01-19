using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyController : MonoBehaviour
{
    public int healthPoints;
    public string username;

    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public LayerMask hitDummyLayer;
    public float moveSpeed = 5f;
    public Camera mainCamera;
    private GameObject bulletPrefab; // Prefab for the explosion particle system.
    public GameObject explosionSparksPrefab; // Prefab for the explosion sparks particle system.
    public Transform particleSpawnerTr;
    public float shootRange = 7.0f;
    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;
    public LineRenderer raycastLine;
    public AudioClip shootClip;
    AudioSource audioSource;
    private bool alreadyShot = false;

    public GameObject blueTeamBullet, redTeamBullet;
    private void Awake()
    {
        characterData = new CharacterData();
        audioSource = gameObject.GetComponent<AudioSource>();

    }
    void Start()
    {
        raycastLine.enabled = false;

        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        bulletHitDummyManager_ = FindObjectOfType<BulletHitDummyManager>();

        mainCamera = FindObjectOfType<Camera>();

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
        if (characterData.actions.shoot == true && alreadyShot == false) 
        {
            //raycastLine.enabled = true;
            audioSource.PlayOneShot(shootClip);
            GameObject explosion = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation);
            alreadyShot = true;
        }

        if(characterData.actions.shoot == false)
        {
            alreadyShot = false;

        }
    }

    public void UpdateDummy(CharacterData data)
    {
        characterData.position = data.position;
        characterData.rotation = data.rotation;

        characterData.actions.run = data.actions.run;
        characterData.actions.shoot = data.actions.shoot;
        characterData.actions.walk = data.actions.walk;
        characterData.actions.dash = data.actions.dash;
        characterData.actions.shield = data.actions.shield;

        characterData.HealthPoints = data.HealthPoints;
        characterData.GameScore = data.GameScore;

        FindObjectOfType<HP_Bar_Manager>().SetWidth_v2(characterData.HealthPoints);
        UpdateActualData();
    }

    public void UpdateActualData()
    {
        transform.position = characterData.position;
        transform.rotation = characterData.rotation;
        healthPoints = characterData.HealthPoints;
        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);


        //if (healthPoints <= 0)
        //{
        //    healthPoints = 100;
        //    characterData.HealthPoints = 100;
        //    FindObjectOfType<HP_Bar_Manager>().Change(100);
        //    FindObjectOfType<HP_Bar_Manager>().Change(-1);
        //}

    }
    void DummyDisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
