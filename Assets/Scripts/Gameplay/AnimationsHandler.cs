using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterData characterData;

    public Rigidbody rb;
    public Animator anim;

    void Start()
    {
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
        anim.SetBool("shoot", characterData.actions.shoot);
        anim.SetBool("walk", characterData.actions.walk);
        anim.SetBool("run", characterData.actions.run);
    }
}
