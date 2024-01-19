using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsHandler : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterData characterData;

    public Rigidbody rb;
    public Animator anim;
    public GameObject dashEffect;
    public GameObject mesh;
    public GameObject meshRifle;
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
        dashEffect.SetActive(characterData.actions.dash);
        mesh.SetActive(!characterData.actions.dash);
        if(characterData.actions.dash)
        {
            meshRifle.SetActive(false);
        }
        else if(!characterData.actions.dash)
        {
            meshRifle.SetActive(true);
        }
    }
}
