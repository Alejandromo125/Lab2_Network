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

    public int TakeDamage(int damage, GameObject entity)
    {
        dummyController_.healthPoints -= damage;
        Debug.Log("Object has been shot: " + entity);

        string temp = entity.gameObject.GetComponent<DummyController>().username;
        HandleHitEffect(temp, dummyController_.healthPoints,entity);

        return dummyController_.healthPoints;
    }

    public void HandleHitEffect(string username,int healthPoints,GameObject entity)
    {
        CharacterData characterData = new CharacterData();
        characterData.HealthPoints = healthPoints;
        characterData.HealthPoints = characterData.HealthPoints <= 0 ? 0 : characterData.HealthPoints;

        characterData.position = entity.transform.position;
        characterData.rotation = entity.transform.rotation;

        characterData.team = entity.GetComponent<DummyController>().characterData.team;
        characterData.GameScore = entity.GetComponent<DummyController>().characterData.GameScore;
        characterData.actions.walk = false;
        characterData.actions.shoot = false;
        characterData.actions.run = false;
        characterData.actions.dash = false;
        characterData.actions.shield = false;
        Message message = new Message(username, characterData, TypesOfMessage.DUMMY_SHOOT);
        GameManager.instance.UpdateData(message);
    }
}