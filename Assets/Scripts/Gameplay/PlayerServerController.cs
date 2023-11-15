using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerServerController : MonoBehaviour
{
    JsonStorageManager jsonStorageManager_;

    //public float deserializationDelay = 0.5f; // Delay for receiving data from server via json deserialization
    public bool disableDataReceive = false; // Disable receiving data for testing or other purposes

    //private float lastDeserializationTime;
    private string json;

    PlayerStruct player;

    // Start is called before the first frame update
    void Start()
    {
        jsonStorageManager_ = FindObjectOfType<JsonStorageManager>();

        json = jsonStorageManager_.LoadJsonFile("playerData.json", "Assets/Scripts/Jsons/");

        player = JsonUtility.FromJson<PlayerStruct>(json);
    }

    // Update is called once per frame
    void Update()
    {
        if (/*(Time.time - lastDeserializationTime > deserializationDelay) &&*/ disableDataReceive == false)
        {
            JsonUtility.FromJsonOverwrite(json, player);

            //lastDeserializationTime = Time.time;

            transform.position = player.position;
            transform.rotation = player.rotation;
        }
    }
}
