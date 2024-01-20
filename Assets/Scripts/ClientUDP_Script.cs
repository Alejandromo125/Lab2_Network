using UnityEngine;
using System.Net.Sockets;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Net;
using System.Threading;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ClientUDP_Script : MonoBehaviour
{

    [SerializeField]
    private TMP_InputField InputFieldTextIP;
    [SerializeField]
    private TMP_InputField InputFieldTextUserName;

    private UdpClient udpClient;
    private string currentServerIP;
    public string userName;
    private int serverPort = 12345;
    //private int clientPort;


    private Thread listenerThread;

    private Thread checkerThread;

    private Message lastMessage;
    private bool lastMessageUpdated = true;

    private bool CreatePlayer = true;

    #region ConnectionCheckerTimers
    private float lastPingTime;
    #endregion

    public Team userTeam;

    public List<string> player = new List<string>();
    public List<Team> teams = new List<Team>();
    //Creating an instance to access it through player's scripts
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Update()
    {
        if (lastMessageUpdated == false)
        {
            HandleMessageOutput(lastMessage);
            lastMessageUpdated = true;
        }

        if (CreatePlayer == true && GameManager.instance)
        {
            GameManager.instance.CreatePlayer(userName,userTeam);
            GameManager.instance.CreateDummies(player, teams);
            //SendCheckConnection();
            //checkerThread = new Thread(HandleCheck);
            CreatePlayer = false;
        }

    }
    public async void LogIn()
    {
        currentServerIP = InputFieldTextIP.text;
        userName = InputFieldTextUserName.text;
        udpClient = new UdpClient();

        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, 0));

        string connectText = "/connect:" + InputFieldTextUserName.text;
        Message _message = new Message(connectText, null, TypesOfMessage.WAITING_ROOM);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        await udpClient.SendAsync(data, data.Length, recipientEndPoint);

        listenerThread = new Thread(RecieveMessages);
        listenerThread.Start();

        SceneManager.LoadScene("ServerScene");
    }
    #region CheckConnectionMessages
    public void SendCheckConnection()
    {
        string message = "ping";
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        Message _message = new Message(message, null, TypesOfMessage.CHECK_CONNECTION);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
    }
    #endregion
    #region WaitingRoomMessages
    public void SendMessageWaitingRoom()
    {
        string message = userName + ":" + UiManager.instance.InputFieldMessage.text;
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        Message _message = new Message(message, null, TypesOfMessage.WAITING_ROOM);
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
        UiManager.instance.UpdateText(message);
    }
    #endregion

    #region GameplayRoomMessages
    public void SendMessageGameplay(Message _message)
    {
        _message.message = userName + ":" + _message.message;
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
    }
    #endregion
    #region StartGameMessage
    public void SendStartMessage(Message _message)
    {
        string jsonData = JsonUtility.ToJson(_message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);
        IPEndPoint recipientEndPoint = new IPEndPoint(IPAddress.Parse(currentServerIP), serverPort);
        udpClient.SendAsync(data, data.Length, recipientEndPoint);
    }
    #endregion

    private async void RecieveMessages()
    {
        while (true)
        {

            try
            {
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                Message newMessage = JsonUtility.FromJson<Message>(receivedMessage);

                lastMessage = newMessage;
                lastMessageUpdated = false;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

    }
    private void HandleMessageOutput(Message message)
    {
        try
        {
            switch (message.type)
            {
                case TypesOfMessage.WAITING_ROOM:
                    if (isFromAnotherUser(message.message))
                    {
                        UiManager.instance.UpdateText(message.message);
                    }
                    break;
                case TypesOfMessage.GAMEPLAY_ROOM:
                    if (isFromAnotherUser(message.message))
                    {
                        message.message = ReturnCorrectDummyName(message.message);
                        GameManager.instance.UpdateDummiesData(message);
                    }
                    break;
                case TypesOfMessage.START_GAME:
                    SceneManager.LoadSceneAsync("GameplayLevelRoom");                 
                    break;
                case TypesOfMessage.CHECK_CONNECTION:
                    lastPingTime = Time.time;
                    SendCheckConnection();
                    break;
                case TypesOfMessage.DUMMY_SHOOT:
                    string[] splittedMessage_score = message.message.Split('/');

                    GameManager.instance.SetScore(int.Parse(splittedMessage_score[0]), int.Parse(splittedMessage_score[1]));
                    break;
                case TypesOfMessage.GENERATE_PLAYERS:
                    string[] splittedMessage = message.message.Split('/');
                    if (isFromAnotherUserDummies(splittedMessage[0]))
                    {
                        player.Add(splittedMessage.ElementAt(0));
                        teams.Add((Team)int.Parse(splittedMessage.ElementAt(1)));
                    }
                    break;
                case TypesOfMessage.FINISH_GAME:
                    if (GameManager.instance.score.scoreBlueTeam >= 5)
                    {
                        
                        SceneManager.LoadScene("BlueTeamWinsScene");
                    }
                    else if (GameManager.instance.score.scoreRedTeam >= 5)
                    {
                        
                        SceneManager.LoadScene("RedTeamWinsScene");
                    }
                    else
                    {
                        
                        SceneManager.LoadSceneAsync("MainMenuScene");
                    }
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);

        }
    }
    private void OnDisable()
    {
        udpClient.Close();
    }

    private bool isFromAnotherUser(string infoSent)
    {
        string[] messageParts = infoSent.Split(':');
        if (messageParts[0] == userName)
        {
            Debug.Log("Same user");
            return false;
        }
        else
        {
            return true;
        }
    }
    private bool isFromAnotherUserDummies(string infoSent)
    {
        if (infoSent == userName)
        {
            Debug.Log("Same user");
            return false;
        }
        else
        {
            return true;
        }
    }

    private string ReturnCorrectDummyName(string message)
    {
        string[] _messageArray = message.Split(':');

        return _messageArray[1];
    }

    public string GetUsername()
    {
        return userName;
    }
    private async void HandleCheck()
    {
        while (true)
        {
            if (lastPingTime + 30 < Time.time)
            {
                checkerThread.Join();
                listenerThread.Join();
                SceneManager.LoadScene("MainMenuScene");
            }
        }
    }

    public void DestroyClient()
    {
        DontDestroyOnLoad(gameObject);
        udpClient.Close();
        Destroy(gameObject);
    }


    public void SetNameAndTeam(string playerName, Team team)
    {
        userName = playerName;
        userTeam = team;
    }
}
