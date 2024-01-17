using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UiManager : MonoBehaviour
{
    
    public TMP_InputField InputFieldMessage;
    
    public TMP_Text textForMessages;

    //InputField
    public TMP_InputField inputFieldPlayerName;
    //Dropdowm
    public TMP_Dropdown dropdownTeam;
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
            Message message = new Message(inputFieldPlayerName.text + "/" + dropdownTeam.value.ToString(), null, TypesOfMessage.GENERATE_PLAYERS);
            server.HandleSendingMessages(message);
        }
        else if (client)
        {
            Message message = new Message(inputFieldPlayerName.text + "/" + dropdownTeam.value.ToString(), null, TypesOfMessage.GENERATE_PLAYERS);
            client.SendStartMessage(message);
        }
    }
}
