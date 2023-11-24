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
    public float shootRange = 7.0f;


    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;
    public LineRenderer raycastLine;

    void Start()
    {
        raycastLine.enabled = false;

        bulletHitManager_ = FindObjectOfType<BulletHitManager>();
        bulletHitDummyManager_ = FindObjectOfType<BulletHitDummyManager>();

        mainCamera = FindObjectOfType<Camera>();
    }

    void Update()
    {
        if(characterData.actions.shoot == true)
        {
            DummyShoot();
        }
    }

    public void UpdateDummy(CharacterData data)
    {
        characterData.position = data.position;
        characterData.rotation = data.rotation;

        characterData.HealthPoints = data.HealthPoints;

        characterData.actions = data.actions;

        // Update also the line renderer as it is attached to the raycast

        UpdateActualData();
    }

    public void UpdateActualData()
    {
        Debug.Log("Updating dummy data");
        transform.position = characterData.position;
        transform.rotation = characterData.rotation;
        healthPoints = characterData.HealthPoints;

        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);

        if (characterData.HealthPoints <= 0)
        {
            transform.position = new Vector3(0.0f, 1.0f, 0.0f);
            characterData.position = transform.position;

            healthPoints = 100;
        }

    }

    void DummyShoot()
    {
        raycastLine.enabled = true;

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

                bulletHitManager_.TakeDamage(100, hit.collider.gameObject);
            }
            else if (Physics.Raycast(transform.position, direction.normalized, out hit, shootRange, hitDummyLayer))
            {
                //raycastLine.SetPosition(1, hit.point);
                UnityEngine.Debug.Log("Hit object: " + hit.transform.name);

                bulletHitDummyManager_.TakeDamage(100, hit.collider.gameObject);
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

        Invoke("DummyDisableRaycastLine", 0.2f);
    }

    void DummyDisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
