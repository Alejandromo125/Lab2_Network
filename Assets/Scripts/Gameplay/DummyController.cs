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
    public GameObject explosionPrefab; // Prefab for the explosion particle system.
    public GameObject explosionSparksPrefab; // Prefab for the explosion sparks particle system.
    public Transform particleSpawnerTr;
    public float shootRange = 7.0f;
    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;
    public LineRenderer raycastLine;
    public AudioClip shootClip;
    AudioSource audioSource;

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
    }

    void Update()
    {
        if (characterData.actions.shoot == true) 
        {
            raycastLine.enabled = true;
            audioSource.PlayOneShot(shootClip);
            GameObject explosion = Instantiate(explosionPrefab, particleSpawnerTr.position, transform.rotation);
            GameObject explosionSparks = Instantiate(explosionSparksPrefab, particleSpawnerTr.position, transform.rotation);

            // Get the duration of the particle system's effect.
            ParticleSystem particleSystem = explosion.GetComponent<ParticleSystem>();
            float duration = particleSystem.main.duration;

            // Destroy the explosion prefab after the particle effect duration.
            Destroy(explosion, duration);
            Destroy(explosionSparks, duration);

            Invoke("DummyDisableRaycastLine", 0.1f);
        }
    }

    public void UpdateDummy(CharacterData data)
    {
        characterData.position = data.position;
        characterData.rotation = data.rotation;

        characterData.actions.run = data.actions.run;
        characterData.actions.shoot = data.actions.shoot;
        characterData.actions.walk = data.actions.walk;

        characterData.GameScore = data.GameScore;
        // Update also the line renderer as it is attached to the raycast
        FindObjectOfType<HP_Bar_Manager>().Set(healthPoints);
        UpdateActualData();
    }

    public void UpdateActualData()
    {
        transform.position = characterData.position;
        transform.rotation = characterData.rotation;

        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);


        if (healthPoints <= 0)
        {
            healthPoints = 100;
            characterData.HealthPoints = 100;
            FindObjectOfType<HP_Bar_Manager>().Change(100);
        }

    }
    void DummyDisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
