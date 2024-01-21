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
    private bool alreadyShotgun = false;

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

        if (characterData.actions.shotgun == true && alreadyShotgun == false)
        {
            //raycastLine.enabled = true;
            audioSource.PlayOneShot(shootClip);

            Invoke("FiveShoots_1", 0.0f);
            Invoke("FiveShoots_2", 0.1f);
            Invoke("FiveShoots_3", 0.2f);
            Invoke("FiveShoots_4", 0.3f);
            Invoke("FiveShoots_5", 0.4f);

            alreadyShotgun = true;
        }

        if (characterData.actions.shotgun == false)
        {
            alreadyShotgun = false;

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
        characterData.actions.shotgun = data.actions.shotgun;

        characterData.HealthPoints = data.HealthPoints;
        characterData.GameScore = data.GameScore;

        gameObject.GetComponent<HP_Bar_Manager>().SetWidth_v2(characterData.HealthPoints);
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

    void FiveShoots_1()
    {
        Quaternion rotation = Quaternion.Euler(0, -30, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    void FiveShoots_2()
    {
        Quaternion rotation = Quaternion.Euler(0, -15, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    void FiveShoots_3()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    void FiveShoots_4()
    {
        Quaternion rotation = Quaternion.Euler(0, 15, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }

    void FiveShoots_5()
    {
        Quaternion rotation = Quaternion.Euler(0, 30, 0);
        GameObject bullet = Instantiate(bulletPrefab, particleSpawnerTr.position, transform.rotation * rotation);
    }
}
