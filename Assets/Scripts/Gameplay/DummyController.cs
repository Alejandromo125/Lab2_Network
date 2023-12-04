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


    public Transform spawner;
    BulletHitManager bulletHitManager_;
    BulletHitDummyManager bulletHitDummyManager_;
    public LineRenderer raycastLine;

    private void Awake()
    {
        characterData = new CharacterData();
       
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
        if (characterData.actions.shoot == true) //<-- null reference of an object
        {
            raycastLine.enabled = true;
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

        UpdateActualData();
    }

    public void UpdateActualData()
    {
        transform.position = characterData.position;
        transform.rotation = characterData.rotation;

        gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);


        if (healthPoints <= 0)
        {
            //transform.position = new Vector3(0.0f, 3.0f, 0.0f);
            //characterData.position = transform.position;
            healthPoints = 100;
            characterData.HealthPoints = 100;
        }

    }
    void DummyDisableRaycastLine()
    {
        raycastLine.enabled = false;
    }
}
