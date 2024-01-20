using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    
    public TMP_InputField InputFieldMessage;
    
    public TMP_Text textForMessages;
    public TMP_Text blueTeamText;
    public TMP_Text redTeamText;


    //InputField
    public TMP_InputField inputFieldPlayerName;
    //Dropdowm
    public TMP_Dropdown dropdownTeam;
    public Button generatorButton;

    private int lastPlayersCountServer;
    private int lastPlayersCountClient;
    public static UiManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    private void Start()
    {
        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if (server)
        {
            inputFieldPlayerName.text = "Host";
        }
        else if (client)
        {
            inputFieldPlayerName.text = client.userName;
        }
    }
    private void Update()
    {
        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if (server)
        {
            if (server.player.Count > lastPlayersCountServer)
            {
                UpdateTeams();
            }
        }
        else if (client)
        {
            if (client.player.Count > lastPlayersCountClient)
            {
                UpdateTeams();
            }
        }
    }
    public void UpdateText(string text_for_update)
    {
        textForMessages.text += text_for_update + "\n";
    }
    public void SendMessage()
    {
        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if(server)
        {
            server.HandleCallbackEvent();
        }
        else if(client)
        {
            client.SendMessageWaitingRoom();
        }
        
    }
    public void StartGame()
    {
        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if(server)
        {
            Message message = new Message("Server", null, TypesOfMessage.START_GAME);
            server.HandleSendingMessages(message);
        }
    }
    public void UpdateTeams()
    {
        blueTeamText.text = "";
        redTeamText.text = "";

        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if (server)
        {
            switch(server.PlayerTeam)
            {
                case Team.NONE: break;
                case Team.BLUE_TEAM:
                    blueTeamText.text += server.PlayerName + "\n";
                    break;

                case Team.RED_TEAM:
                    redTeamText.text += server.PlayerName + "\n";
                    break;
            }
            for (int i = 0; i < server.player.Count; i++)
            {
                switch (server.teams.ElementAt(i))
                {
                    case Team.NONE: break;
                    case Team.RED_TEAM:
                        redTeamText.text += server.player.ElementAt(i) + "\n";
                        break;
                    case Team.BLUE_TEAM: 
                        blueTeamText.text += server.player.ElementAt(i) + "\n";
                        break;
                }
            }
            lastPlayersCountServer = server.player.Count;
        }
        else if (client)
        {
            switch (client.userTeam)
            {
                case Team.NONE: break;
                case Team.BLUE_TEAM:
                    blueTeamText.text += client.userName + "\n";
                    break;

                case Team.RED_TEAM:
                    redTeamText.text += client.userName + "\n";
                    break;
            }
            for (int i = 0; i < client.player.Count; i++)
            {
                switch (client.teams.ElementAt(i))
                {
                    case Team.NONE: break;
                    case Team.RED_TEAM:
                        redTeamText.text += client.player.ElementAt(i) + "\n";
                        break;
                    case Team.BLUE_TEAM:
                        blueTeamText.text += client.player.ElementAt(i) + "\n";
                        break;
                }
            }
            lastPlayersCountClient = client.player.Count;
        }
    }
    public void TurnOffGeneratorObjets()
    {
        inputFieldPlayerName.gameObject.SetActive(false);
        dropdownTeam.gameObject.SetActive(false);
        generatorButton.gameObject.SetActive(false);
            
    }
    public void GoToTitleScreen()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    //Generate
    public void GenerateValuesForPlayers()
    {
        ServerUDP_Script server = FindObjectOfType<ServerUDP_Script>();
        ClientUDP_Script client = FindObjectOfType<ClientUDP_Script>();
        if (server)
        {
            server.SetNameAndTeam(inputFieldPlayerName.text, (Team)(dropdownTeam.value + 1));
            Message message = new Message(inputFieldPlayerName.text + "/" + (dropdownTeam.value + 1).ToString(), null, TypesOfMessage.GENERATE_PLAYERS);
            server.HandleSendingMessages(message);
        }
        else if (client)
        {
            client.SetNameAndTeam(inputFieldPlayerName.text, (Team)(dropdownTeam.value + 1));
            Message message = new Message(inputFieldPlayerName.text + "/" + (dropdownTeam.value + 1).ToString(), null, TypesOfMessage.GENERATE_PLAYERS);
            client.SendStartMessage(message);
        }
        TurnOffGeneratorObjets();
        UpdateTeams();
    }
}
