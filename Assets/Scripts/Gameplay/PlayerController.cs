using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string name;
    public CharacterData characterData;
    public Transform gunTransform;
    public LayerMask hitLayer;
    public float moveSpeed = 5f;

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
}