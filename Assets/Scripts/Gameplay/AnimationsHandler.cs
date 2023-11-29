using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private CharacterData characterData;

    public Rigidbody rb;
    public Animator anim;

    private bool triggerWalk;
    private bool shoot;
    void Start()
    {
        triggerWalk = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<PlayerController>() != null) 
        { characterData = gameObject.GetComponent<PlayerController>().characterData; }
        if (gameObject.GetComponent<DummyController>() != null) 
        { characterData = gameObject.GetComponent<DummyController>().characterData;}
        
        AnimationsStates();
    }

    void AnimationsStates()
    {
        Vector3 velocityNormalized = rb.velocity.normalized;
        Vector3 vel = transform.InverseTransformDirection(velocityNormalized);

        if ((vel.x > Vector3.zero.x || vel.z > Vector3.zero.z) && triggerWalk == false)
        {
            anim.SetTrigger("walkTrigger");
            triggerWalk = true;
        }
        if((vel.x <= Vector3.zero.x || vel.z <= Vector3.zero.z))
        {
            triggerWalk = false;
        }

        anim.SetFloat("DirectionForward", vel.z);
        anim.SetFloat("DirectionSides", vel.x);

        anim.SetBool("shoot", characterData.actions.shoot);
        anim.SetBool("walk", characterData.actions.walk);
    }
}
