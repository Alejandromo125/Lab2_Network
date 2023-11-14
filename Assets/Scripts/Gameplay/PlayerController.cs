using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int healthPoints;
    public string name;
    public TypesOfActions actions;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float moveSpeed = 5f;


    #region TimerForData
    [SerializeField]
    private float timeForUpdate;
    private float timerUpdate;
    #endregion
    private void Awake()
    {
        characterData = new CharacterData(healthPoints, this.transform, actions);
    }
    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleCharacterUpdates();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput).normalized;
        movement = transform.TransformDirection(movement); // Convert to character's local space.

        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        if (movement.magnitude > 0)
        {
            // Rotate the character to face the direction of movement.
            Quaternion newRotation = Quaternion.LookRotation(movement);
            transform.rotation = newRotation;
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        {
            // Do something with the hit object (e.g., damage it).
            Debug.Log("Hit object: " + hit.transform.name);
        }
    }
    #region NetworkUpdates
    private void UpdateCharacterData()
    {
        characterData.transform.position = gameObject.transform.position;
        characterData.transform.rotation = gameObject.transform.rotation;
        characterData.transform.localScale = gameObject.transform.localScale;

        characterData.HealthPoints = 10;

        characterData.actions.walk = true;
        characterData.actions.run = false;
        characterData.actions.dash = false;
        characterData.actions.shoot = false;
        characterData.actions.shield = false;

    }
    private void HandleCharacterUpdates()
    {
        timerUpdate += Time.deltaTime;
        if(timerUpdate > timeForUpdate)
        {
            UpdateCharacterData();
            UpdateInfo();
            timerUpdate= 0;
        }
    }
    void UpdateInfo()
    {
        Message message = new Message(name, characterData, TypesOfMessage.GAMEPLAY_ROOM);
        GameManager.instance.UpdateData(message);
    }

    #endregion
}