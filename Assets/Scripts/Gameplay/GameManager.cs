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

    public Vector3 startingBluePos;
    public Vector3 startingRedPos;
    public static GameManager instance { get; private set; }

    public GameScore score;
    public TextMeshProUGUI scoreBlueTeam;
    public TextMeshProUGUI scoreRedTeam;


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
            Invoke("TriggerWinRed",2);
        }
        else if(score.scoreBlueTeam >= 5)
        {
            Invoke("TriggerWinBlue", 2);
        }

    }
    private void TriggerWinRed() 
    {
        Message message = new Message("Red team", null, TypesOfMessage.FINISH_GAME);
        UpdateData(message); 
    }
    private void TriggerWinBlue()
    {
        Message message = new Message("Blue team", null, TypesOfMessage.FINISH_GAME);
        UpdateData(message);
    }

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

    public void CreatePlayerAndDummy(string playerName,Team playerTeam,string dummyName,Team dummyTeam)
    {
        GameObject player = null;
        GameObject dummy = null;
        if (playerTeam == Team.BLUE_TEAM)
        {
            player = Instantiate(playerPrefab, startingBluePos, Quaternion.identity, null);
        }
        else if(playerTeam == Team.RED_TEAM)
        {
            player = Instantiate(playerPrefab, startingRedPos, Quaternion.identity, null);
        }
        player.GetComponent<PlayerController>().username = playerName;
        player.GetComponent<PlayerController>().characterData.team = playerTeam;

        if (dummyTeam == Team.BLUE_TEAM)
        {
            dummy = Instantiate(dummyPrefab, startingBluePos, Quaternion.identity, null);
        }
        else if (dummyTeam == Team.RED_TEAM)
        {
            dummy = Instantiate(dummyPrefab, startingRedPos, Quaternion.identity, null);
        }
        dummy.GetComponent<DummyController>().username = dummyName;
        dummy.GetComponent<DummyController>().characterData.team = dummyTeam;
        dummies.Add(dummy.GetComponent<DummyController>());
    }
}
