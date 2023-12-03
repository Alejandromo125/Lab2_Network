using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public struct GameScore
{
    public int scoreRedTeam;
    public int scoreBlueTeam;
}


public class GameManager : MonoBehaviour
{
    private ClientUDP_Script client;
    private ServerUDP_Script server;
    public List<DummyController> dummies;

    public GameObject playerPrefab;
    public GameObject dummyPrefab;

    public Vector3 startingPlayerPos;
    public Vector3 startingDummyPos;
    public static GameManager instance { get; private set; }

    public GameScore score;
    public TextMeshProUGUI scoreBlueTeam;
    public TextMeshProUGUI scoreRedTeam;
    // Start is called before the first frame update
    private void Awake()
    {
        score.scoreRedTeam = 0;
        score.scoreBlueTeam = 0;

        if(instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        client = FindObjectOfType<ClientUDP_Script>();
        server = FindObjectOfType<ServerUDP_Script>();
    }

    private void Update()
    {
        VictoryHandler();
        UpdateScore();
    }
    public void UpdateScore()
    {
        score.scoreBlueTeam = FindObjectOfType<PlayerController>().characterData.GameScore;
        score.scoreRedTeam = FindObjectOfType<DummyController>().characterData.GameScore;
        scoreBlueTeam.text = score.scoreBlueTeam.ToString();
        scoreRedTeam.text = score.scoreRedTeam.ToString();


    }
    private void VictoryHandler()
    {
        if(score.scoreRedTeam >= 5)
        {
            TriggerWin("Red Team");
        }
        else if(score.scoreBlueTeam >= 5)
        {
            TriggerWin("Blue Team");
        }

    }
    private void TriggerWin(string msg) 
    {
        Message message = new Message(msg, null, TypesOfMessage.FINISH_GAME);
        UpdateData(message); 
    }
    // Update is called once per frame
    public void UpdateData(Message message)
    {
        if(server)
        {
            server.HandleSendingMessages(message);
        }
        if(client)
        {
            client.SendMessageGameplay(message);
        }
    }

    public void UpdateDummiesData(Message message)
    {
        
        foreach (DummyController dummyController in dummies) 
        {
            if(dummyController.username == message.message) 
            {
                dummyController.UpdateDummy(message.characterData);
            }
        }
    }

    public void UpdatePlayersData(Message message)
    {

        PlayerController player = FindObjectOfType<PlayerController>();
        if(player.username == message.message) 
        {
            player.UpdateLocalData(message.characterData);
        }
    }

    public void CreatePlayerAndDummy(string playerName,string dummyName)
    {
       GameObject player =  Instantiate(playerPrefab,startingPlayerPos,Quaternion.identity,null);
       player.GetComponent<PlayerController>().username = playerName;
       player.GetComponent<PlayerController>().characterData.team = Team.BLUE_TEAM;

        GameObject dummy = Instantiate(dummyPrefab,startingDummyPos,Quaternion.identity,null);
       dummy.GetComponent<DummyController>().username = dummyName;
        dummy.GetComponent<DummyController>().characterData.team = Team.RED_TEAM;
       dummies.Add(dummy.GetComponent<DummyController>());
    }
}
