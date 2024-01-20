using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    private Rigidbody rb;
    public LayerMask mask;
    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Apply initial force to the bullet in the forward direction
        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BlueTeamBullet") || collision.gameObject.CompareTag("RedTeamBullet"))
            return;

        Destroy(gameObject); 
    }
}
