using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitDummyManager : MonoBehaviour
{
    DummyController dummyController_;

    void Start()
    {
        dummyController_ = FindObjectOfType<DummyController>();
    }

    void Update()
    {
        // Check for entity life conditions, if needed.
    }

    public void TakeDamage(int damage, GameObject entity)
    {
        dummyController_.healthPoints -= damage;
        Debug.Log("Object has been shot: " + entity);

        string temp = entity.gameObject.GetComponent<DummyController>().username;
        HandleHitEffect(temp, dummyController_.healthPoints);
        // Check for entity life conditions, e.g., destroy the entity if life reaches zero.
    }

    public void HandleHitEffect(string username,int healthPoints)
    {
        CharacterData characterData = new CharacterData();
        characterData.HealthPoints -= healthPoints;
        characterData.HealthPoints = characterData.HealthPoints <= 0 ? 0 : characterData.HealthPoints;
       
        characterData.actions.walk = false;
        characterData.actions.shoot = false;
        characterData.actions.run = false;
        characterData.actions.dash = false;
        characterData.actions.shield = false;
        Message message = new Message(username, characterData, TypesOfMessage.DUMMY_SHOOT);
        GameManager.instance.UpdateData(message);
    }
}