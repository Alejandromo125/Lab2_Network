using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class ServerUDP_Script : MonoBehaviour
{

    private UdpClient udpListener;
    private int port = 12345;
    private bool lastMessageUpdated = true;
    private Message lastMessage;
    private Thread serverThread;
    private List<IPEndPoint> clientEndPoint = new List<IPEndPoint>();
    private bool playerCreated = true;

    public List<string> player = new List<string>();
    public List<Team> teams = new List<Team>();

    public string PlayerName;
    public Team PlayerTeam;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private async void Start()
    {
        udpListener = new UdpClient(port);


        Debug.Log("Host started on port " + port);
        UdpReceiveResult result = await udpListener.ReceiveAsync();
        clientEndPoint.Add(result.RemoteEndPoint);
        string message = Encoding.UTF8.GetString(result.Buffer);
        StartWaitingScene();

        serverThread = new Thread(HandleRecieveMessages);
        serverThread.Start();

    }
    private void Update()
    {
        if (lastMessageUpdated == false)
        {
            HandleMessageOutput(lastMessage);
            lastMessageUpdated = true;
        }

        if (playerCreated == true && GameManager.instance)
        {
            GameManager.instance.CreatePlayer(PlayerName, PlayerTeam);
            GameManager.instance.CreateDummies(player,teams);
            
            playerCreated = false;
        }
    }

    private void OnDisable()
    {
        udpListener.Close();
    }
    public void StartWaitingScene()
    {
        Message message = new Message("/start_room", null, TypesOfMessage.WAITING_ROOM);
        HandleSendingMessages(message);
    }
    public void HandleCallbackEvent()
    {
        string message = "Host" + ":" + UiManager.instance.InputFieldMessage.text;
        Message newMessage = new Message(message, null, TypesOfMessage.WAITING_ROOM);
        HandleSendingMessages(newMessage);
    }

    private async void HandleRecieveMessages()
    {
        while (true)
        {
            UdpReceiveResult result = await udpListener.ReceiveAsync();
            string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
            Message newMessage = JsonUtility.FromJson<Message>(receivedMessage);

            lastMessage = newMessage;
            lastMessageUpdated = false;


            if (!clientEndPoint.Contains(result.RemoteEndPoint))
            {
                Debug.Log(newMessage.message);
                clientEndPoint.Add(result.RemoteEndPoint);
            }

            HandleSendingMessages(newMessage);

        }

    }

    private void HandleMessageOutput(Message message)
    {
        try
        {
            switch (message.type)
            {
                case TypesOfMessage.CHECK_CONNECTION:
                    message.message = "pong";
                    HandleSendingMessages(message);
                    break;

                case TypesOfMessage.GAMEPLAY_ROOM:
                    message.message = ReturnCorrectDummyName(message.message);
                    GameManager.instance.UpdateDummiesData(message);
                    break;
                case TypesOfMessage.GENERATE_PLAYERS:
                    string[] splittedMessage = message.message.Split('/');
                    player.Add(splittedMessage[0]);
                    teams.Add((Team)int.Parse(splittedMessage[1]));
                    break;
                case TypesOfMessage.DUMMY_SHOOT:
                    break;
                case TypesOfMessage.FINISH_GAME:

                    //serverThread.Join();
                    //SceneManager.LoadSceneAsync("MainMenuScene");
                    if (GameManager.instance.score.scoreBlueTeam >= 5)
                    {
                        SceneManager.LoadScene("BlueTeamWinsScene");
                    }
                    else if (GameManager.instance.score.scoreRedTeam >= 5)
                    {
                        SceneManager.LoadScene("RedTeamWinsScene");
                    }
                    break;

            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
            //serverThread.Join();
            //SceneManager.LoadSceneAsync("MainMenuScene");
            if (FindObjectOfType<PlayerController>().characterData.GameScore >= 5)
            {
                SceneManager.LoadSceneAsync("WinScene");
            }
            else if (FindObjectOfType<DummyController>().characterData.GameScore >= 5)
            {
                SceneManager.LoadSceneAsync("LooseScene");
            }
            else
            {
                SceneManager.LoadSceneAsync("MainMenuScene");
            }
        }
    }

    public void HandleSendingMessages(Message message)
    {

        switch (message.type)
        {
            case TypesOfMessage.CHECK_CONNECTION:
                Debug.Log("Connection check from client");
                break;
            case TypesOfMessage.WAITING_ROOM:
                if (UiManager.instance != null)
                {
                
                    UiManager.instance.UpdateText(message.message);
                    
                }
                break;
            case TypesOfMessage.DUMMY_SHOOT:
                GameManager.instance.AddScore((Team)int.Parse(message.message));
                message.message = GameManager.instance.score.scoreRedTeam + "/" + GameManager.instance.score.scoreBlueTeam;
                break;
            case TypesOfMessage.GAMEPLAY_ROOM:
                message.message = "Server:" + message.message;
                break;
            case TypesOfMessage.START_GAME:
                Debug.Log("SEND MESSAGE");
                SceneManager.LoadSceneAsync("GameplayLevelRoom");
                break;
            case TypesOfMessage.FINISH_GAME:

                //serverThread.Join();
                //SceneManager.LoadScene("MainMenuScene");
                if (GameManager.instance.score.scoreBlueTeam >= 5)
                {
                    SceneManager.LoadScene("BlueTeamWinsScene");
                }
                else if(GameManager.instance.score.scoreRedTeam >= 5)
                {
                    SceneManager.LoadScene("RedTeamWinsScene");
                }
                break;
            case TypesOfMessage.UPDATE_SCORE:
                string[] splittedMessage_score = message.message.Split('/');

                GameManager.instance.SetScore(int.Parse(splittedMessage_score[0]), int.Parse(splittedMessage_score[1]));
                break;
        }


        string jsonData = JsonUtility.ToJson(message);
        byte[] data = Encoding.UTF8.GetBytes(jsonData);

        foreach (IPEndPoint client in clientEndPoint)
        {
            udpListener.SendAsync(data, data.Length, client);
        }
    }


    private string ReturnCorrectDummyName(string message)
    {
        string[] _messageArray = message.Split(':');

        return _messageArray[1];
    }
    private bool isFromAnotherUserDummies(string infoSent)
    {
        if (infoSent == PlayerName)
        {
            Debug.Log("Same user");
            return false;
        }
        else
        {
            return true;
        }
    }
    public void DestroyServer()
    {
        DontDestroyOnLoad(gameObject);
        udpListener.Close();
        Destroy(gameObject);
    }
    public void SetNameAndTeam(string playerName,Team team)
    {
        PlayerName = playerName;
        PlayerTeam = team;
    }
}
