using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    public GameObject playerPrefabBlue;
    public GameObject playerPrefabRed;

    public GameObject dummyPrefabBlue;
    public GameObject dummyPrefabRed;


    public Vector3 startingBluePos;
    public Vector3 startingRedPos;
    public static GameManager instance { get; private set; }

    public GameScore score;
    private List<GameObject> blueTeamList = new List<GameObject>();
    private List<GameObject> redTeamList = new List<GameObject>();

    public TextMeshProUGUI scoreBlueTeam;
    public TextMeshProUGUI scoreRedTeam;

    public GameObject EndBannerCanvasRedTeamObject;
    public GameObject EndBannerCanvasBlueTeamObject;

    public Material redMaterial;
    public Material blueMaterial;
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
    }

    private void VictoryHandler()
    {
        
        if(score.scoreRedTeam >= 5)
        {
            EndBannerCanvasRedTeamObject.SetActive(true);

            Invoke("TriggerWinRed", 1.5f);
        }
        else if(score.scoreBlueTeam >= 5)
        {
            EndBannerCanvasBlueTeamObject.SetActive(true);

            Invoke("TriggerWinBlue", 1.5f);
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
    public void CreatePlayer(string playerName, Team playerTeam)
    {
        GameObject player = null;
        if (playerTeam == Team.BLUE_TEAM)
        {
            player = Instantiate(playerPrefabBlue, startingBluePos, Quaternion.identity, null);
        }
        else if (playerTeam == Team.RED_TEAM)
        {
            player = Instantiate(playerPrefabRed, startingRedPos, Quaternion.identity, null);

        }
        player.GetComponent<PlayerController>().username = playerName;
        player.GetComponent<PlayerController>().characterData.team = playerTeam;

       
    }
    public void CreateDummies(List<string> dummyNames,List<Team> dummyTeams)
    {
        GameObject dummy = null;
        for(int i = 0; i<dummyNames.Count;i++) 
        {
            if (dummyTeams.ElementAt(i) == Team.BLUE_TEAM)
            {
                dummy = Instantiate(dummyPrefabBlue, startingBluePos, Quaternion.identity, null);

            }
            else if (dummyTeams.ElementAt(i) == Team.RED_TEAM)
            {
                dummy = Instantiate(dummyPrefabRed, startingRedPos, Quaternion.identity, null);

            }
            dummy.GetComponent<DummyController>().username = dummyNames.ElementAt(i);
            dummy.GetComponent<DummyController>().characterData.team = dummyTeams.ElementAt(i);
            dummies.Add(dummy.GetComponent<DummyController>());
        }
        
    }
    public void SetScore(int scoreRed,int scoreBlue)
    {
        score.scoreBlueTeam = scoreBlue;
        score.scoreRedTeam = scoreRed;
        scoreBlueTeam.text = score.scoreBlueTeam.ToString();
        scoreRedTeam.text = score.scoreRedTeam.ToString();
    }
    public void AddScore(Team teamPlayerDead)
    {
        switch(teamPlayerDead)
        {
            case Team.NONE:
                break;
            case Team.RED_TEAM:
                score.scoreBlueTeam++;
                break;
            case Team.BLUE_TEAM:
                score.scoreRedTeam++;
                break;
        }

        scoreBlueTeam.text = score.scoreBlueTeam.ToString();
        scoreRedTeam.text = score.scoreRedTeam.ToString();
    }
}
